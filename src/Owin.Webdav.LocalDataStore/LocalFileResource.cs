using Owin.Webdav.Models;
using System.IO;
using System;
using Microsoft.Owin;

namespace Owin.Webdav
{
    class LocalFileResource : Resource
    {
        private FileInfo _info;

        public LocalFileResource(IOwinContext context, string logicalPath, string physicalPath) : base(context, logicalPath)
        {
            _info = new FileInfo(physicalPath);
        }

        public string PhysicalPath { get { return _info.FullName; } }
        public override ResourceType Type { get { return ResourceType.File; } }
        public override DateTime CreateDate { get { return _info.CreationTimeUtc; } }
        public override DateTime ModifyDate { get { return _info.LastWriteTimeUtc; } }
        public override long Length { get { return _info.Length; } }

        public override Stream GetReadStream()
        {
            return new FileStream(PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}