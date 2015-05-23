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
                context.Response.Headers.Append(Consts.Header.Dav, _options.DavClass.ToString().Replace("Class", ""));
                context.Response.Headers.AppendCommaSeparatedValues("Allow",
                    Consts.Method.Options,
                    Consts.Method.PropFind,
                    Consts.Method.PropPatch,
                    Consts.Method.MkCol,
                    Consts.Method.Copy,
                    Consts.Method.Move,
                    Consts.Method.Delete,
                    Consts.Method.Lock,
                    Consts.Method.Unlock,
                    Consts.Method.Get);

                context.Response.Headers.AppendCommaSeparatedValues("Public",
                    Consts.Method.Options,
                    Consts.Method.PropFind,
                    Consts.Method.PropPatch,
                    Consts.Method.MkCol,
                    Consts.Method.Copy,
                    Consts.Method.Move,
                    Consts.Method.Delete,
                    Consts.Method.Lock,
                    Consts.Method.Unlock,
                    Consts.Method.Get);

                context.Response.ContentLength = 0;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

    }
}
