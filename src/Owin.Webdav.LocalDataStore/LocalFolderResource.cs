using System;
using Soukoku.Owin.Webdav.Models;
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
            CreateDate.Value = _info.CreationTimeUtc;
            ModifyDate.Value = _info.LastWriteTimeUtc;
        }

        public string PhysicalPath { get { return _info.FullName; } }
        public override ResourceType Type { get { return ResourceType.Folder; } }

    }
}