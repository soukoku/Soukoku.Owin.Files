using Soukoku.Owin.Webdav.Models;
using System.IO;
using System;
using Soukoku.Owin;
using Soukoku.Owin.Webdav;

namespace Owin.Webdav
{
    /// <summary>
    /// Represenets a static file on a file system.
    /// </summary>
    public class FileResource : DavResource
    {
        private FileInfo _info;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileResource" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logicalPath">The logical path.</param>
        /// <param name="physicalPath">The physical path.</param>
        public FileResource(DavContext context, string logicalPath, string physicalPath) : base(context, logicalPath)
        {
            _info = new FileInfo(physicalPath);
        }

        public string PhysicalPath { get { return _info.FullName; } }

        public override ResourceType ResourceType { get { return ResourceType.Resource; } }

        public override long Length { get { return _info.Length; } }

        public override DateTime CreationDateUtc { get { return _info.CreationTimeUtc; } }

        public override DateTime ModifiedDateUtc { get { return _info.LastWriteTimeUtc; } }

    }
}