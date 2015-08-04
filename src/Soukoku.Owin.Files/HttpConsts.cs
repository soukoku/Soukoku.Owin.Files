using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files
{

    /// <summary>
    /// Contains the known http method strings.
    /// </summary>
    public static class HttpMethodNames
    {
        // spec section 9

        public const string PropFind = "PROPFIND";
        public const string PropPatch = "PROPPATCH";
        public const string MakeCollection = "MKCOL";
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
    /// Contains common http headers names.
    /// </summary>
    public static class HttpHeaderNames
    {
        public const string LastModified = "Last-Modified";
        public const string ContentDisposition = "Content-Disposition";
        public const string ContentLength = "Content-Length";
        public const string ContentType = "Content-Type";

    }
}
