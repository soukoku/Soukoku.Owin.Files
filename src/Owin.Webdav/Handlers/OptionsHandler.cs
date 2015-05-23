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

namespace Soukoku.Owin.Webdav.Handlers
{
    sealed class OptionsHandler : IMethodHandler
    {
        private WebdavConfig _options;

        public OptionsHandler(WebdavConfig options)
        {
            _options = options;
        }

        public Task<bool> HandleAsync(IOwinContext context, IResource resource)
        {
            if (resource != null)
            {
                // lie and say we can deal with it all for now

                context.Response.Headers.Append("MS-Author-Via", "DAV");
                context.Response.Headers.Append(Consts.Headers.Dav, _options.DavClass.ToString().Replace("Class", ""));
                context.Response.Headers.AppendCommaSeparatedValues("Allow",
                    Consts.Methods.Options,
                    Consts.Methods.PropFind,
                    Consts.Methods.PropPatch,
                    Consts.Methods.MkCol,
                    Consts.Methods.Copy,
                    Consts.Methods.Move,
                    Consts.Methods.Delete,
                    Consts.Methods.Lock,
                    Consts.Methods.Unlock,
                    Consts.Methods.Get);

                context.Response.Headers.AppendCommaSeparatedValues("Public",
                    Consts.Methods.Options,
                    Consts.Methods.PropFind,
                    Consts.Methods.PropPatch,
                    Consts.Methods.MkCol,
                    Consts.Methods.Copy,
                    Consts.Methods.Move,
                    Consts.Methods.Delete,
                    Consts.Methods.Lock,
                    Consts.Methods.Unlock,
                    Consts.Methods.Get);

                context.Response.ContentLength = 0;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

    }
}
