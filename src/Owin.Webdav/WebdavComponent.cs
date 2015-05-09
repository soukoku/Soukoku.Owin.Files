using Microsoft.Owin;
using Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Owin.Webdav
{
    public class WebdavComponent : OwinMiddleware
    {
        readonly WebdavConfig _options;

        public WebdavComponent(OwinMiddleware next, WebdavConfig options) : base(next)
        {
            if (options == null) { throw new ArgumentException("options"); }

            _options = options;
        }


        public override Task Invoke(IOwinContext context)
        {
            switch (context.Request.Method.ToUpperInvariant())
            {
                case "PROPFIND":
                    return HandlePropFind(context);
                case "GET":
                    return HandleGet(context);
                default:
                    return Next.Invoke(context);
            }
        }

        private Task HandlePropFind(IOwinContext context)
        {
            var path = context.Request.Uri.AbsolutePath;
            Resource resource = _options.DataStore.GetResource(path);
            if (resource != null)
            {

            }
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Task.FromResult(0);
        }

        private Task HandleGet(IOwinContext context)
        {
            
            Console.WriteLine("Begin Webdav Request " + context.Request.Uri);
            Console.WriteLine("End Webdav Request");
            return Task.FromResult(0);
        }
    }
}
