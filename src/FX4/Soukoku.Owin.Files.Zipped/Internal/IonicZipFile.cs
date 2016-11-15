using Ionic.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files.Internal
{
    class IonicZipFile : IZipFile
    {
        ZipFile _zip;
        public IonicZipFile(Stream stream)
        {
            _zip = ZipFile.Read(stream);
        }

        public IEnumerable<IZipEntry> Entries
        {
            get
            {
                return _zip.Entries.Select(e => new IonicZipEntry(e));
            }
        }

        public void Dispose()
        {
            if (_zip != null)
                _zip.Dispose();
        }

        class IonicZipEntry : IZipEntry
        {
            private ZipEntry _entry;

            public IonicZipEntry(ZipEntry entry)
            {
                this._entry = entry;
            }

            public DateTime Created
            {
                get
                {
                    return _entry.CreationTime.ToUniversalTime();
                }
            }

            public string FullPath
            {
                get
                {
                    // normalize to use /
                    return _entry.FileName.Replace('\\', '/');
                }
            }

            public bool IsFolder
            {
                get
                {
                    return _entry.IsDirectory;
                }
            }

            public DateTime Modified
            {
                get
                {
                    return _entry.LastModified.ToUniversalTime();
                }
            }

            public long OrigSize
            {
                get
                {
                    return _entry.UncompressedSize;
                }
            }

            public Stream Open()
            {
                return _entry.OpenReader();
            }
        }
    }
}
