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
    sealed class OptionsHandler : IMethodHandler
    {
        private WebdavConfig _options;

        public OptionsHandler(WebdavConfig options)
        {
            _options = options;
        }

        public Task<bool> HandleAsync(Context context, IResource resource)
        {
            if (resource != null)
            {
                // lie and say we can deal with it all for now

                context.Response.Headers.Replace("MS-Author-Via", "DAV");
                context.Response.Headers.Replace(DavConsts.Headers.Dav, _options.DavClass.ToString().Replace("Class", "").Replace(" ", ""));
                context.Response.Headers.Replace("Allow",
                    string.Join(",",
                    DavConsts.Methods.Options,
                    DavConsts.Methods.PropFind,
                    DavConsts.Methods.PropPatch,
                    DavConsts.Methods.MkCol,
                    DavConsts.Methods.Get,
                    DavConsts.Methods.Post,
                    DavConsts.Methods.Delete,
                    DavConsts.Methods.Put,
                    DavConsts.Methods.Copy,
                    DavConsts.Methods.Move,
                    DavConsts.Methods.Lock,
                    DavConsts.Methods.Unlock));

                context.Response.Headers.Replace("Public",
                    string.Join(",",
                    DavConsts.Methods.Options,
                    DavConsts.Methods.PropFind,
                    DavConsts.Methods.PropPatch,
                    DavConsts.Methods.MkCol,
                    DavConsts.Methods.Get,
                    DavConsts.Methods.Post,
                    DavConsts.Methods.Delete,
                    DavConsts.Methods.Put,
                    DavConsts.Methods.Copy,
                    DavConsts.Methods.Move,
                    DavConsts.Methods.Lock,
                    DavConsts.Methods.Unlock));

                context.Response.Headers.ContentLength = 0;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

    }
}
