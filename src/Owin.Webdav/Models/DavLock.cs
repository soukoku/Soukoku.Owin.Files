using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav.Models
{
    public class DavLock
    {
        public string Token { get; set; }

        public LockType LockType { get; set; }
    }
}
