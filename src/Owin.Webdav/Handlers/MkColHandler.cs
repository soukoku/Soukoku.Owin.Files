using Soukoku.Owin.Webdav.Models;
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
        private WebdavConfig _options;

        public MkColHandler(WebdavConfig options)
        {
            _options = options;
        }

        public async Task<StatusCode> HandleAsync(Context context, IResource resource)
        {
            if (resource != null)
            {
                return StatusCode.MethodNotAllowed;
            }

            var newPath = context.Request.Path.Trim('/');
            var parentPath = Path.GetDirectoryName(newPath).Replace(Path.DirectorySeparatorChar, '/');
            IResource parent = _options.DataStore.GetResource(context.Request.PathBase, parentPath);
            if (string.IsNullOrEmpty(newPath) || parent == null)
            {
                return StatusCode.Conflict;
            }

            string body = await context.Request.Body.ReadStringAsync();
            if (!string.IsNullOrEmpty(body))
            {
                return StatusCode.UnsupportedMediaType;
            }


            ResourceStatus result = _options.DataStore.CreateCollection(parent, Path.GetFileName(newPath));
            return result.Code;
        }

    }
}
