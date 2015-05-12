using Microsoft.Owin;
using Owin.Webdav;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

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
            var values = context.Request.Headers.GetValues("Depth");
            int.TryParse(values.FirstOrDefault(), out depth);
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

        internal static async Task<string> ReadRequestStringAsync(this IOwinContext context)
        {
            string body = null;
            if (context.Request.Body != null)
            {
                //if (!context.Request.Body.CanSeek)
                //{
                //    // keep the body around for other components?
                //    MemoryStream ms = new MemoryStream();
                //    await context.Request.Body.CopyToAsync(ms);
                //    context.Request.Body = ms;
                //    ms.Position = 0;
                //}
                using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, false, 4096, true))
                {
                    body = await reader.ReadToEndAsync();
                }
                //context.Request.Body.Position = 0;
            }
            return body;
        }

        internal static Task<string> ReadStringAsync(this Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEndAsync();
            }
        }

        internal static string PrettyXml(this string xml)
        {
            string formattedXml = XElement.Parse(xml).ToString();
            return formattedXml;
        }

        internal static byte[] Serialize(this XmlDocument xmlDoc)
        {
            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms))
            {
                xmlDoc.Save(writer);
                return ms.ToArray();
            }
        }
    }
}