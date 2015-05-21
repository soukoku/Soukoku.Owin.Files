using Soukoku.Owin.Webdav.Models;
using System.IO;
using System;
using Microsoft.Owin;

namespace Owin.Webdav
{
    class FileResource : Resource
    {
        private FileInfo _info;

        public FileResource(IOwinContext context, string logicalPath, string physicalPath) : base(context, logicalPath)
        {
            _info = new FileInfo(physicalPath);
            Length.Value = _info.Length;
            CreateDate.Value = _info.CreationTimeUtc;
            ModifyDate.Value = _info.LastWriteTimeUtc;
        }

        public string PhysicalPath { get { return _info.FullName; } }
        public override ResourceType Type { get { return ResourceType.File; } }

        public override Stream GetReadStream()
        {
            return new FileStream(PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}