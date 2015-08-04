using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Soukoku.Owin;

namespace Soukoku.Owin.Files.Services.BuiltIn
{
    /// <summary>
    /// Implements <see cref="IDataStore"/> over loose static files.
    /// </summary>
    public class LooseFilesDataStore : IReadOnlyDataStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LooseFilesDataStore"/> class.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <exception cref="System.ArgumentException">Invalid root path.;rootPath</exception>
        public LooseFilesDataStore(string rootPath)
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

        public ResourceResult GetResource(OwinContext context, string logicalPath)
        {
            var fullPath = MapPath(logicalPath);
            return OnCreateResource(context, logicalPath, fullPath);
        }

        public IEnumerable<ResourceResult> GetSubResources(OwinContext context, Resource parentFolder)
        {
            if (parentFolder == null) { throw new ArgumentNullException("parentFolder"); }

            if (parentFolder.IsFolder)
            {
                var fullPath = MapPath(parentFolder.LogicalPath);

                if (Directory.Exists(fullPath))
                {
                    foreach (var itemPath in Directory.GetFileSystemEntries(fullPath))
                    {
                        yield return OnCreateResource(context, Path.Combine(parentFolder.LogicalPath, Path.GetFileName(itemPath)), itemPath);
                    }
                }
            }
        }

        public Stream Open(Resource resource)
        {
            if (resource == null) { throw new ArgumentNullException("resource"); }
            if (resource.IsFolder) { throw new InvalidOperationException("Cannot open stream for a folder."); }

            var path = MapPath(resource.LogicalPath);
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        #region utilities


        /// <summary>
        /// Maps the logical path into the full physical path.
        /// </summary>
        /// <param name="logicalPath">The logical path.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Accessing paths outside of root is not allowed.</exception>
        protected string MapPath(string logicalPath)
        {
            return MapPath(logicalPath, true);
        }

        /// <summary>
        /// Maps the logical path into the full physical path.
        /// </summary>
        /// <param name="logicalPath">The logical path.</param>
        /// <param name="throwIfOutsideRoot">if set to <c>true</c> then throw an exception if outside root.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Accessing paths outside of root is not allowed.</exception>
        protected string MapPath(string logicalPath, bool throwIfOutsideRoot)
        {
            if (logicalPath == null)
            {
                logicalPath = "";
            }
            else if (logicalPath.StartsWith("/", StringComparison.Ordinal))
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

        protected virtual ResourceResult OnCreateResource(OwinContext context, string logicalPath, string fullPath)
        {
            if (Directory.Exists(fullPath))
            {
                var info = new DirectoryInfo(fullPath);
                return new ResourceResult
                {
                    Resource = new Resource(context, logicalPath, true)
                    {
                        CreationDateUtc = info.CreationTimeUtc,
                        ModifiedDateUtc = info.LastWriteTimeUtc
                    }
                };
            }
            else if (File.Exists(fullPath))
            {
                var info = new FileInfo(fullPath);
                return new ResourceResult
                {
                    Resource = new Resource(context, logicalPath, false)
                    {
                        CreationDateUtc = info.CreationTimeUtc,
                        ModifiedDateUtc = info.LastWriteTimeUtc,
                        Length = info.Length,
                    }
                };
            }
            return new ResourceResult { StatusCode = System.Net.HttpStatusCode.NotFound };
        }


        #endregion
    }
}
