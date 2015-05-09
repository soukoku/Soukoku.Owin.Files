using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin.Webdav.Models;

namespace Owin.Webdav
{
    public class LocalDataStore : IDataStore
    {
        public LocalDataStore(string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath)) { throw new ArgumentException("Invalid root path.", "rootPath"); }
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            RootPath = rootPath;
        }

        public string RootPath { get; private set; }

        public Resource GetResource(string path)
        {
            var localPath = MapPath(path);
            FileAttributes attr = File.GetAttributes(localPath);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                return new LocalFolderResource(localPath);
            }
            return new LocalFileResource(localPath);
        }

        private string MapPath(string path)
        {
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            path = Path.Combine(RootPath, path);

            // TODO: verify path is not outside root

            return path;
        }
    }
}
