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
        IEnumerable<IZipEntry> _folders;

        public IonicZipFile(Stream stream)
        {
            _zip = ZipFile.Read(stream);

            ReadForFolders();
        }

        private void ReadForFolders()
        {
            // why?, cuz some zip files don't have folder entries
            // so just always make them ourselves.
            _folders = _zip.Entries
                .Select(e => Path.GetDirectoryName(e.FileName))
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
                    .Where(e => !e.IsDirectory)
                    .Select(e => new IonicZipEntry(e))
                    .Union(_folders);
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

            public override string ToString()
            {
                return FullPath;
            }
        }
    }
}
