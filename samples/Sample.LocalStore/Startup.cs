using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin.Webdav;
using System.IO;

namespace Sample.LocalStore
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "dav-store");


            app.UseWebdav(new Owin.Webdav.WebdavConfig(new LocalDataStore(path))
            {

            });
        }
    }
}
