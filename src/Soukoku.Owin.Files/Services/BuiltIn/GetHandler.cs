using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files.Services.BuiltIn
{
    sealed class GetHandler : IMethodHandler
    {
        public async Task<int> HandleAsync(Resource resource)
        {
            if (resource != null)
            {
                var config = resource.Context.GetFilesConfig();

                var headOnly = string.Equals(resource.Context.Request.Method, HttpMethods.Head, StringComparison.OrdinalIgnoreCase);

                if (resource.IsFolder)
                {
                    if (config.AllowDirectoryBrowsing)
                    {
                        await ShowDirectoryListingAsync(config, resource, headOnly);
                    }
                }
                else
                {
                    await SendFileAsync(config, resource, headOnly);
                }
                return (int)HttpStatusCode.OK;
            }
            return (int)HttpStatusCode.NotFound;
        }


        async Task ShowDirectoryListingAsync(FilesConfig config, Resource resource, bool headOnly)
        {
            resource.Context.Response.Headers.ContentType = config.MimeTypeProvider.GetMimeType(".html");
            if (!headOnly)
            {
                var subRes = config.DataStore.GetSubResources(resource.Context, resource);
                var content = await config.DirectoryGenerator.GenerateAsync(resource.Context, resource, subRes.Select(r => r.Resource));
                await resource.Context.Response.WriteAsync(content, resource.Context.CancellationToken);
            }
        }

        static async Task SendFileAsync(FilesConfig config, Resource resource, bool headOnly)
        {
            if (resource.Length > 0)
            {
                resource.Context.Response.Headers.ContentLength = resource.Length;
            }
            resource.Context.Response.Headers.ContentType = resource.ContentType;
            //resource.Context.Response.Headers.Append("Content-Disposition", "inline; filename=" + Uri.EscapeUriString(resource.DisplayName));

            if (!headOnly)
            {
                using (Stream fs = config.DataStore.Open(resource))
                {
                    byte[] buff = new byte[4096];
                    int read = 0;

                    while ((read = await fs.ReadAsync(buff, 0, buff.Length)) > 0)
                    {
                        await resource.Context.Response.Body.WriteAsync(buff, 0, read, resource.Context.CancellationToken);
                    }
                }
            }
        }
    }
}
