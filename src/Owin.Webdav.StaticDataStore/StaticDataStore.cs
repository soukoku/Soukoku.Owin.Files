﻿using System;
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
            return OnCreateResource(context, logicalPath, fullPath);
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
                    foreach (var itemPath in Directory.GetFileSystemEntries(fullPath))
                    {
                        yield return OnCreateResource(context, Path.Combine(collectionResource.LogicalPath, Path.GetFileName(itemPath)), itemPath);
                    }
                }
            }
        }

        #region utilities

        /// <summary>
        /// Maps the logical path into the full physical path.
        /// </summary>
        /// <param name="logicalPath">The logical path.</param>
        /// <param name="throwIfOutsideRoot">if set to <c>true</c> then throw an exception if outside root.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Accessing paths outside of root is not allowed.</exception>
        protected string MapPath(string logicalPath, bool throwIfOutsideRoot = true)
        {
            if (logicalPath.StartsWith("/", StringComparison.Ordinal))
            {
                logicalPath = logicalPath.Substring(1);
            }
            var fullPath = Path.GetFullPath(Path.Combine(RootPath, logicalPath));

            // verify path is not outside root
            if (throwIfOutsideRoot && !fullPath.StartsWith(RootPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Accessing paths outside of root is not allowed.");
            }

            return fullPath;
        }

        protected virtual IResource OnCreateResource(IOwinContext context, string logicalPath, string fullPath)
        {
            if (Directory.Exists(fullPath))
            {
                return new FolderResource(context, logicalPath, fullPath);
            }
            else if (File.Exists(fullPath))
            {
                return new FileResource(context, logicalPath, fullPath);
            }
            // TODO: allow temp lock resources?
            return null;
        }

        #endregion
    }
}
