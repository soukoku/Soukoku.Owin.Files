using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin.Webdav.Models;
using System.Reflection;

namespace Owin.Webdav
{
    public class LocalDataStore : IDataStore
    {
        public LocalDataStore(string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath)) { throw new ArgumentException("Invalid root path.", "rootPath"); }
            rootPath = Path.GetFullPath(rootPath);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            RootPath = rootPath;
        }

        public string RootPath { get; private set; }

        public Resource GetResource(string logicalPath)
        {
            var fullPath = MapPath(logicalPath);
            return MakeIntoResource(logicalPath, fullPath);
        }

        public IEnumerable<Resource> GetSubResources(Resource resource)
        {
            if (resource.Type == ResourceType.Folder)
            {
                var fullPath = MapPath(resource.LogicalPath);

                if (Directory.Exists(fullPath))
                {
                    foreach (var item in Directory.GetFileSystemEntries(fullPath))
                    {
                        yield return MakeIntoResource(Path.Combine(resource.LogicalPath, Path.GetFileName(item)), item);
                    }
                }
            }
        }

        #region utilities

        private string MapPath(string path, bool throwIfOutsideRoot = true)
        {
            if (path.StartsWith("/"))
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

        private static Resource MakeIntoResource(string logicalPath, string fullPath)
        {
            if (Directory.Exists(fullPath))
            {
                return new LocalFolderResource(logicalPath, fullPath);
            }
            else if (File.Exists(fullPath))
            {
                return new LocalFileResource(logicalPath, fullPath);
            }
            return null;
        }

        #endregion
    }
}
