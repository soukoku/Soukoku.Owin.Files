using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files
{
    using Services;
    using Services.BuiltIn;
    using AppFunc = Func<IDictionary<string, object>, Task>;

    /// <summary>
    /// Owin middle ware for files function.
    /// </summary>
    public class FilesMiddleware
    {
        readonly FilesConfig _options;
        readonly AppFunc _next;
        protected Dictionary<string, IMethodHandler> Handlers { get; private set; }

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is how Owin works.")]
        public FilesMiddleware(AppFunc next, FilesConfig options)
        {
            if (next == null) { throw new ArgumentNullException("next"); }
            if (options == null) { throw new ArgumentNullException("options"); }

            _next = next;
            _options = options;
            Handlers = new Dictionary<string, IMethodHandler>(StringComparer.OrdinalIgnoreCase);

            var getHandler = new GetHandler();
            Handlers.Add(HttpMethods.Get, getHandler);
            Handlers.Add(HttpMethods.Head, getHandler);
        }


        /// <summary>
        /// Process the owin request.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <returns></returns>
        public virtual async Task Invoke(IDictionary<string, object> environment)
        {
            var context = new Context(environment);

            try
            {
                environment[FilesConfig.OwinKey] = _options;

                _options.Log.LogDebug("{0} for {1}{2}", context.Request.Method, context.Request.PathBase, context.Request.Path);

                ResourceResult test = _options.DataStore.GetResource(context, context.Request.Path);

                if (test.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // TODO: handle query string part?
                    //var fullUrl = context.Request.Uri.ToString();
                    //if (resource.Resource != null && resource.Resource.ResourceType == ResourceType.Collection && !fullUrl.EndsWith("/", StringComparison.Ordinal))
                    //{
                    //    context.Response.Headers.Replace("Content-Location", fullUrl + "/");
                    //}

                    var code = 0;
                    IMethodHandler handler;
                    if (Handlers.TryGetValue(context.Request.Method, out handler))
                    {
                        code = await handler.HandleAsync(test.Resource);
                    }

                    if (code == 0)
                    {
                        await _next.Invoke(environment);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)code;
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)test.StatusCode;
                }
            }
            catch (Exception ex)
            {
                _options.Log.LogError("FilesMiddleware.Invoke: " + ex.ToString());
                //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //return context.Response.WriteAsync(ex.Message);
                throw;
            }
        }
    }
}
