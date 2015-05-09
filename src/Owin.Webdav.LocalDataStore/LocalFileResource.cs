using Owin.Webdav.Models;
using System.IO;

namespace Owin.Webdav
{
    class LocalFileResource : FileResource
    {
        private string _localPath;

        public LocalFileResource(string localPath)
        {
            this._localPath = localPath;
        }

        public override string Name
        {
            get
            {
                return Path.GetFileName(_localPath);
            }
        }
    }
}