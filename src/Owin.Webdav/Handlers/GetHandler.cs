﻿using Microsoft.Owin;
using Soukoku.Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace Soukoku.Owin.Webdav.Handlers
{
    sealed class GetHandler : IMethodHandler
    {
        private WebdavConfig _options;

        public GetHandler(WebdavConfig options)
        {
            _options = options;
        }

        public async Task<bool> HandleAsync(IOwinContext context, IResource resource)
        {
            if (resource != null)
            {
                if (resource.Type == ResourceType.Collection)
                {
                    if (_options.AllowDirectoryBrowsing)
                    {
                        await ShowDirectoryListingAsync(context, resource);
                    }
                }
                else if (resource.Type == ResourceType.Resource)
                {
                    await SendFileAsync(context, resource);
                }
                return true;
            }
            return false;
        }


        async Task ShowDirectoryListingAsync(IOwinContext context, IResource resource)
        {
            context.Response.ContentType = MimeTypeMap.GetMimeType(".html");

            // there's a better way for templating but I don't know it yet.
            var rows = new StringBuilder();
            foreach (var item in _options.DataStore.GetSubResources(context, resource).OrderByDescending(r => r.Type).ThenBy(r => r.DisplayName))
            {
                rows.Append("<tr>");
                var url = context.GenerateUrl(item);
                if (item.Type == ResourceType.Collection)
                {
                    rows.AppendFormat(string.Format(CultureInfo.InvariantCulture, "<td>{2}</td><td></td><td><span class=\"text-warning glyphicon glyphicon-folder-close\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(url)), WebUtility.HtmlEncode(item.DisplayName), item.ModifiedDateUtc.ToString("yyyy/MM/dd hh:mm tt")));
                }
                else
                {
                    rows.AppendFormat(string.Format(CultureInfo.InvariantCulture, "<td>{3}</td><td class=\"text-right\">{2}</td><td><span class=\"text-info glyphicon glyphicon-file\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(url)), WebUtility.HtmlEncode(item.DisplayName), item.Length.PrettySize(), item.ModifiedDateUtc.ToString("yyyy/MM/dd hh:mm tt")));
                }
                rows.Append("</tr>");
            }

            var title = WebUtility.HtmlEncode(context.Request.Uri.AbsolutePath);
            var content = string.Format(CultureInfo.InvariantCulture, await GetDirectoryListingTemplateAsync(), title, rows);
            await context.Response.WriteAsync(content);
        }

        static async Task SendFileAsync(IOwinContext context, IResource resource)
        {
            if (resource.Length > 0)
            {
                context.Response.ContentLength = resource.Length;
            }
            context.Response.ContentType = resource.ContentType;
            context.Response.Headers.Append("Content-Disposition", "inline; filename=" + Uri.EscapeUriString(resource.DisplayName));

            using (Stream fs = resource.OpenReadStream())
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
