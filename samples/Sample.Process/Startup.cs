using Owin;
using Soukoku.Owin.Webdav;
using System;
using System.IO;

namespace Sample.Process
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var path = Path.Combine(Environment.CurrentDirectory, @"..\..\dav-store");
            var davCfg = new WebdavConfig(new Owin.Webdav.StaticDataStore(path))
            {
                AllowDirectoryBrowsing = true,
                Log = new TraceLog(System.Diagnostics.TraceLevel.Verbose)
            };

            //app.Map("/davroot", map =>
            //{
            //    map.Use<WebdavMiddleware>(davCfg);
            //});
            app.Use<WebdavMiddleware>(davCfg);
        }
    }
}
