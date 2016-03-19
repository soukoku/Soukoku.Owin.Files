using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files.Internal
{
    /// <summary>
    /// A stream wrapper that disposes other IDisposables when disposed.
    /// </summary>
    class AttachedDisposableStream : Stream
    {
        IDisposable[] _others;
        Stream _real;
        public AttachedDisposableStream(Stream stream, params IDisposable[] otherDisposables)
        {
            _real = stream;
            _others = otherDisposables;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _real.Dispose();
                if(_others != null)
                {
                    foreach(var o in _others) { o.Dispose(); }
                }
            }
            base.Dispose(disposing);
        }

        public override bool CanRead
        {
            get
            {
                return _real.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return _real.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _real.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return _real.Length;
            }
        }

        public override long Position
        {
            get
            {
                return _real.Position;
            }

            set
            {
                _real.Position = value;
            }
        }

        public override void Flush()
        {
            _real.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _real.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _real.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _real.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _real.Write(buffer, offset, count);
        }
    }
}
