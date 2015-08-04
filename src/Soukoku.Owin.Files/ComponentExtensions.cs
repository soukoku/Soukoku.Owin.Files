using Soukoku.Owin;
using Soukoku.Owin.Files.Services;
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

namespace Soukoku.Owin.Files
{
    /// <summary>
    /// Contains extension methods for this component.
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// Generates http 1.1 status message from a status code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static string GenerateStatusMessage(this HttpStatusCode code, string message)
        {
            return string.Format(CultureInfo.InvariantCulture, "HTTP/1.1 {0} {1}", (int)code, message ?? code.ToString());
        }

        /// <summary>
        /// Reads the stream into a string.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static Task<string> ReadStringAsync(this Stream stream)
        {
            if (stream == null) { return null; }

            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEndAsync();
            }
        }

        /// <summary>
        /// Gets the <see cref="FilesConfig"/> object.
        /// </summary>
        /// <param name="owinContext">The owin context.</param>
        /// <returns></returns>
        public static FilesConfig GetFilesConfig(this OwinContext owinContext)
        {
            if (owinContext == null) { return null; }

            object config;
            owinContext.Environment.TryGetValue(FilesConfig.OwinKey, out config);
            return config as FilesConfig;
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


        //internal static string PrettyXml(this string xml)
        //{
        //    string formattedXml = XElement.Parse(xml).ToString();
        //    return formattedXml;
        //}

        /// <summary>
        /// Formats the byte file size into strings like N KB.
        /// </summary>
        /// <param name="fileSize">Size of the file.</param>
        /// <returns></returns>
        public static string Humanize(this long fileSize)
        {
            var format = "{0:0.##} B";
            if (fileSize > 1024)
            {
                fileSize /= 1024;
                format = "{0:0.##} KB";

                if (fileSize > 1024)
                {
                    fileSize /= 1024;
                    format = "{0:0.##} MB";

                    if (fileSize > 1024)
                    {
                        fileSize /= 1024;
                        format = "{0:0.##} GB";

                        if (fileSize > 1024)
                        {
                            fileSize /= 1024;
                            format = "{0:0.##} TB";
                        }
                    }
                }
            }

            return string.Format(CultureInfo.InvariantCulture, format, fileSize);
        }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Not an actual problem.")]
        //internal static byte[] Serialize(this XmlDocument xmlDoc)
        //{
        //    using (var ms = new MemoryStream())
        //    using (var writer = new StreamWriter(ms))
        //    {
        //        xmlDoc.Save(writer);
        //        return ms.ToArray();
        //    }
        //}
    }
}