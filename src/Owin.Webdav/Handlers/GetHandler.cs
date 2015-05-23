using Soukoku.Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav.Handlers
{
    sealed class GetHandler : IMethodHandler
    {
        private WebdavConfig _options;

        public GetHandler(WebdavConfig options)
        {
            _options = options;
        }

        public async Task<bool> HandleAsync(Context context, IResource resource)
        {
            if (resource != null)
            {
                if (resource.ResourceType == ResourceType.Collection)
                {
                    if (_options.AllowDirectoryBrowsing)
                    {
                        await ShowDirectoryListingAsync(context, resource);
                    }
                }
                else if (resource.ResourceType == ResourceType.Resource)
                {
                    await SendFileAsync(context, resource);
                }
                return true;
            }
            return false;
        }


        async Task ShowDirectoryListingAsync(Context context, IResource resource)
        {
            context.Response.Headers.ContentType = MimeTypeMap.GetMimeType(".html");

            // there's a better way for templating but I don't know it yet.
            var rows = new StringBuilder();
            foreach (var item in _options.DataStore.GetSubResources(context.Request.PathBase, resource).OrderByDescending(r => r.ResourceType).ThenBy(r => r.DisplayName))
            {
                rows.Append("<tr>");
                var url = context.GenerateUrl(item);
                if (item.ResourceType == ResourceType.Collection)
                {
                    rows.AppendFormat(string.Format(CultureInfo.InvariantCulture, "<td>{2}</td><td></td><td><span class=\"text-warning glyphicon glyphicon-folder-close\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(url)), WebUtility.HtmlEncode(item.DisplayName), item.ModifiedDateUtc.ToString("yyyy/MM/dd hh:mm tt")));
                }
                else
                {
                    rows.AppendFormat(string.Format(CultureInfo.InvariantCulture, "<td>{3}</td><td class=\"text-right\">{2}</td><td><span class=\"text-info glyphicon glyphicon-file\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(url)), WebUtility.HtmlEncode(item.DisplayName), item.Length.PrettySize(), item.ModifiedDateUtc.ToString("yyyy/MM/dd hh:mm tt")));
                }
                rows.Append("</tr>");
            }
            
            var title = WebUtility.HtmlEncode(context.Request.PathBase + context.Request.Path);
            var content = string.Format(CultureInfo.InvariantCulture, await GetDirectoryListingTemplateAsync(), title, rows);
            await context.Response.WriteAsync(content, context.CancellationToken);
        }

        static async Task SendFileAsync(Context context, IResource resource)
        {
            if (resource.Length > 0)
            {
                context.Response.Headers.ContentLength = resource.Length;
            }
            context.Response.Headers.ContentType = resource.ContentType;
            context.Response.Headers.Append("Content-Disposition", "inline; filename=" + Uri.EscapeUriString(resource.DisplayName));

            using (Stream fs = resource.OpenReadStream())
            {
                byte[] buff = new byte[4096];
                int read = 0;

                while ((read = await fs.ReadAsync(buff, 0, buff.Length)) > 0)
                {
                    await context.Response.Body.WriteAsync(buff, 0, read, context.CancellationToken);
                }
            }
        }

        static string _template;
        static async Task<string> GetDirectoryListingTemplateAsync()
        {
            if (_template == null)
            {
                _template = await typeof(WebdavMiddleware).Assembly.GetManifestResourceStream("Soukoku.Owin.Webdav.Responses.DirectoryListing.html").ReadStringAsync();
            }
            return _template;
        }

    }
}
