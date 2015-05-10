using System;
using Owin.Webdav.Models;
using System.IO;
using Microsoft.Owin;

namespace Owin.Webdav
{
    class LocalFolderResource : Resource
    {
        private DirectoryInfo _info;

        public LocalFolderResource(IOwinContext context, string logicalPath, string physicalPath) : base(context, logicalPath)
        {
            _info = new DirectoryInfo(physicalPath);
        }

        public string PhysicalPath { get { return _info.FullName; } }
        public override ResourceType Type { get { return ResourceType.Folder; } }
        public override DateTime CreateDate { get { return _info.CreationTimeUtc; } }
        public override DateTime ModifyDate { get { return _info.LastWriteTimeUtc; } }

    }
}