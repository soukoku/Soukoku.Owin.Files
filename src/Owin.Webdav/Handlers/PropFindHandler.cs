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
        public async Task<StatusCode> HandleAsync(DavContext context, ResourceResponse resource)
        {
            int maxDepth = context.Depth;
            context.Config.Log.LogDebug("Depth={0}", maxDepth);
            if (maxDepth == int.MaxValue && !context.Config.AllowInfiniteDepth)
            {
                // TODO: correct?
                return StatusCode.BadRequest;
            }

            if (resource.Resource == null)
            {
                return StatusCode.NotFound;
            }
            
            bool nameOnly = false;
            var filter = new List<PropertyFilter>();
            
            var reqXml = context.ReadRequestAsXml();
            if (reqXml != null)
            {
                context.Config.Log.LogDebug("Request:{0}{1}", Environment.NewLine, reqXml.OuterXml.PrettyXml());

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
                                case ElementNames.Prop:
                                    foreach (XmlNode propNode in node.ChildNodes)
                                    {
                                        filter.Add(new PropertyFilter { Name = propNode.Name, XmlNamespace = propNode.NamespaceURI });
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


            var list = new List<ResourceResponse>();
            WalkResourceTree(context, maxDepth, 0, list, resource);

            return await WriteMultiStatusReponse(context, list, nameOnly, filter);
        }


        private async Task<StatusCode> WriteMultiStatusReponse(DavContext context, List<ResourceResponse> list, bool nameOnly, List<PropertyFilter> filter)
        {
            XmlDocument xmlDoc = XmlGenerator.CreateMultiStatus(context, list, nameOnly, filter);

            ////context.Response.Headers.Append("Cache-Control", "private");
            var content = xmlDoc.Serialize();
            context.Response.Headers.ContentType = MimeTypeMap.GetMimeType(".xml");
            context.Response.Headers.ContentLength = content.Length;

            context.Config.Log.LogDebug("Response:{0}{1}", Environment.NewLine, Encoding.UTF8.GetString(content));
            await context.Response.Body.WriteAsync(content, 0, content.Length, context.CancellationToken);
            return StatusCode.MultiStatus;
        }

        private void WalkResourceTree(DavContext context, int maxDepth, int curDepth, List<ResourceResponse> addToList, ResourceResponse resource)
        {
            addToList.Add(resource);
            if (resource.Resource.ResourceType == ResourceType.Collection && curDepth < maxDepth)
            {
                foreach (var subR in context.Config.DataStore.GetSubResources(context, resource.Resource))
                {
                    WalkResourceTree(context, maxDepth, curDepth + 1, addToList, subR);
                }
            }
        }
    }
}
