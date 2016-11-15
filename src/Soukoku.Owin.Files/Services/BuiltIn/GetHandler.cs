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
                var context = resource.Context;

                var headOnly = string.Equals(context.Request.Method, HttpMethodNames.Head, StringComparison.OrdinalIgnoreCase);

                if (resource.IsFolder)
                {
                    if (config.AllowDirectoryBrowsing)
                    {
                        await ShowDirectoryListingAsync(config, resource, headOnly).ConfigureAwait(false);
                    }
                    else
                    {
                        if (context.Request.Path.EndsWith("/"))
                        {
                            // try find default docs
                            var defaultFile = config.DataStore.GetSubResources(context, resource)
                                .FirstOrDefault(sf => config.DefaultDocuments.Contains(sf.Resource.DisplayName, StringComparer.OrdinalIgnoreCase));
                            if (defaultFile != null)
                            {
                                await SendFileAsync(config, defaultFile.Resource, headOnly).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            // redirect to path with /
                            context.Response.Headers.Add("Location", new[] { context.Request.PathBase + context.Request.Path + "/" + context.Request.QueryString });
                            return (int)HttpStatusCode.Moved;
                        }
                    }
                }
                else
                {
                    await SendFileAsync(config, resource, headOnly).ConfigureAwait(false);
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
                var content = await config.DirectoryGenerator.GenerateAsync(resource.Context, resource, subRes.Select(r => r.Resource)).ConfigureAwait(false);
                await resource.Context.Response.WriteAsync(content, resource.Context.CancellationToken).ConfigureAwait(false);
            }
        }

        static async Task SendFileAsync(FilesConfig config, Resource resource, bool headOnly)
        {
            if (resource.Length > 0)
            {
                resource.Context.Response.Headers.ContentLength = resource.Length;
            }
            resource.Context.Response.Headers.ContentType = resource.ContentType;
            if (resource.ModifiedDateUtc > DateTime.MinValue)
            {
                resource.Context.Response.Headers.Append(HttpHeaderNames.LastModified, resource.ModifiedDateUtc.ToString("R"));
            }

            //resource.Context.Response.Headers.Append(HttpHeaders.ContentDisposition, "inline; filename=" + Uri.EscapeUriString(resource.DisplayName));

            if (!headOnly)
            {
                using (Stream fs = config.DataStore.Open(resource))
                {
                    byte[] buff = new byte[4096];
                    int read = 0;

                    while ((read = await fs.ReadAsync(buff, 0, buff.Length).ConfigureAwait(false)) > 0)
                    {
                        await resource.Context.Response.Body.WriteAsync(buff, 0, read, resource.Context.CancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
