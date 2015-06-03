using Owin;
using Soukoku.Owin.Webdav;
using System.IO;
using Microsoft.Owin.Extensions;

namespace Sample.IIS
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"dav-store");
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
            // required to make sure requests don't get caught by IIS static file handler
            app.UseStageMarker(PipelineStage.MapHandler);
        }
    }
}
