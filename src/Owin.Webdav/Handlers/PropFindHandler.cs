using Soukoku.Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Soukoku.Owin.Webdav.Responses;
using static Soukoku.Owin.Webdav.DavConsts;

namespace Soukoku.Owin.Webdav.Handlers
{
    sealed class PropFindHandler : IMethodHandler
    {
        private WebdavConfig _options;

        public PropFindHandler(WebdavConfig options)
        {
            _options = options;
        }

        public async Task<StatusCode> HandleAsync(Context context, IResource resource)
        {
            int maxDepth = context.GetDepth();
            _options.Log.LogDebug("Depth={0}", maxDepth);
            if (maxDepth == int.MaxValue && !_options.AllowInfiniteDepth)
            {
                // TODO: correct?
                return StatusCode.BadRequest;
            }

            if (resource == null)
            {
                return StatusCode.NotFound;
            }

            bool allProperties = true;
            bool nameOnly = false;
            var filter = new List<Tuple<string, string>>(); // item1 = name, item2 = namespace
            
            var reqXml = context.Request.ReadRequestAsXml();
            if (reqXml != null)
            {
                _options.Log.LogDebug("Request:{0}{1}", Environment.NewLine, reqXml.OuterXml.PrettyXml());

                if (reqXml.DocumentElement.Name == ElementNames.PropFind &&
                    reqXml.DocumentElement.NamespaceURI == XmlNamespace)
                {
                    foreach (XmlNode node in reqXml.DocumentElement.ChildNodes)
                    {
                        if (node.NamespaceURI == XmlNamespace)
                        {
                            switch (node.Name)
                            {
                                case ElementNames.PropName:
                                    nameOnly = true;
                                    break;
                                case ElementNames.AllProp:
                                    allProperties = true;
                                    break;
                                case ElementNames.Prop:
                                    allProperties = false;
                                    foreach (XmlNode propNode in node.ChildNodes)
                                    {
                                        filter.Add(new Tuple<string, string>(propNode.Name, propNode.NamespaceURI));
                                    }
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    return StatusCode.BadRequest;
                }
            }


            var list = new List<IResource>();
            WalkResourceTree(context, maxDepth, 0, list, resource);

            return await WriteMultiStatusReponse(context, list, allProperties, nameOnly, filter);
        }


        private async Task<StatusCode> WriteMultiStatusReponse(Context context, List<IResource> list, bool allProperties, bool nameOnly, List<Tuple<string, string>> filter)
        {
            XmlDocument xmlDoc = XmlGenerator.CreateMultiStatus(context, list, allProperties, nameOnly, filter);

            ////context.Response.Headers.Append("Cache-Control", "private");
            var content = xmlDoc.Serialize();
            context.Response.Headers.ContentType = MimeTypeMap.GetMimeType(".xml");
            context.Response.Headers.ContentLength = content.Length;

            _options.Log.LogDebug("Response:{0}{1}", Environment.NewLine, Encoding.UTF8.GetString(content));
            await context.Response.Body.WriteAsync(content, 0, content.Length, context.CancellationToken);
            return StatusCode.MultiStatus;
        }

        private void WalkResourceTree(Context context, int maxDepth, int curDepth, List<IResource> addToList, IResource resource)
        {
            addToList.Add(resource);
            if (resource.ResourceType == ResourceType.Collection && curDepth < maxDepth)
            {
                foreach (var subR in _options.DataStore.GetSubResources(context.Request.PathBase, resource))
                {
                    WalkResourceTree(context, maxDepth, curDepth + 1, addToList, subR);
                }
            }
        }
    }
}
