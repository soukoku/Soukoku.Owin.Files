using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.Webdav
{
    static class WebdavConsts
    {
        public const string XmlNamespace = "DAV:";

        public static class Methods
        {
            public const string Options = "OPTIONS";
            public const string PropFind = "PROPFIND";
            public const string PropPatch = "PROPPATCH";
            public const string Copy = "COPY";
            public const string Move = "MOVE";
            public const string Delete = "DELETE";
            public const string MkCol = "MKCOL";
            public const string Lock = "LOCK";
            public const string Unlock = "UNLOCK";
            public const string Get = "GET";
        }
    }
}
