﻿using Soukoku.Owin.Webdav.Models;
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
        }

        string PhysicalPath { get { return _info.FullName; } }
        public override ResourceType Type { get { return ResourceType.Resource; } }
        public override long Length { get { return _info.Length; } }
        public override DateTime CreationDateUtc { get { return _info.CreationTimeUtc; } }
        public override DateTime ModifiedDateUtc { get { return _info.LastWriteTimeUtc; } }

        public override Stream OpenReadStream()
        {
            return new FileStream(PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}