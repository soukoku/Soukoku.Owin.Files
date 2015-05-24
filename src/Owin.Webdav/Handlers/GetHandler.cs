using Soukoku.Owin.Webdav.Models;
using Soukoku.Owin.Webdav.Responses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Soukoku.Owin.Webdav.DavConsts;

namespace Soukoku.Owin.Webdav.Handlers
{
    sealed class GetHandler : IMethodHandler
    {
        public async Task<StatusCode> HandleAsync(DavContext context, ResourceResponse resource)
        {
            if (resource.Resource != null)
            {
                var headOnly = string.Equals(context.Request.Method, Methods.Head, StringComparison.OrdinalIgnoreCase);

                if (resource.Resource.ResourceType == ResourceType.Collection)
                {
                    if (context.Config.AllowDirectoryBrowsing)
                    {
                        await ShowDirectoryListingAsync(context, resource.Resource, headOnly);
                    }
                }
                else if (resource.Resource.ResourceType == ResourceType.Resource)
                {
                    await SendFileAsync(context, resource.Resource, headOnly);
                }
                return StatusCode.OK;
            }
            return StatusCode.NotFound;
        }


        async Task ShowDirectoryListingAsync(DavContext context, IResource resource, bool headOnly)
        {
            context.Response.Headers.ContentType = MimeTypeMap.GetMimeType(".html");
            if (!headOnly)
            {
                var subRes = context.Config.DataStore.GetSubResources(context, resource);
                var content = await context.Config.DirectoryGenerator.GenerateAsync(context, resource, subRes.Select(r => r.Resource));
                await context.Response.WriteAsync(content, context.CancellationToken);
            }
        }

        static async Task SendFileAsync(DavContext context, IResource resource, bool headOnly)
        {
            if (resource.Length > 0)
            {
                context.Response.Headers.ContentLength = resource.Length;
            }
            context.Response.Headers.ContentType = resource.ContentType;
            context.Response.Headers.Append("Content-Disposition", "inline; filename=" + Uri.EscapeUriString(resource.DisplayName));

            if (!headOnly)
            {
                using (Stream fs = context.Config.DataStore.Read(resource))
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
}
