using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Owin.Webdav
{
    static class WebdavConsts
    {
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
            public const string Put = "PUT";
        }

        public static class Xml
        {
            public const string Namespace = "DAV:";

            public const string ResponseList = "multistatus";

            public const string Response = "response";
            public const string RespHref = "href";
            public const string RespProperty = "propstat";
            
            public const string PropertyStatus = "status";
            public const string PropertyList = "prop";
            public const string PropDisplayName = "displayname";
            public const string PropGetContentLength = "getcontentlength";
            public const string PropGetContentType = "getcontenttype";
            public const string PropCreationDate = "creationdate";
            public const string PropGetLastModified = "getlastmodified";
            public const string PropResourceType = "resourcetype";

            // locks
            public const string PropSupportedLock = "supportedlock";
            public const string LockEntry = "lockentry";
            public const string LockScope = "lockscope";
            public const string LockScopeExclusive = "exclusive";
            public const string LockScopeShared = "shared";
            public const string LockType = "locktype";
            public const string LockTypeWrite = "write";

        }
    }
}
