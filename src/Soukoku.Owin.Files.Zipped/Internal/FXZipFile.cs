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
    class FXZipFile : IZipFile
    {
        ZipArchive _zip;
        public FXZipFile(Stream stream)
        {
            _zip = new ZipArchive(stream, ZipArchiveMode.Read);
        }

        public IEnumerable<IZipEntry> Entries
        {
            get
            {
                return _zip.Entries.Select(e => new FXZipEntry(e));
            }
        }

        public void Dispose()
        {
            if (_zip != null)
                _zip.Dispose();
        }

        class FXZipEntry : IZipEntry
        {
            private ZipArchiveEntry _entry;

            public FXZipEntry(ZipArchiveEntry entry)
            {
                this._entry = entry;
            }

            public DateTime Created
            {
                get
                {
                    return _entry.LastWriteTime.UtcDateTime;
                }
            }

            public string FullPath
            {
                get
                {
                    return _entry.FullName;
                }
            }

            public bool IsFolder
            {
                get
                {
                    return string.IsNullOrEmpty(_entry.Name);
                }
            }

            public DateTime Modified
            {
                get
                {
                    return _entry.LastWriteTime.UtcDateTime;
                }
            }

            public long OrigSize
            {
                get
                {
                    return _entry.Length;
                }
            }

            public Stream Open()
            {
                return _entry.Open();
            }
        }
    }
}
