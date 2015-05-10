using Microsoft.Owin;
using Owin.Webdav;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Owin
{
    public static class WebdavMiddlewareExtensions
    {
        public static IAppBuilder UseWebdav(this IAppBuilder app, WebdavConfig options)
        {
            return app.Use<WebdavMiddleware>(options);
        }


        internal static CancellationToken GetCancellationToken(this IOwinContext context)
        {
            return (CancellationToken)context.Environment["owin.CallCancelled"];
        }
        internal static int GetDepth(this IOwinContext context)
        {
            int depth;
            int.TryParse(context.Request.Headers.GetValues("Depth").FirstOrDefault(), out depth);
            if (depth != 0 && depth != 1)
            {
                depth = int.MaxValue;
            }
            return depth;
        }

        internal static string GenerateStatusMessage(this HttpStatusCode code, string message = null)
        {
            return string.Format("HTTP/1.1 {0} {1}", (int)code, message ?? code.ToString());
        }
    }
}