using Microsoft.Owin;
using MimeTypes;
using Soukoku.Owin.Webdav.Models;
using Soukoku.Owin.Webdav.Responses;
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
using Owin;

namespace Soukoku.Owin.Webdav
{
    public class WebdavMiddleware
    {
        readonly WebdavConfig _options;
        readonly Func<IDictionary<string, object>, Task> _next;

        public WebdavMiddleware(Func<IDictionary<string, object>, Task> next, WebdavConfig options)
        {
            if (next == null) { throw new ArgumentException("next"); }
            if (options == null) { throw new ArgumentException("options"); }

            _next = next;
            _options = options;
        }


        public Task Invoke(IDictionary<string, object> environment)
        {
            var context = new OwinContext(environment);

            var logicalPath = Uri.UnescapeDataString(context.Request.Uri.AbsolutePath.Substring(context.Request.PathBase.Value.Length));
            if (logicalPath.StartsWith("/")) { logicalPath = logicalPath.Substring(1); }

            _options.Log.Debug("DAV received {0} for [{1}]", context.Request.Method, logicalPath);

            Resource resource = _options.DataStore.GetResource(context, logicalPath);
            if (resource != null)
            {
                if (resource.Type == ResourceType.Collection && !context.Request.Uri.ToString().EndsWith("/"))
                {
                    context.Response.Headers.Append("Content-Location", context.Request.Uri.ToString() + "/");
                }
                switch (context.Request.Method.ToUpperInvariant())
                {
                    case Consts.Method.Options:
                        return HandleOptions(context);
                    case Consts.Method.PropFind:
                        return HandlePropFindAsync(context, resource);
                    case Consts.Method.Get:
                        return HandleGetAsync(context, resource);
                }
            }
            return _next.Invoke(environment);
        }

        #region http method handling

        private Task HandleOptions(IOwinContext context)
        {
            // lie and say we can deal with it all for now

            context.Response.Headers.Append("MS-Author-Via", "DAV");
            context.Response.Headers.AppendCommaSeparatedValues(Consts.Header.Dav, "1", "2", "3");
            context.Response.Headers.AppendCommaSeparatedValues("Allow",
                Consts.Method.Options,
                Consts.Method.PropFind,
                Consts.Method.PropPatch,
                Consts.Method.MkCol,
                Consts.Method.Copy,
                Consts.Method.Move,
                Consts.Method.Delete,
                Consts.Method.Lock,
                Consts.Method.Unlock,
                Consts.Method.Get);

            context.Response.Headers.AppendCommaSeparatedValues("Public",
                Consts.Method.Options,
                Consts.Method.PropFind,
                Consts.Method.PropPatch,
                Consts.Method.MkCol,
                Consts.Method.Copy,
                Consts.Method.Move,
                Consts.Method.Delete,
                Consts.Method.Lock,
                Consts.Method.Unlock,
                Consts.Method.Get);

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

            _options.Log.Debug("Depth={0}", maxDepth);

            // TODO: support client request instead of always returning fixed property list
            var reqBody = await context.ReadRequestStringAsync();
            if (!string.IsNullOrEmpty(reqBody))
            {
                _options.Log.Debug("Request:{0}{1}", Environment.NewLine, reqBody.PrettyXml());

                XmlDocument reqXml = new XmlDocument();
                reqXml.LoadXml(reqBody);
            }

            List<Resource> list = new List<Resource>();
            var curDepth = 0;
            WalkResourceTree(context, maxDepth, curDepth, list, resource);

            await WriteMultiStatusReponse(context, list);
        }

        private async Task HandleGetAsync(IOwinContext context, Resource resource)
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
        }

        #endregion

        #region utilities

        private Task WriteMultiStatusReponse(IOwinContext context, List<Resource> list)
        {
            XmlDocument xmlDoc = MultiStatusResponse.Create(list);

            ////context.Response.Headers.Append("Cache-Control", "private");
            var content = xmlDoc.Serialize();
            context.Response.ContentType = MimeTypeMap.GetMimeType(".xml");
            context.Response.StatusCode = (int)Consts.StatusCode.MultiStatus;
            context.Response.ContentLength = content.Length;

            _options.Log.Debug("Response:{0}{1}", Environment.NewLine, Encoding.UTF8.GetString(content));
            return context.Response.WriteAsync(content);
        }

        private void WalkResourceTree(IOwinContext context, int maxDepth, int curDepth, List<Resource> addToList, Resource resource)
        {
            addToList.Add(resource);
            if (resource.Type == ResourceType.Collection && curDepth < maxDepth)
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
            foreach (var item in _options.DataStore.GetSubResources(context, resource).OrderBy(r => r.Type).ThenBy(r => r.DisplayName.Value))
            {
                rows.Append("<tr>");
                if (item.Type == ResourceType.Collection)
                {
                    rows.AppendFormat(string.Format("<td>{2}</td><td></td><td><span class=\"glyphicon glyphicon-folder-close\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(item.Url)), WebUtility.HtmlEncode(item.DisplayName.Value), item.ModifyDate.Value.ToString("yyyy/MM/dd hh:mm tt")));
                }
                else
                {
                    rows.AppendFormat(string.Format("<td>{3}</td><td class=\"text-right\">{2}</td><td><span class=\"glyphicon glyphicon-file\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(item.Url)), WebUtility.HtmlEncode(item.DisplayName.Value), item.Length.Value.PrettySize(), item.ModifyDate.Value.ToString("yyyy/MM/dd hh:mm tt")));
                }
                rows.Append("</tr>");
            }

            var title = WebUtility.HtmlEncode(context.Request.Uri.AbsolutePath);
            var content = string.Format(await GetDirectoryListingTemplateAsync(), title, rows);
            await context.Response.WriteAsync(content);
        }

        static async Task SendFileAsync(IOwinContext context, Resource resource)
        {
            if (resource.Length.Value > 0)
            {
                context.Response.ContentLength = resource.Length.Value;
            }
            context.Response.ContentType = resource.ContentType.Value;
            context.Response.Headers.Append("Content-Disposition", "inline; filename=" + Uri.EscapeUriString(resource.DisplayName.Value));

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
            return await typeof(WebdavMiddleware).Assembly.GetManifestResourceStream("Soukoku.Owin.Webdav.Responses.DirectoryListing.html").ReadStringAsync();
        }

        #endregion
    }
}
