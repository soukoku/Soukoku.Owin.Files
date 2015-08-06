using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files
{
    /// <summary>
    /// Represents an http request in Owin.
    /// </summary>
    public class OwinRequest
    {
        private IDictionary<string, object> _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="OwinRequest"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <exception cref="System.ArgumentNullException">environment</exception>
        public OwinRequest(IDictionary<string, object> environment)
        {
            if (environment == null) { throw new ArgumentNullException("environment"); }

            _environment = environment;
            Headers = new OwinHeaders(environment.Get<IDictionary<string, string[]>>(OwinConsts.RequestHeaders));
        }


        /// <summary>
        /// A Stream with the request body, if any. Stream.Null MAY be used as 
        /// a placeholder if there is no request body.
        /// </summary>
        public Stream Body { get { return _environment.Get<Stream>(OwinConsts.RequestBody, Stream.Null); } }

        /// <summary>
        /// The requeset headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        public OwinHeaders Headers { get; private set; }

        /// <summary>
        /// A string containing the HTTP request method of the request (e.g., "GET", "POST").
        /// </summary>
        public string Method { get { return _environment.Get<string>(OwinConsts.RequestMethod); } }

        /// <summary>
        /// A string containing the request path. 
        /// The path MUST be relative to the "root" of the application delegate.
        /// </summary>
        public string Path { get { return _environment.Get<string>(OwinConsts.RequestPath); } }

        /// <summary>
        /// A string containing the portion of the request path corresponding 
        /// to the "root" of the application delegate.
        /// </summary>
        public string PathBase { get { return _environment.Get<string>(OwinConsts.RequestPathBase); } }

        /// <summary>
        /// A string containing the protocol name and version (e.g. "HTTP/1.0" or "HTTP/1.1").
        /// </summary>
        public string Protocol { get { return _environment.Get<string>(OwinConsts.RequestProtocol); } }

        /// <summary>
        /// A string containing the query string component of the HTTP request URI, 
        /// without the leading "?" (e.g., "foo=bar&amp;baz=quux"). 
        /// The value may be an empty string.
        /// </summary>
        public string QueryString { get { return _environment.Get<string>(OwinConsts.RequestQueryString); } }

        /// <summary>
        /// A string containing the URI scheme used for the request (e.g., "http", "https").
        /// </summary>
        public string Scheme { get { return _environment.Get<string>(OwinConsts.RequestScheme); } }

        /// <summary>
        /// An optional string that uniquely identifies a request. 
        /// The value is opaque and SHOULD have some level of uniqueness. 
        /// A Host MAY specify this value. If it is not specified, middleware MAY set it. 
        /// Once set, it SHOULD NOT be modified.
        /// </summary>
        public string Id { get { return _environment.Get<string>(OwinConsts.RequestId); } }

        /// <summary>
        /// Gets the requested host name.
        /// </summary>
        public string Host { get { return Headers["Host"]; } }

        Uri _uri;
        /// <summary>
        /// Gets the reconstructed request URI.
        /// </summary>
        public Uri Uri
        {
            get
            {
                if (_uri == null)
                {
                    var urlTemp = Scheme + Uri.SchemeDelimiter + Host + PathBase + Path;
                    if (!string.IsNullOrEmpty(QueryString))
                    {
                        urlTemp += "?" + QueryString;
                    }
                    _uri = new Uri(urlTemp);
                }
                return _uri;
            }
        }
    }
}