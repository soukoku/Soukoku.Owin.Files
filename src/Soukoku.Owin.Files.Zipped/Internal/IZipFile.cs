using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files.Internal
{
    interface IZipFile : IDisposable
    {
        IEnumerable<IZipEntry> Entries { get; }
    }
    interface IZipEntry
    {
        string FullPath { get; }
        bool IsFolder { get; }
        long OrigSize { get; }
        DateTime Created { get; }
        DateTime Modified { get; }
        Stream Open();
    }
}
