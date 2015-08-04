using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Soukoku.Owin.Files.DavConsts;

namespace Soukoku.Owin.Files.Services
{
    sealed class OptionsHandler : IMethodHandler
    {
        public Task<int> HandleAsync(Resource resource)
        {
            if (resource != null)
            {
                // lie and say we can deal with it all for now

                resource.Context.Response.Headers.Replace("MS-Author-Via", "DAV");

                var cls = "1";
                string methods;
                //if (resource.Resource.SupportedLocks != LockScopes.None)
                //{
                //    cls += ",2";
                //    methods = string.Join(",", HttpMethodNames.Options,
                //        HttpMethodNames.PropFind,
                //        HttpMethodNames.PropPatch,
                //        HttpMethodNames.MakeCollection,
                //        HttpMethodNames.Get,
                //        HttpMethodNames.Head,
                //        //HttpMethodNames.Post,
                //        HttpMethodNames.Delete,
                //        HttpMethodNames.Put,
                //        HttpMethodNames.Copy,
                //        HttpMethodNames.Move,
                //        HttpMethodNames.Lock,
                //        HttpMethodNames.Unlock);
                //}
                //else
                //{
                methods = string.Join(",", HttpMethodNames.Options,
                    HttpMethodNames.PropFind,
                    HttpMethodNames.PropPatch,
                    HttpMethodNames.MakeCollection,
                    HttpMethodNames.Get,
                    HttpMethodNames.Head,
                    //HttpMethodNames.Post,
                    HttpMethodNames.Delete,
                    HttpMethodNames.Put,
                    HttpMethodNames.Copy,
                    HttpMethodNames.Move);
                //}

                resource.Context.Response.Headers.Replace(DavConsts.Headers.Dav, cls);
                resource.Context.Response.Headers.Replace("Allow", methods);
                resource.Context.Response.Headers.Replace("Public", methods); // ???

                resource.Context.Response.Headers.ContentLength = 0;
                return Task.FromResult((int)StatusCode.OK);
            }
            return Task.FromResult((int)StatusCode.NotHandled);
        }

    }
}
