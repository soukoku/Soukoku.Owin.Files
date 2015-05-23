﻿using Soukoku.Owin.Webdav.Models;
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
            var subRes = _options.DataStore.GetSubResources(context.Request.PathBase, resource);

            var content = await _options.DirectoryGenerator.GenerateAsync(context, resource, subRes);

            context.Response.Headers.ContentType = MimeTypeMap.GetMimeType(".html");

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
    }
}