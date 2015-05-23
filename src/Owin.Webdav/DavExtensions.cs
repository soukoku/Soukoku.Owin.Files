using Soukoku.Owin;
using Soukoku.Owin.Webdav;
using Soukoku.Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Soukoku.Owin.Webdav
{
    /// <summary>
    /// Contains extension methods for webdav component.
    /// </summary>
    static class WebdavMiddlewareExtensions
    {
        ///// <summary>
        ///// Uses the webdav middleware.
        ///// </summary>
        ///// <param name="app">The application.</param>
        ///// <param name="options">The options.</param>
        ///// <returns></returns>
        //public static IAppBuilder UseWebdav(this IAppBuilder app, WebdavConfig options)
        //{
        //    return app.Use<WebdavMiddleware>(options);
        //}

        internal static int GetDepth(this Context context)
        {
            int depth;
            var values = context.Request.Headers[DavConsts.Headers.Depth];
            if (int.TryParse(values, out depth))
            {
                if (depth != 0 && depth != 1)
                {
                    depth = int.MaxValue;
                }
            }
            return depth;
        }

        internal static string GenerateUrl(this Context context, IResource resource)
        {
            var tentative = string.Format(CultureInfo.InvariantCulture, "{0}://{1}{2}{3}", context.Request.Scheme, context.Request.Host, context.Request.PathBase, resource.LogicalPath);//.TrimEnd('/');
            if (resource.ResourceType == ResourceType.Collection)
            {
                tentative += "/";
            }
            return tentative;
        }

        internal static string GenerateStatusMessage(this StatusCode code, string message = null)
        {
            return string.Format(CultureInfo.InvariantCulture, "HTTP/1.1 {0} {1}", (int)code, message ?? code.ToString());
        }

        internal static async Task<string> ReadRequestStringAsync(this Context context)
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

        internal static string PrettySize(this long fileSize)
        {
            var format = "{0:0.##} B";
            if (fileSize > 1024)
            {
                fileSize /= 1024;
                format = "{0:0.##} KB";
            }
            if (fileSize > 1024)
            {
                fileSize /= 1024;
                format = "{0:0.##} MB";
            }
            if (fileSize > 1024)
            {
                fileSize /= 1024;
                format = "{0:0.##} GB";
            }
            if (fileSize > 1024)
            {
                fileSize /= 1024;
                format = "{0:0.##} TB";
            }

            return string.Format(CultureInfo.InvariantCulture, format, fileSize);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Not an actual problem.")]
        internal static byte[] Serialize(this XmlDocument xmlDoc)
        {
            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms, Encoding.UTF8))
            {
                xmlDoc.Save(writer);
                return ms.ToArray();
            }
        }
    }
}