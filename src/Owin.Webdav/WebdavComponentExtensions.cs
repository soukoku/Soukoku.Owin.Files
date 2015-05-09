using Owin.Webdav;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin
{
    public static class WebdavComponentExtensions
    {
        public static IAppBuilder UseWebdav(this IAppBuilder app, WebdavConfig options)
        {
            return app.Use<WebdavComponent>(options);
        }
    }
}
