//using Soukoku.Owin.Webdav.Models;
//using Soukoku.Owin.Webdav.Responses;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using static Soukoku.Owin.Files.DavConsts;

//namespace Soukoku.Owin.Files.Services
//{
//    sealed class MkColHandler : IMethodHandler
//    {
//        public async Task<int> HandleAsync(Resource resource)
//        {
//            if (resource != null)
//            {
//                return (int)StatusCode.MethodNotAllowed;
//            }

//            var newPath = resource.Context.Request.Path.Trim('/');
//            var parentPath = Path.GetDirectoryName(newPath).Replace(Path.DirectorySeparatorChar, '/');
//            ResourceResult parent = context.Config.DataStore.GetResource(context, parentPath);
//            if (string.IsNullOrEmpty(newPath) || parent.Resource == null)
//            {
//                return (int)StatusCode.Conflict;
//            }

//            string body = await resource.Context.Request.Body.ReadStringAsync();
//            if (!string.IsNullOrEmpty(body))
//            {
//                return StatusCode.UnsupportedMediaType;
//            }


//            var code = context.Config.DataStore.CreateCollection(parent.Resource, Path.GetFileName(newPath));
//            return code;
//        }

//    }
//}
