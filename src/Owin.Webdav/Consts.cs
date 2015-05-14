using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Soukoku.Owin.Webdav
{
    /// <summary>
    /// Contains constant values used by webdav.
    /// </summary>
    static class Consts
    {
        /// <summary>
        /// The namespace for all built-in webdav xml names.
        /// </summary>
        public const string XmlNamespace = "DAV:";

        /// <summary>
        /// Contains the http methods used by webdav.
        /// </summary>
        public static class Method
        {
            // spec section 9

            public const string PropFind = "PROPFIND";
            public const string PropPatch = "PROPPATCH";
            public const string MkCol = "MKCOL";
            public const string Get = "GET";
            public const string Head = "HEAD";
            public const string Post = "POST";
            public const string Delete = "DELETE";
            public const string Put = "PUT";
            public const string Copy = "COPY";
            public const string Move = "MOVE";
            public const string Lock = "LOCK";
            public const string Unlock = "UNLOCK";

            public const string Options = "OPTIONS";
        }

        /// <summary>
        /// Contains the http header names used by webdav.
        /// </summary>
        public static class Header
        {
            // spec section 10
            
            public const string Dav = "DAV";
            public const string Depth = "Depth";
            public const string Destination = "Destination";
            public const string If = "If";
            public const string LockToken = "Lock-Token";
            public const string Overwrite = "Overwrite";
            public const string Timeout = "Timeout";
        }

        /// <summary>
        /// Extra http status codes used by webdav.
        /// </summary>
        public enum StatusCode
        {
            // spec section 11

            MultiStatus = 207,
            UnprocessableEntity = 422,
            Locked = 423,
            FailedDependency = 424,
            InsufficientStorage = 507,
        }

        /// <summary>
        /// Contains the xml element names used by webdav
        /// </summary>
        /// public class
        public static class ElementName
        {
            // spec section 14

            public const string ActiveLock = "activelock";
            public const string AllProp = "allprop";
            public const string Collection = "collection";
            public const string Depth = "depth";
            public const string Error = "error";
            public const string Exclusive = "exclusive";
            public const string Href = "href";
            public const string Include = "include";
            public const string Location = "location";
            public const string LockEntry = "lockentry";
            public const string LockInfo = "lockinfo";
            public const string LockRoot = "lockroot";
            public const string LockScope = "lockscope";
            public const string LockToken = "locktoken";
            public const string LockType = "locktype";
            public const string MultiStatus = "multistatus";
            public const string Owner = "owner";
            public const string Prop = "prop";
            public const string PropertyUpdate = "propertyupdate";
            public const string PropFind = "propfind";
            public const string PropName = "propname";
            public const string PropStat = "propstat";
            public const string Remove = "remove";
            public const string Response = "response";
            public const string ResponseDescription = "responsedescription";
            public const string Set = "set";
            public const string Shared = "shared";
            public const string Status = "status";
            public const string Timeout = "timeout";
            public const string Write = "write";

        }
        
        /// <summary>
        /// Contains property names defined by webdav.
        /// </summary>
        public static class PropertyName
        {
            // spec section 15

            public const string CreationDate = "creationdate";
            public const string DisplayName = "displayname";
            public const string GetContentLanguage = "getcontentlanguage";
            public const string GetContentLength = "getcontentlength";
            public const string GetContentType = "getcontenttype";
            public const string GetETag = "getetag";
            public const string GetLastModified = "getlastmodified";
            public const string LockDiscovery = "lockdiscovery";
            public const string ResourceType = "resourcetype";
            public const string SupportedLock = "supportedlock";
        }

    }
}
