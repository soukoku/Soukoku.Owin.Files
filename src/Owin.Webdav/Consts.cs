using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Soukoku.Owin.Webdav
{
    static class Consts
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

        }


        public class LockNames
        {
            public const string Entry = "lockentry";
            public const string Scope = "lockscope";
            public const string ScopeExclusive = "exclusive";
            public const string ScopeShared = "shared";
            public const string Type = "locktype";
            public const string TypeWrite = "write";
        }


        public class PropertyNames
        {
            public const string DisplayName = "displayname";
            public const string GetContentLength = "getcontentlength";
            public const string GetContentType = "getcontenttype";
            public const string CreationDate = "creationdate";
            public const string GetLastModified = "getlastmodified";
            public const string ResourceType = "resourcetype";

            // locks
            public const string SupportedLock = "supportedlock";
        }

    }
}
