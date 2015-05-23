using Microsoft.Owin;
using Soukoku.Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Owin;
using System.Xml;
using Soukoku.Owin.Webdav.Responses;

namespace Soukoku.Owin.Webdav.Handlers
{
    sealed class PropFindHandler : IMethodHandler
    {
        private WebdavConfig _options;

        public PropFindHandler(WebdavConfig options)
        {
            _options = options;
        }

        public async Task<bool> HandleAsync(IOwinContext context, IResource resource)
        {
            if (resource != null)
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

                    var reqXml = new XmlDocument();
                    reqXml.LoadXml(reqBody);
                }

                var list = new List<IResource>();
                var curDepth = 0;
                WalkResourceTree(context, maxDepth, curDepth, list, resource);

                await WriteMultiStatusReponse(context, list);
                return true;
            }
            return false;
        }


        private Task WriteMultiStatusReponse(IOwinContext context, List<IResource> list)
        {
            XmlDocument xmlDoc = XmlGenerator.CreateMultiStatus(context, list);

            ////context.Response.Headers.Append("Cache-Control", "private");
            var content = xmlDoc.Serialize();
            context.Response.ContentType = MimeTypeMap.GetMimeType(".xml");
            context.Response.StatusCode = (int)StatusCode.MultiStatus;
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
    }
}
