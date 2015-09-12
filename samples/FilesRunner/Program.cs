using Microsoft.Owin.Hosting;
using Owin;
using Soukoku.Owin.Files;
using Soukoku.Owin.Files.Services.BuiltIn;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesRunner
{
    class Program
    {
        const string looseRoot = "/loose";
        const string zippedRoot = "/zipped";
        const string pdfjsRoot = "/pdfjs";
        const string webdavRoot = "/webdav";

        static void Main(string[] args)
        {
            var urlRoot = "http://localhost:12345";
            using (WebApp.Start<Startup>(urlRoot))
            {
                Console.WriteLine("Press enter to exit...");

                //using (Process.Start(urlRoot + looseRoot)) { }
                //using (Process.Start(urlRoot + zippedRoot)) { }
                using (Process.Start(urlRoot + pdfjsRoot)) { }
                Console.ReadLine();
            }
        }

        class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                //app.UseErrorPage();

                //app.Map(looseRoot, mapped =>
                //{
                //    var path = Path.Combine(Environment.CurrentDirectory, @"..\..\wwwroot");
                //    var cfg = new FilesConfig(new LooseFilesDataStore(path))
                //    {
                //        AllowDirectoryBrowsing = true,
                //        Log = new TraceLog(System.Diagnostics.TraceLevel.Verbose)
                //    };
                //    mapped.Use<FilesMiddleware>(cfg);
                //});

                //app.Map(zippedRoot, mapped =>
                //{
                //    var zipPath = Path.Combine(Environment.CurrentDirectory, @"wwwroot.zip");
                //    if (File.Exists(zipPath))
                //    {
                //        var ms = new MemoryStream(File.ReadAllBytes(zipPath));
                //        var cfg = new FilesConfig(new ZippedFileDataStore(ms))
                //        {
                //            AllowDirectoryBrowsing = true,
                //            Log = new TraceLog(System.Diagnostics.TraceLevel.Verbose)
                //        };
                //        mapped.Use<FilesMiddleware>(cfg);
                //    }
                //});

                app.Map("/single", mapped =>
                {
                    var filePath = Path.Combine(Environment.CurrentDirectory, @"..\..\wwwroot\dummy.txt");
                    if (File.Exists(filePath))
                    {
                        mapped.Use<FilesMiddleware>(new FilesConfig(new SingleFilesDataStore(filePath))
                        {
                            AllowDirectoryBrowsing = true
                        });
                    }
                });

                app.Map(pdfjsRoot, mapped =>
                {
                    var zipPath = Path.Combine(Environment.CurrentDirectory, @"pdfjs-1.1.114-dist.zip");
                    if (File.Exists(zipPath))
                    {
                        var cfg = new FilesConfig(new ZippedFileDataStore(File.ReadAllBytes(zipPath)))
                        {
                            AllowDirectoryBrowsing = true,
                            Log = new TraceLog(System.Diagnostics.TraceLevel.Verbose)
                        };
                        mapped.Use<FilesMiddleware>(cfg);
                    }

                    //var path = Path.Combine(Environment.CurrentDirectory, @"pdfjs-1.1.114-dist");
                    //var cfg = new FilesConfig(new LooseFilesDataStore(path))
                    //{
                    //    AllowDirectoryBrowsing = true,
                    //    Log = new TraceLog(System.Diagnostics.TraceLevel.Verbose)
                    //};
                    //mapped.Use<FilesMiddleware>(cfg);
                });
            }
        }
    }
}

