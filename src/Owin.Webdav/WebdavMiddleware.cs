using Microsoft.Owin;
using MimeTypes;
using Owin.Webdav.Models;
using Owin.Webdav.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

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
            var logicalPath = Uri.UnescapeDataString(context.Request.Uri.AbsolutePath.Substring(context.Request.PathBase.Value.Length));
            if (logicalPath.StartsWith("/")) { logicalPath = logicalPath.Substring(1); }

            Console.WriteLine("DAV received {0} for [{1}]", context.Request.Method, logicalPath);

            Resource resource = _options.DataStore.GetResource(context, logicalPath);
            if (resource != null)
            {
                context.Response.Headers.Append("MS-Author-Via", "DAV");
                switch (context.Request.Method.ToUpperInvariant())
                {
                    case WebdavConsts.Methods.Options:
                        return HandleOptions(context);
                    case WebdavConsts.Methods.PropFind:
                        return HandlePropFindAsync(context, resource);
                    case WebdavConsts.Methods.Get:
                        return HandleGetAsync(context, resource);
                }
            }
            return Next.Invoke(context);
        }

        private Task HandleOptions(IOwinContext context)
        {
            // lie and say we can deal with it all for now

            context.Response.Headers.AppendCommaSeparatedValues("DAV", "1");//, "2");
            context.Response.Headers.AppendCommaSeparatedValues("Allow",
                WebdavConsts.Methods.Options,
                WebdavConsts.Methods.PropFind,
                WebdavConsts.Methods.PropPatch,
                WebdavConsts.Methods.MkCol,
                WebdavConsts.Methods.Copy,
                WebdavConsts.Methods.Move,
                WebdavConsts.Methods.Delete,
                WebdavConsts.Methods.Lock,
                WebdavConsts.Methods.Unlock,
                WebdavConsts.Methods.Get);

            context.Response.Headers.AppendCommaSeparatedValues("Public",
                WebdavConsts.Methods.Options,
                WebdavConsts.Methods.PropFind,
                WebdavConsts.Methods.PropPatch,
                WebdavConsts.Methods.MkCol,
                WebdavConsts.Methods.Copy,
                WebdavConsts.Methods.Move,
                WebdavConsts.Methods.Delete,
                WebdavConsts.Methods.Lock,
                WebdavConsts.Methods.Unlock,
                WebdavConsts.Methods.Get);

            context.Response.ContentLength = 0;
            return Task.FromResult(0);
        }

        private async Task HandlePropFindAsync(IOwinContext context, Resource resource)
        {
            int maxDepth = context.GetDepth();
            if (maxDepth == int.MaxValue && !_options.AllowInfiniteDepth)
            {
                // TODO: return dav error
            }

            Console.WriteLine("Depth=" + maxDepth);

            // TODO: support client request instead of always returning fixed property list
            var reqBody = await context.ReadRequestStringAsync();
            if (!string.IsNullOrEmpty(reqBody))
            {
                Console.WriteLine(reqBody.PrettyXml());

                XmlDocument reqXml = new XmlDocument();
                reqXml.LoadXml(reqBody);
            }

            List<Resource> list = new List<Resource>();
            var curDepth = 0;
            WalkResourceTree(context, maxDepth, curDepth, list, resource);

            await WriteMultiStatusReponse(context, list);
        }

        private Task WriteMultiStatusReponse(IOwinContext context, List<Resource> list)
        {
            XmlDocument xmlDoc = MultiStatusResponse.Create(list);

            ////context.Response.Headers.Append("Cache-Control", "private");
            var content = xmlDoc.Serialize();
            context.Response.ContentType = MimeTypeMap.GetMimeType(".xml");
            context.Response.StatusCode = 207;
            context.Response.ContentLength = content.Length;
            Console.WriteLine(Encoding.UTF8.GetString(content));
            return context.Response.WriteAsync(content);
        }

        private async Task HandleGetAsync(IOwinContext context, Resource resource)
        {
            if (resource.Type == Resource.ResourceType.Folder)
            {
                if (_options.AllowDirectoryBrowsing)
                {
                    await ShowDirectoryListingAsync(context, resource);
                }
            }
            else if (resource.Type == Resource.ResourceType.File)
            {
                await SendFileAsync(context, resource);
            }
        }

        #region utilities

        private void WalkResourceTree(IOwinContext context, int maxDepth, int curDepth, List<Resource> addToList, Resource resource)
        {
            addToList.Add(resource);
            if (resource.Type == Resource.ResourceType.Folder && curDepth < maxDepth)
            {
                foreach (var subR in _options.DataStore.GetSubResources(context, resource))
                {
                    WalkResourceTree(context, maxDepth, curDepth + 1, addToList, subR);
                }
            }
        }

        async Task ShowDirectoryListingAsync(IOwinContext context, Resource resource)
        {
            context.Response.ContentType = MimeTypeMap.GetMimeType(".html");

            // there's a better way for templating but I don't know it yet.
            var rows = new StringBuilder();
            foreach (var item in _options.DataStore.GetSubResources(context, resource).OrderBy(r => r.Type).ThenBy(r => r.Name))
            {
                rows.Append("<tr>");
                if (item.Type == Resource.ResourceType.Folder)
                {
                    rows.AppendFormat(string.Format("<td>{2}</td><td></td><td><span class=\"glyphicon glyphicon-folder-close\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(item.Url)), WebUtility.HtmlEncode(item.Name), item.ModifyDate.ToString("yyyy/MM/dd hh:mm tt")));
                }
                else
                {
                    rows.AppendFormat(string.Format("<td>{3}</td><td class=\"text-right\">{2}</td><td><span class=\"glyphicon glyphicon-file\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(item.Url)), WebUtility.HtmlEncode(item.Name), item.Length.PrettySize(), item.ModifyDate.ToString("yyyy/MM/dd hh:mm tt")));
                }
                rows.Append("</tr>");
            }

            var title = WebUtility.HtmlEncode(context.Request.Uri.AbsolutePath);// string.IsNullOrEmpty(resource.Name) ? "[root]" : WebUtility.HtmlEncode(resource.Name);
            var content = string.Format(await GetDirectoryListingTemplateAsync(), title, rows);
            //context.Response.ContentLength = content.Length;
            await context.Response.WriteAsync(content);
        }

        static async Task SendFileAsync(IOwinContext context, Resource resource)
        {
            if (resource.Length > 0)
            {
                context.Response.ContentLength = resource.Length;
            }
            context.Response.ContentType = MimeTypeMap.GetMimeType(Path.GetExtension(resource.LogicalPath));
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
            return await Assembly.GetExecutingAssembly().GetManifestResourceStream("Owin.Webdav.Responses.DirectoryListing.html").ReadStringAsync();
        }

        #endregion
    }
}
