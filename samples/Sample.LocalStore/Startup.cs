using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin.Webdav;
using System.IO;
using Soukoku.Owin.Webdav;

namespace Sample.LocalStore
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "dav-store");

            //app.Map("/davroot", map =>
            //{
                app.UseWebdav(new WebdavConfig(new LocalDataStore(path))
                {
                    AllowDirectoryBrowsing = true
                });
            //});
            // write dummy file
            File.WriteAllText(Path.Combine(path, "dummy.txt"), "This is a dummy file.");
        }
    }
}
