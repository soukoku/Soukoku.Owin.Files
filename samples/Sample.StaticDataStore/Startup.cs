using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin.Webdav;
using System.IO;
using Soukoku.Owin.Webdav;

namespace Sample.StaticDataStore
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var path = Path.Combine(Environment.CurrentDirectory, @"..\..\dav-store");
            var davCfg = new WebdavConfig(new Owin.Webdav.StaticDataStore(path))
            {
                AllowDirectoryBrowsing = true,
                DavClass = DavClasses.Class1 | DavClasses.Class2,
                Log = new TraceLog(System.Diagnostics.TraceLevel.Verbose)
            };



            //app.Map("/davroot", map =>
            //{
            app.Use<WebdavMiddleware>(davCfg);
            //});
            // write dummy file
            File.WriteAllText(Path.Combine(path, "dummy.txt"), "This is a dummy file.");
        }
    }
}
