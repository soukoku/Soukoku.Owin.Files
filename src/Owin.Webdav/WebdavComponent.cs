using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
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


        public override async Task Invoke(IOwinContext context)
        {
            Console.WriteLine("Begin Webdav Request " + context.Request.Uri);
            await Next.Invoke(context);
            Console.WriteLine("End Webdav Request");
        }
    }
}
