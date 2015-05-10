using Microsoft.Owin;
using Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Owin.Webdav
{
    public class WebdavMiddleware : OwinMiddleware
    {
        readonly WebdavConfig _options;

        public WebdavMiddleware(OwinMiddleware next, WebdavConfig options) : base(next)
        {
            if (options == null) { throw new ArgumentException("options"); }

            _options = options;
        }


        public override Task Invoke(IOwinContext context)
        {
            var path = Uri.UnescapeDataString(context.Request.Uri.AbsolutePath);
            Resource resource = _options.DataStore.GetResource(path);
            if (resource != null)
            {
                switch (context.Request.Method.ToUpperInvariant())
                {
                    case "OPTIONS":
                        return HandleOptions(context);
                    case "PROPFIND":
                        return HandlePropFindAsync(context, resource);
                    case "GET":
                        return HandleGetAsync(context, resource);
                }
            }
            return Next.Invoke(context);
        }

        private Task HandleOptions(IOwinContext context)
        {
            // lie and say we can deal with it all for now

            context.Response.Headers.Append("DAV", "1, 2");
            context.Response.Headers.Append("Allow", "OPTIONS, PROPFIND, PROPPATCH, COPY, MOVE, DELETE, MKCOL, LOCK, UNLOCK");

            return Task.FromResult(0);
        }

        private Task HandlePropFindAsync(IOwinContext context, Resource resource)
        {
            int depth = context.GetDepth();
            if (!_options.AllowUnlimitedDepth)
            {
                // TODO: return dav error
            }

            // TODO: client request is ignored for now
            //List<Resource> retVal = new List<Resource>();


            return Task.FromResult(0);
        }

        private async Task HandleGetAsync(IOwinContext context, Resource resource)
        {
            if (resource.Type == ResourceType.Folder)
            {
                if (_options.AllowGetDirectoryBrowsing)
                {
                    await ShowDirectoryListingAsync(context, resource);
                }
            }
            else if (resource.Type == ResourceType.File)
            {
                await SendFileAsync(context, resource);
            }
        }

        #region utilities

        async Task ShowDirectoryListingAsync(IOwinContext context, Resource resource)
        {
            context.Response.ContentType = "text/html";

            // there's a better way for templating but I don't know it yet.
            var rows = new StringBuilder();
            if (!string.IsNullOrEmpty(resource.Name))
            {
                rows.Append("<tr><td><span class=\"glyphicon glyphicon-arrow-up\"></span><a href=\"..\">Up</a></td></tr>");
            }
            foreach (var item in _options.DataStore.GetSubResources(resource).OrderBy(r => r.Type).ThenBy(r => r.Name))
            {
                rows.Append("<tr>");
                if (item.Type == ResourceType.Folder)
                {
                    rows.AppendFormat(string.Format("<td><span class=\"glyphicon glyphicon-folder-close\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(item.LogicalPath)), WebUtility.HtmlEncode(item.Name)));
                }
                else
                {
                    rows.AppendFormat(string.Format("<td><span class=\"glyphicon glyphicon-file\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(item.LogicalPath)), WebUtility.HtmlEncode(item.Name)));
                }
                rows.Append("</tr>");
            }

            var title = string.IsNullOrEmpty(resource.Name) ? "[root]" : WebUtility.HtmlEncode(resource.Name);
            var content = string.Format(await GetDirectoryListingTemplateAsync(), title, rows);
            await context.Response.WriteAsync(content);
        }

        static async Task SendFileAsync(IOwinContext context, Resource resource)
        {
            if (resource.Length > 0)
            {
                context.Response.ContentLength = resource.Length;
            }
            context.Response.ContentType = MimeTypes.MimeTypeMap.GetMimeType(Path.GetExtension(resource.LogicalPath));
            context.Response.Headers.Append("Content-Disposition", "inline; filename=" + Uri.EscapeUriString(resource.Name));

            using (Stream fs = resource.GetReadStream())
            {
                byte[] buff = new byte[4096];
                int read = 0;
                var ctk = context.GetCancellationToken();

                while ((read = await fs.ReadAsync(buff, 0, buff.Length)) > 0)
                {
                    await context.Response.WriteAsync(buff, 0, read, ctk);
                }
            }
        }

        static async Task<string> GetDirectoryListingTemplateAsync()
        {
            using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Owin.Webdav.Responses.DirectoryListing.html")))
            {
                return await reader.ReadToEndAsync();
            }
        }

        #endregion
    }
}
