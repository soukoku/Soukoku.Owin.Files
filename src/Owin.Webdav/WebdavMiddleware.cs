using Microsoft.Owin;
using Soukoku.Owin.Webdav.Models;
using Soukoku.Owin.Webdav.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Owin;
using System.Globalization;

using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
using Soukoku.Owin.Webdav.Handlers;

namespace Soukoku.Owin.Webdav
{
    /// <summary>
    /// Owin middle ware for webdav function.
    /// </summary>
    public class WebdavMiddleware
    {
        readonly WebdavConfig _options;
        readonly AppFunc _next;
        readonly Dictionary<string, IMethodHandler> _handlers;

        /// <summary>
        /// Instantiates the middleware with a pointer to the next component.
        /// </summary>
        /// <param name="next">The next component.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="System.ArgumentNullException">
        /// next
        /// or
        /// options
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is what Owin does.")]
        public WebdavMiddleware(AppFunc next, WebdavConfig options)
        {
            if (next == null) { throw new ArgumentNullException("next"); }
            if (options == null) { throw new ArgumentNullException("options"); }

            _next = next;
            _options = options;
            _handlers = new Dictionary<string, IMethodHandler>(StringComparer.OrdinalIgnoreCase);
            _handlers.Add(Consts.Method.Options, new OptionsHandler(_options));
            _handlers.Add(Consts.Method.Get, new GetHandler(_options));
            _handlers.Add(Consts.Method.PropFind, new PropFindHandler(_options));
        }


        /// <summary>
        /// Process the owin request.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <returns></returns>
        public async Task Invoke(IDictionary<string, object> environment)
        {
            var context = new OwinContext(environment);

            try
            {
                var logicalPath = Uri.UnescapeDataString(context.Request.Uri.AbsolutePath.Substring(context.Request.PathBase.Value.Length));
                if (logicalPath.StartsWith("/", StringComparison.Ordinal)) { logicalPath = logicalPath.Substring(1); }
                _options.Log.LogDebug("{0} for {1}", context.Request.Method, logicalPath);


                IResource resource = _options.DataStore.GetResource(context, logicalPath);

                var fullUrl = context.Request.Uri.ToString();
                if (resource != null && resource.Type == ResourceType.Collection && !fullUrl.EndsWith("/", StringComparison.Ordinal))
                {
                    context.Response.Headers.Append("Content-Location", fullUrl + "/");
                }

                var handled = false;
                IMethodHandler handler;
                if (_handlers.TryGetValue(context.Request.Method, out handler))
                {
                    handled = await handler.HandleAsync(context, resource);
                }

                if (!handled)
                {
                    await _next.Invoke(environment);
                }
            }
            catch (Exception ex)
            {
                _options.Log.LogError(ex.ToString());
                //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //return context.Response.WriteAsync(ex.Message);
                throw;
            }
        }
    }
}
