using Owin.Webdav.Models;
using System.IO;
using System;

namespace Owin.Webdav
{
    class LocalFileResource : Resource
    {
        private FileInfo _info;

        public LocalFileResource(string logicalPath, string fullPath) : base(logicalPath)
        {
            _info = new FileInfo(fullPath);
        }

        public string FullPath { get { return _info.FullName; } }
        public override ResourceType Type { get { return ResourceType.File; } }
        public override DateTime CreateDate { get { return _info.CreationTimeUtc; } }
        public override DateTime ModifyDate { get { return _info.LastWriteTimeUtc; } }
        public override long Length { get { return _info.Length; } }

        public override Stream GetReadStream()
        {
            return new FileStream(FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}