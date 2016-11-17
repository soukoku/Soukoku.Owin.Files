using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files.Internal
{
    class DummyFolderZipEntry : IZipEntry
    {
        public DummyFolderZipEntry(string path)
        {
            FullPath = path.Replace('\\', '/');
        }

        public DateTime Created { get; }

        public string FullPath { get; private set; }

        public bool IsFolder { get { return true; } }

        public DateTime Modified { get; }

        public long OrigSize { get; }

        public Stream Open()
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return FullPath;
        }
    }
}
