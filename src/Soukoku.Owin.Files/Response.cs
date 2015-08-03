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
    public class Response
    {
        IDictionary<string, object> _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <exception cref="System.ArgumentNullException">environment</exception>
        public Response(IDictionary<string, object> environment)
        {
            if (environment == null) { throw new ArgumentNullException("environment"); }

            _environment = environment;
            Headers = new Headers(environment.Get<IDictionary<string, string[]>>(OwinConsts.ResponseHeaders));
        }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public Stream Body
        {
            get { return _environment.Get<Stream>(OwinConsts.ResponseBody, Stream.Null); }
            set { if (value != null) { _environment[OwinConsts.ResponseBody] = value; } }
        }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        public Headers Headers { get; private set; }

        public int StatusCode
        {
            get { return _environment.Get<int>(OwinConsts.ResponseStatusCode, 200); }
            set { _environment[OwinConsts.ResponseStatusCode] = value; }
        }
        public string ReasonPhrase
        {
            get { return _environment.Get<string>(OwinConsts.ResponseReasonPhrase); }
            set { _environment[OwinConsts.ResponseReasonPhrase] = value; }
        }
        public string Protocol
        {
            get { return _environment.Get<string>(OwinConsts.ResponseProtocol); }
            set { _environment[OwinConsts.ResponseProtocol] = value; }
        }

        public Task WriteAsync(string content, CancellationToken cancellationToken)
        {
            if (content == null) { return Task.FromResult(0); }
            var buff = Encoding.UTF8.GetBytes(content);
            return Body.WriteAsync(buff, 0, buff.Length, cancellationToken);
        }
    }
}
