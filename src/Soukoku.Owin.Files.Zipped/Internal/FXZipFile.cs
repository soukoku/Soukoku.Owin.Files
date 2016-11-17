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
        IEnumerable<IZipEntry> _folders;

        public FXZipFile(Stream stream)
        {
            _zip = new ZipArchive(stream, ZipArchiveMode.Read);

            ReadForFolders();
        }
        private void ReadForFolders()
        {
            // why?, cuz some zip files don't have folder entries
            // so just always make them ourselves.
            _folders = _zip.Entries
                .Where(e => !e.IsDirectory())
                .Select(e => Path.GetDirectoryName(e.FullName))
                .Where(path => !string.IsNullOrEmpty(path))
                .Distinct()
                .Select(path => new DummyFolderZipEntry(path))
                .ToList();
        }

        public IEnumerable<IZipEntry> Entries
        {
            get
            {
                return _zip.Entries
                    .Where(e => !e.IsDirectory())
                    .Select(e => new FXZipEntry(e))
                    .Union(_folders);
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
                    // normalize to use /
                    return _entry.FullName.Replace('\\', '/');
                }
            }

            public bool IsFolder
            {
                get
                {
                    return _entry.IsDirectory();
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

            public override string ToString()
            {
                return FullPath;
            }
        }
    }
}
