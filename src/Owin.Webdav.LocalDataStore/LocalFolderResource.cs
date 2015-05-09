using System;
using Owin.Webdav.Models;
using System.IO;

namespace Owin.Webdav
{
    class LocalFolderResource : FolderResource
    {
        private string _localPath;

        public LocalFolderResource(string localPath)
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