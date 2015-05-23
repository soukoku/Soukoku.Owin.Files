using System;
using Soukoku.Owin.Webdav.Models;
using System.IO;
using Soukoku.Owin;

namespace Owin.Webdav
{
    /// <summary>
    /// Represenets a static directory on a file system.
    /// </summary>
    public class FolderResource : DavResource
    {
        private DirectoryInfo _info;

        public FolderResource(string pathBase, string logicalPath, string physicalPath) : base(pathBase, logicalPath)
        {
            _info = new DirectoryInfo(physicalPath);
        }

        public string PhysicalPath { get { return _info.FullName; } }

        public override ResourceType ResourceType { get { return ResourceType.Collection; } }

        public override DateTime CreationDateUtc { get { return _info.CreationTimeUtc; } }

        public override DateTime ModifiedDateUtc { get { return _info.LastWriteTimeUtc; } }

    }
}