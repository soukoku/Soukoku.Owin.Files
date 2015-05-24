using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soukoku.Owin.Webdav.Models;
using System.Reflection;
using Soukoku.Owin;
using Soukoku.Owin.Webdav;
using Soukoku.Owin.Webdav.Responses;

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

        public IEnumerable<ResourceResponse> CopyTo(IResource resource, string targetPath)
        {
            throw new NotImplementedException();
        }

        public ResourceResponse Delete(IResource resource)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DavLock> GetLocks(IResource resource)
        {
            throw new NotImplementedException();
        }

        public ResourceResponse GetResource(DavContext context, string logicalPath)
        {
            var fullPath = MapPath(logicalPath);
            return new ResourceResponse { Resource = OnCreateResource(context, logicalPath, fullPath) };
        }

        public IEnumerable<ResourceResponse> GetSubResources(DavContext context, IResource collectionResource)
        {
            if (collectionResource.ResourceType == ResourceType.Collection)
            {
                var fullPath = MapPath(collectionResource.LogicalPath);

                if (Directory.Exists(fullPath))
                {
                    foreach (var itemPath in Directory.GetFileSystemEntries(fullPath))
                    {
                        yield return new ResourceResponse
                        {
                            Resource = OnCreateResource(context, Path.Combine(collectionResource.LogicalPath, Path.GetFileName(itemPath)), itemPath)
                        };
                    }
                }
            }
        }

        public DavLock Lock(IResource resource, DavLock proposed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ResourceResponse> MoveTo(IResource resource, string targetPath)
        {
            throw new NotImplementedException();
        }

        public Stream Read(IResource resource)
        {
            FileResource fr = resource as FileResource;
            if(fr != null)
            {
                return new FileStream(fr.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }

            return null;
        }

        public DavLock RefreshLock(IResource resource, DavLock proposed)
        {
            throw new NotImplementedException();
        }

        public StatusCode Unlock(IResource resource, DavLock currentLock)
        {
            throw new NotImplementedException();
        }

        public StatusCode Write(IResource resource, Stream source, int offset)
        {
            throw new NotImplementedException();
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

        protected virtual IResource OnCreateResource(DavContext context, string logicalPath, string fullPath)
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

        StatusCode IDataStore.CreateCollection(IResource parent, string name)
        {
            var newPath = Path.Combine(MapPath(parent.LogicalPath), name);
            Directory.CreateDirectory(newPath);
            return StatusCode.Created;
        }

        #endregion
    }
}
