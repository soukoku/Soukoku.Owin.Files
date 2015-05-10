using System;
using Owin.Webdav.Models;
using System.IO;

namespace Owin.Webdav
{
    class LocalFolderResource : Resource
    {
        private DirectoryInfo _info;

        public LocalFolderResource(string logicalPath, string fullPath) : base(logicalPath)
        {
            _info = new DirectoryInfo(fullPath);
        }

        public string FullPath { get { return _info.FullName; } }
        public override ResourceType Type { get { return ResourceType.Folder; } }
        public override DateTime CreateDate { get { return _info.CreationTimeUtc; } }
        public override DateTime ModifyDate { get { return _info.LastWriteTimeUtc; } }

    }
}