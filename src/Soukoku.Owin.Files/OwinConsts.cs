using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files
{
    static class OwinConsts
    {
        /// <summary>
        /// A Stream with the request body, if any. Stream.Null MAY be used as 
        /// a placeholder if there is no request body.
        /// </summary>
        public const string RequestBody = "owin.RequestBody";

        /// <summary>
        /// An IDictionary<string, string[]> of request headers.
        /// </summary>
        public const string RequestHeaders = "owin.RequestHeaders";

        /// <summary>
        /// A string containing the HTTP request method of the request (e.g., "GET", "POST").
        /// </summary>
        public const string RequestMethod = "owin.RequestMethod";

        /// <summary>
        /// A string containing the request path. 
        /// The path MUST be relative to the "root" of the application delegate.
        /// </summary>
        public const string RequestPath = "owin.RequestPath";

        /// <summary>
        /// A string containing the portion of the request path corresponding 
        /// to the "root" of the application delegate.
        /// </summary>
        public const string RequestPathBase = "owin.RequestPathBase";

        /// <summary>
        /// A string containing the protocol name and version (e.g. "HTTP/1.0" or "HTTP/1.1").
        /// </summary>
        public const string RequestProtocol = "owin.RequestProtocol";

        /// <summary>
        /// A string containing the query string component of the HTTP request URI, 
        /// without the leading "?" (e.g., "foo=bar&amp;baz=quux"). 
        /// The value may be an empty string.
        /// </summary>
        public const string RequestQueryString = "owin.RequestQueryString";

        /// <summary>
        /// A string containing the URI scheme used for the request (e.g., "http", "https").
        /// </summary>
        public const string RequestScheme = "owin.RequestScheme";

        /// <summary>
        /// An optional string that uniquely identifies a request. 
        /// The value is opaque and SHOULD have some level of uniqueness. 
        /// A Host MAY specify this value. If it is not specified, middleware MAY set it. 
        /// Once set, it SHOULD NOT be modified.
        /// </summary>
        public const string RequestId = "owin.RequestId";


        /// <summary>
        /// A Stream used to write out the response body, if any.
        /// </summary>
        public const string ResponseBody = "owin.ResponseBody";

        /// <summary>
        /// An IDictionary<string, string[]> of response headers.
        /// </summary>
        public const string ResponseHeaders = "owin.ResponseHeaders";

        /// <summary>
        /// An optional int containing the HTTP response status code as defined in 
        /// RFC 2616 section 6.1.1. The default is 200.
        /// </summary>
        public const string ResponseStatusCode = "owin.ResponseStatusCode";

        /// <summary>
        /// An optional string containing the reason phrase associated the given status code. 
        /// If none is provided then the server SHOULD provide a default as described in 
        /// RFC 2616 section 6.1.1
        /// </summary>
        public const string ResponseReasonPhrase = "owin.ResponseReasonPhrase";

        /// <summary>
        /// An optional string containing the protocol name and version 
        /// (e.g. "HTTP/1.0" or "HTTP/1.1"). If none is provided then the 
        /// "owin.RequestProtocol" key's value is the default.
        /// </summary>
        public const string ResponseProtocol = "owin.ResponseProtocol";


        /// <summary>
        /// A CancellationToken indicating if the request has been canceled/aborted.
        /// </summary>
        public const string CallCancelled = "owin.CallCancelled";

        /// <summary>
        /// A string indicating the OWIN version.
        /// </summary>
        public const string Version = "owin.Version";
    }
}
