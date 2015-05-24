using Soukoku.Owin;
using Soukoku.Owin.Webdav;
using Soukoku.Owin.Webdav.Models;
using Soukoku.Owin.Webdav.Responses;
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
    static class DavExtensions
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

        /// <summary>
        /// Generates the full URL on the resource.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="pathAbsolute">if set to <c>true</c> then only generate path absolute urls (/path/to/resource), otherwise generate the full url.</param>
        /// <returns></returns>
        public static string GenerateUrl(this Context context, IResource resource, bool pathAbsolute)
        {
            var url = context.Request.PathBase + resource.LogicalPath;

            if (pathAbsolute)
            {
                if (!url.StartsWith("/", StringComparison.Ordinal)) { url = "/" + url; }
            }
            else
            {
                url = context.Request.Scheme + Uri.SchemeDelimiter + context.Request.Host + url;
            }

            if (resource.ResourceType == ResourceType.Collection &&
                !url.EndsWith("/", StringComparison.Ordinal))
            {
                url += "/";
            }
            return url;
        }

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
            }else
            {
                depth = int.MaxValue;
            }
            return depth;
        }

        internal static string GenerateStatusMessage(this StatusCode code, string message = null)
        {
            return string.Format(CultureInfo.InvariantCulture, "HTTP/1.1 {0} {1}", (int)code, message ?? code.ToString());
        }

        internal static XmlDocument ReadRequestAsXml(this Request request)
        {
            XmlDocument doc = null;

            if (request.Headers.ContentType == "application/xml" ||
                request.Headers.ContentType == "text/xml")
            {
                // todo: should somehow make it async?
                doc = new XmlDocument();
                doc.Load(request.Body);
            }
            return doc;
        }

        //internal static async Task<string> ReadRequestStringAsync(this Request request)
        //{
        //    string body = null;

        //    //if (!context.Request.Body.CanSeek)
        //    //{
        //    //    // keep the body around for other components?
        //    //    MemoryStream ms = new MemoryStream();
        //    //    await context.Request.Body.CopyToAsync(ms);
        //    //    context.Request.Body = ms;
        //    //    ms.Position = 0;
        //    //}
        //    using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, false, 4096, true))
        //    {
        //        body = await reader.ReadToEndAsync();
        //    }
        //    //context.Request.Body.Position = 0;
        //    return body;
        //}

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
            using (var writer = new StreamWriter(ms))
            {
                xmlDoc.Save(writer);
                return ms.ToArray();
            }
        }
    }
}