using Microsoft.Owin;
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
using System.Globalization;

using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace Soukoku.Owin.Webdav
{
    class WebdavMiddleware
    {
        readonly WebdavConfig _options;
        readonly AppFunc _next;
        
        public WebdavMiddleware(AppFunc next, WebdavConfig options)
        {
            if (next == null) { throw new ArgumentNullException("next"); }
            if (options == null) { throw new ArgumentNullException("options"); }

            _next = next;
            _options = options;
        }


        public Task Invoke(IDictionary<string, object> environment)
        {
            var context = new OwinContext(environment);

            try
            {
                var logicalPath = Uri.UnescapeDataString(context.Request.Uri.AbsolutePath.Substring(context.Request.PathBase.Value.Length));
                if (logicalPath.StartsWith("/", StringComparison.Ordinal)) { logicalPath = logicalPath.Substring(1); }


                IResource resource = _options.DataStore.GetResource(context, logicalPath);
                if (resource != null)
                {
                    var fullUrl = context.Request.Uri.ToString();
                    _options.Log.LogDebug("{0} for {1}@{2}", context.Request.Method, resource.Type, resource.LogicalPath);
                    if (resource.Type == ResourceType.Collection && !fullUrl.EndsWith("/", StringComparison.Ordinal))
                    {
                        context.Response.Headers.Append("Content-Location", fullUrl + "/");
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
            catch (Exception ex)
            {
                _options.Log.LogError(ex.ToString());
                //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //return context.Response.WriteAsync(ex.Message);
                throw;
            }
        }

        #region http method handling

        static Task HandleOptions(IOwinContext context)
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

        private async Task HandlePropFindAsync(IOwinContext context, IResource resource)
        {
            int maxDepth = context.GetDepth();
            if (maxDepth == int.MaxValue && !_options.AllowInfiniteDepth)
            {
                // TODO: return dav error
            }

            _options.Log.LogDebug("Depth={0}", maxDepth);

            // TODO: support client request instead of always returning fixed property list
            var reqBody = await context.ReadRequestStringAsync();
            if (!string.IsNullOrEmpty(reqBody))
            {
                _options.Log.LogDebug("Request:{0}{1}", Environment.NewLine, reqBody.PrettyXml());

                XmlDocument reqXml = new XmlDocument();
                reqXml.LoadXml(reqBody);
            }

            var list = new List<IResource>();
            var curDepth = 0;
            WalkResourceTree(context, maxDepth, curDepth, list, resource);

            await WriteMultiStatusReponse(context, list);
        }

        private Task HandleGetAsync(IOwinContext context, IResource resource)
        {
            if (resource.Type == ResourceType.Collection)
            {
                if (_options.AllowDirectoryBrowsing)
                {
                    return ShowDirectoryListingAsync(context, resource);
                }
            }
            else if (resource.Type == ResourceType.Resource)
            {
                return SendFileAsync(context, resource);
            }
            return Task.FromResult(0);
        }

        #endregion

        #region utilities

        private Task WriteMultiStatusReponse(IOwinContext context, List<IResource> list)
        {
            XmlDocument xmlDoc = XmlGenerator.CreateMultiStatus(context, list);

            ////context.Response.Headers.Append("Cache-Control", "private");
            var content = xmlDoc.Serialize();
            context.Response.ContentType = MimeTypeMap.GetMimeType(".xml");
            context.Response.StatusCode = (int)Consts.StatusCode.MultiStatus;
            context.Response.ContentLength = content.Length;

            _options.Log.LogDebug("Response:{0}{1}", Environment.NewLine, Encoding.UTF8.GetString(content));
            return context.Response.WriteAsync(content);
        }

        private void WalkResourceTree(IOwinContext context, int maxDepth, int curDepth, List<IResource> addToList, IResource resource)
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

        static async Task<string> GetDirectoryListingTemplateAsync()
        {
            return await typeof(WebdavMiddleware).Assembly.GetManifestResourceStream("Soukoku.Owin.Webdav.Responses.DirectoryListing.html").ReadStringAsync();
        }

        #endregion
    }
}
