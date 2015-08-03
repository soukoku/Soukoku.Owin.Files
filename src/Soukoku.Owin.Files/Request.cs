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
    public class Request
    {
        private IDictionary<string, object> _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <exception cref="System.ArgumentNullException">environment</exception>
        public Request(IDictionary<string, object> environment)
        {
            if (environment == null) { throw new ArgumentNullException("environment"); }

            _environment = environment;
            Headers = new Headers(environment.Get<IDictionary<string, string[]>>(OwinConsts.RequestHeaders));
        }

        /// <summary>
        /// Gets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public Stream Body { get { return _environment.Get<Stream>(OwinConsts.RequestBody, Stream.Null); } }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        public Headers Headers { get; private set; }

        public string Method { get { return _environment.Get<string>(OwinConsts.RequestMethod); } }

        public string Path { get { return _environment.Get<string>(OwinConsts.RequestPath); } }

        public string PathBase { get { return _environment.Get<string>(OwinConsts.RequestPathBase); } }

        public string Protocol { get { return _environment.Get<string>(OwinConsts.RequestProtocol); } }

        public string QueryString { get { return _environment.Get<string>(OwinConsts.RequestQueryString); } }

        public string Scheme { get { return _environment.Get<string>(OwinConsts.RequestScheme); } }

        public string Host { get { return Headers["Host"]; } }

        Uri _uri;
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