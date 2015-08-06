using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files
{
    /// <summary>
    /// Represents an http response in Owin.
    /// </summary>
    public class OwinResponse
    {
        IDictionary<string, object> _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="OwinResponse"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <exception cref="System.ArgumentNullException">environment</exception>
        public OwinResponse(IDictionary<string, object> environment)
        {
            if (environment == null) { throw new ArgumentNullException("environment"); }

            _environment = environment;
            Headers = new OwinHeaders(environment.Get<IDictionary<string, string[]>>(OwinConsts.ResponseHeaders));
        }

        /// <summary>
        /// A Stream used to write out the response body, if any.
        /// </summary>
        public Stream Body
        {
            get { return _environment.Get<Stream>(OwinConsts.ResponseBody, Stream.Null); }
            set { if (value != null) { _environment[OwinConsts.ResponseBody] = value; } }
        }

        /// <summary>
        /// The response headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        public OwinHeaders Headers { get; private set; }

        /// <summary>
        /// An optional int containing the HTTP response status code as defined in 
        /// RFC 2616 section 6.1.1. The default is 200.
        /// </summary>
        public int StatusCode
        {
            get { return _environment.Get<int>(OwinConsts.ResponseStatusCode, 200); }
            set { _environment[OwinConsts.ResponseStatusCode] = value; }
        }
        /// <summary>
        /// An optional string containing the reason phrase associated the given status code. 
        /// If none is provided then the server SHOULD provide a default as described in 
        /// RFC 2616 section 6.1.1
        /// </summary>
        public string ReasonPhrase
        {
            get { return _environment.Get<string>(OwinConsts.ResponseReasonPhrase); }
            set { _environment[OwinConsts.ResponseReasonPhrase] = value; }
        }
        /// <summary>
        /// An optional string containing the protocol name and version 
        /// (e.g. "HTTP/1.0" or "HTTP/1.1"). If none is provided then the 
        /// "owin.RequestProtocol" key's value is the default.
        /// </summary>
        public string Protocol
        {
            get { return _environment.Get<string>(OwinConsts.ResponseProtocol); }
            set { _environment[OwinConsts.ResponseProtocol] = value; }
        }

        /// <summary>
        /// Writes the string to the response body.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task WriteAsync(string content, CancellationToken cancellationToken)
        {
            if (content == null) { return Task.FromResult(0); }
            var buff = Encoding.UTF8.GetBytes(content);
            return Body.WriteAsync(buff, 0, buff.Length, cancellationToken);
        }
    }
}
