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
                context.Response.Headers.Replace(DavConsts.Headers.Dav, _options.DavClass.ToString().Replace("Class", ""));
                context.Response.Headers.Replace("Allow",
                    DavConsts.Methods.Options,
                    DavConsts.Methods.PropFind,
                    DavConsts.Methods.PropPatch,
                    DavConsts.Methods.MkCol,
                    DavConsts.Methods.Copy,
                    DavConsts.Methods.Move,
                    DavConsts.Methods.Delete,
                    DavConsts.Methods.Lock,
                    DavConsts.Methods.Unlock,
                    DavConsts.Methods.Get);

                context.Response.Headers.Replace("Public",
                    DavConsts.Methods.Options,
                    DavConsts.Methods.PropFind,
                    DavConsts.Methods.PropPatch,
                    DavConsts.Methods.MkCol,
                    DavConsts.Methods.Copy,
                    DavConsts.Methods.Move,
                    DavConsts.Methods.Delete,
                    DavConsts.Methods.Lock,
                    DavConsts.Methods.Unlock,
                    DavConsts.Methods.Get);

                context.Response.Headers.ContentLength = 0;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

    }
}
