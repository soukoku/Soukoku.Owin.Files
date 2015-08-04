using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files
{
    /// <summary>
    /// Represents an http context in Owin.
    /// </summary>
    public class OwinContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwinContext"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <exception cref="System.ArgumentNullException">environment</exception>
        public OwinContext(IDictionary<string, object> environment)
        {
            if (environment == null) { throw new ArgumentNullException("environment"); }

            Environment = environment;
            CancellationToken = (CancellationToken)environment[OwinConsts.CallCancelled];
        }

        /// <summary>
        /// Gets the environment.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
        public IDictionary<string, object> Environment { get; private set; }

        OwinRequest _request;
        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>
        /// The request.
        /// </value>
        public OwinRequest Request
        {
            get { return _request ?? (_request = OnCreateRequest()); }
        }

        OwinResponse _response;
        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public OwinResponse Response
        {
            get { return _response ?? (_response = OnCreateResponse()); }
        }

        /// <summary>
        /// Gets the cancellation token.
        /// </summary>
        /// <value>
        /// The cancellation token.
        /// </value>
        public CancellationToken CancellationToken { get; private set; }

        /// <summary>
        /// Gets the Owin version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version
        {
            get { return Environment.Get<string>(OwinConsts.Version); }
        }


        /// <summary>
        /// Called when the <see cref="Request"/> property is first called.
        /// </summary>
        /// <returns></returns>
        protected virtual OwinRequest OnCreateRequest()
        {
            return new OwinRequest(Environment);
        }

        /// <summary>
        /// Called when the <see cref="Response"/> property is first called.
        /// </summary>
        /// <returns></returns>
        protected virtual OwinResponse OnCreateResponse()
        {
            return new OwinResponse(Environment);
        }


    }
}
