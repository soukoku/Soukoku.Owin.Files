using Microsoft.Owin;
using MimeTypes;
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

        private async Task WriteMultiStatusReponse(IOwinContext context, List<Resource> list)
        {
            XmlDocument xmlDoc = new XmlDocument();


            XmlNode rootNode = xmlDoc.CreateElement(WebdavConsts.Xml.ResponseList, WebdavConsts.Xml.Namespace);
            xmlDoc.AppendChild(rootNode);
            
            foreach (var resource in list)
            {
                XmlNode response = xmlDoc.CreateElement(WebdavConsts.Xml.Response, WebdavConsts.Xml.Namespace);
                rootNode.AppendChild(response);

                XmlNode respHref = xmlDoc.CreateElement(WebdavConsts.Xml.RespHref, WebdavConsts.Xml.Namespace);
                respHref.InnerText = Uri.EscapeUriString(resource.Url);
                response.AppendChild(respHref);

                XmlNode respProperty = xmlDoc.CreateElement(WebdavConsts.Xml.RespProperty, WebdavConsts.Xml.Namespace);
                response.AppendChild(respProperty);

                XmlNode propStatus = xmlDoc.CreateElement(WebdavConsts.Xml.PropertyStatus, WebdavConsts.Xml.Namespace);
                // todo: use real status code
                propStatus.InnerText = HttpStatusCode.OK.GenerateStatusMessage();
                respProperty.AppendChild(propStatus);


                XmlNode propList = xmlDoc.CreateElement(WebdavConsts.Xml.PropertyList, WebdavConsts.Xml.Namespace);
                respProperty.AppendChild(propList);

                #region dav-properties

                XmlNode nameNode = xmlDoc.CreateElement(WebdavConsts.Xml.PropDisplayName, WebdavConsts.Xml.Namespace);
                nameNode.InnerText = Path.GetFileName(resource.Url.Trim('/')); // must be actual url part name event if root of dav store
                propList.AppendChild(nameNode);

                XmlNode clNode = xmlDoc.CreateElement(WebdavConsts.Xml.PropGetContentLength, WebdavConsts.Xml.Namespace);
                clNode.InnerText = resource.Length.ToString();
                propList.AppendChild(clNode);

                XmlNode ctNode = xmlDoc.CreateElement(WebdavConsts.Xml.PropGetContentType, WebdavConsts.Xml.Namespace);
                if (!string.IsNullOrEmpty(resource.ContentType))
                {
                    ctNode.InnerText = resource.ContentType;
                }
                propList.AppendChild(ctNode);

                XmlNode createNode = xmlDoc.CreateElement(WebdavConsts.Xml.PropCreationDate, WebdavConsts.Xml.Namespace);
                createNode.InnerText = XmlConvert.ToString(resource.CreateDate, XmlDateTimeSerializationMode.Utc); // rfc 3339?
                propList.AppendChild(createNode);

                XmlNode modNode = xmlDoc.CreateElement(WebdavConsts.Xml.PropGetLastModified, WebdavConsts.Xml.Namespace);
                modNode.InnerText = resource.ModifyDate.ToString("r"); // RFC1123
                propList.AppendChild(modNode);
                
                XmlNode resTypeNode = xmlDoc.CreateElement(WebdavConsts.Xml.PropResourceType, WebdavConsts.Xml.Namespace);
                if (resource.Type == Resource.ResourceType.Folder)
                {
                    resTypeNode.AppendChild(xmlDoc.CreateElement("collection", WebdavConsts.Xml.Namespace));
                }
                propList.AppendChild(resTypeNode);

                XmlNode lockNode = xmlDoc.CreateElement(WebdavConsts.Xml.PropSupportedLock, WebdavConsts.Xml.Namespace);
                propList.AppendChild(lockNode);

                #endregion

                // custom properties
                foreach (var prop in resource.CustomProperties.Values)
                {
                    XmlNode propNode = prop.SerializeElement(xmlDoc);
                    if (propNode != null)
                    {
                        propList.AppendChild(propNode);
                    }
                }
            }


            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms))
            {
                xmlDoc.Save(writer);
                ////context.Response.Headers.Append("Cache-Control", "private");
                context.Response.ContentType = MimeTypeMap.GetMimeType(".xml");
                var content = ms.ToArray();
                context.Response.StatusCode = 207;
                context.Response.ContentLength = content.Length;
                Console.WriteLine(Encoding.UTF8.GetString(content));
                await context.Response.WriteAsync(content);
            }

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
            if (!string.IsNullOrEmpty(resource.LogicalPath))
            {
                rows.Append("<tr><td><span class=\"glyphicon glyphicon-arrow-up\"></span><a href=\"..\">Up</a></td></tr>");
            }
            foreach (var item in _options.DataStore.GetSubResources(context, resource).OrderBy(r => r.Type).ThenBy(r => r.Name))
            {
                rows.Append("<tr>");
                if (item.Type == Resource.ResourceType.Folder)
                {
                    rows.AppendFormat(string.Format("<td><span class=\"glyphicon glyphicon-folder-close\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(item.Url)), WebUtility.HtmlEncode(item.Name)));
                }
                else
                {
                    rows.AppendFormat(string.Format("<td><span class=\"glyphicon glyphicon-file\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(item.Url)), WebUtility.HtmlEncode(item.Name)));
                }
                rows.Append("</tr>");
            }

            var title = string.IsNullOrEmpty(resource.Name) ? "[root]" : WebUtility.HtmlEncode(resource.Name);
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
