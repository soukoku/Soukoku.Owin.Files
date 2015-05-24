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

namespace Soukoku.Owin.Webdav.Handlers
{
    sealed class MkColHandler : IMethodHandler
    {
        public async Task<StatusCode> HandleAsync(DavContext context, ResourceResponse resource)
        {
            if (resource.Resource != null)
            {
                return StatusCode.MethodNotAllowed;
            }

            var newPath = context.Request.Path.Trim('/');
            var parentPath = Path.GetDirectoryName(newPath).Replace(Path.DirectorySeparatorChar, '/');
            ResourceResponse parent = context.Config.DataStore.GetResource(context, parentPath);
            if (string.IsNullOrEmpty(newPath) || parent.Resource == null)
            {
                return StatusCode.Conflict;
            }

            string body = await context.Request.Body.ReadStringAsync();
            if (!string.IsNullOrEmpty(body))
            {
                return StatusCode.UnsupportedMediaType;
            }


            var code = context.Config.DataStore.CreateCollection(parent.Resource, Path.GetFileName(newPath));
            return code;
        }

    }
}
