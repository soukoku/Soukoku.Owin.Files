using Microsoft.Owin;
using Soukoku.Owin.Webdav.Handlers;
using Soukoku.Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav
{
    /// <summary>
    /// Named delegate for owin middleware invocation.
    /// </summary>
    /// <param name="environment">The environment.</param>
    /// <returns></returns>
    public delegate Task MiddlewareFunc(IDictionary<string, object> environment);


    /// <summary>
    /// Owin middle ware for webdav function.
    /// </summary>
    public class WebdavMiddleware
    {
        readonly WebdavConfig _options;
        readonly MiddlewareFunc _next;
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
        public WebdavMiddleware(MiddlewareFunc next, WebdavConfig options)
        {
            if (next == null) { throw new ArgumentNullException("next"); }
            if (options == null) { throw new ArgumentNullException("options"); }

            _next = next;
            _options = options;
            _handlers = new Dictionary<string, IMethodHandler>(StringComparer.OrdinalIgnoreCase);
            _handlers.Add(Consts.Methods.Options, new OptionsHandler(_options));
            _handlers.Add(Consts.Methods.Get, new GetHandler(_options));
            _handlers.Add(Consts.Methods.PropFind, new PropFindHandler(_options));
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
