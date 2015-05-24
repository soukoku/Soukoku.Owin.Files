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
    sealed class OptionsHandler : IMethodHandler
    {
        public Task<StatusCode> HandleAsync(DavContext context, ResourceResponse resource)
        {
            if (resource.Resource != null)
            {
                // lie and say we can deal with it all for now

                context.Response.Headers.Replace("MS-Author-Via", "DAV");

                var cls = "1";
                string methods;
                if (resource.Resource.SupportedLocks != LockScopes.None)
                {
                    cls += ",2";
                    methods = string.Join(",", DavConsts.Methods.Options,
                        DavConsts.Methods.PropFind,
                        DavConsts.Methods.PropPatch,
                        DavConsts.Methods.MkCol,
                        DavConsts.Methods.Get,
                        DavConsts.Methods.Head,
                        //DavConsts.Methods.Post,
                        DavConsts.Methods.Delete,
                        DavConsts.Methods.Put,
                        DavConsts.Methods.Copy,
                        DavConsts.Methods.Move,
                        DavConsts.Methods.Lock,
                        DavConsts.Methods.Unlock);
                }
                else
                {
                    methods = string.Join(",", DavConsts.Methods.Options,
                        DavConsts.Methods.PropFind,
                        DavConsts.Methods.PropPatch,
                        DavConsts.Methods.MkCol,
                        DavConsts.Methods.Get,
                        DavConsts.Methods.Head,
                        //DavConsts.Methods.Post,
                        DavConsts.Methods.Delete,
                        DavConsts.Methods.Put,
                        DavConsts.Methods.Copy,
                        DavConsts.Methods.Move);
                }

                context.Response.Headers.Replace(DavConsts.Headers.Dav, cls);
                context.Response.Headers.Replace("Allow", methods);
                context.Response.Headers.Replace("Public", methods); // ???

                context.Response.Headers.ContentLength = 0;
                return Task.FromResult(StatusCode.OK);
            }
            return Task.FromResult(StatusCode.NotHandled);
        }

    }
}
