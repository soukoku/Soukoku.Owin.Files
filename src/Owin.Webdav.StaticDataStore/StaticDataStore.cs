using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soukoku.Owin.Webdav.Models;
using System.Reflection;
using Microsoft.Owin;

namespace Owin.Webdav
{
    /// <summary>
    /// Implements <see cref="IDataStore"/> over static files.
    /// </summary>
    public class StaticDataStore : IDataStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticDataStore"/> class.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <exception cref="System.ArgumentException">Invalid root path.;rootPath</exception>
        public StaticDataStore(string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath)) { throw new ArgumentException("Invalid root path.", "rootPath"); }
            rootPath = Path.GetFullPath(rootPath);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            RootPath = rootPath;
        }

        /// <summary>
        /// Gets the root path.
        /// </summary>
        /// <value>
        /// The root path.
        /// </value>
        public string RootPath { get; private set; }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logicalPath">The logical path.</param>
        /// <returns></returns>
        public IResource GetResource(IOwinContext context, string logicalPath)
        {
            var fullPath = MapPath(logicalPath);
            return MakeIntoResource(context, logicalPath, fullPath);
        }

        /// <summary>
        /// Gets the resources under a collection resource.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="collectionResource">The collection resource.</param>
        /// <returns></returns>
        public IEnumerable<IResource> GetSubResources(IOwinContext context, IResource collectionResource)
        {
            if (collectionResource.Type == ResourceType.Collection)
            {
                var fullPath = MapPath(collectionResource.LogicalPath);

                if (Directory.Exists(fullPath))
                {
                    foreach (var item in Directory.GetFileSystemEntries(fullPath))
                    {
                        yield return MakeIntoResource(context, Path.Combine(collectionResource.LogicalPath, Path.GetFileName(item)), item);
                    }
                }
            }
        }

        #region utilities

        private string MapPath(string path, bool throwIfOutsideRoot = true)
        {
            if (path.StartsWith("/", StringComparison.Ordinal))
            {
                path = path.Substring(1);
            }
            path = Path.GetFullPath(Path.Combine(RootPath, path));

            // verify path is not outside root
            if (throwIfOutsideRoot && !path.StartsWith(RootPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Accessing paths outside of root is not allowed.");
            }

            return path;
        }

        private static Resource MakeIntoResource(IOwinContext context, string logicalPath, string fullPath)
        {
            if (Directory.Exists(fullPath))
            {
                return new FolderResource(context, logicalPath, fullPath);
            }
            else if (File.Exists(fullPath))
            {
                return new FileResource(context, logicalPath, fullPath);
            }
            // TODO: allow locknulls
            return null;
        }

        #endregion
    }
}
