using Microsoft.Owin.Hosting;
using Owin;
using Soukoku.Owin.Files;
using Soukoku.Owin.Files.Services.BuiltIn;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FilesRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var urlRoot = "http://localhost:12345";
            using (WebApp.Start<Startup>(urlRoot))
            {
                Console.WriteLine("Press enter to exit...");

                using (Process.Start(urlRoot + "/index.html")) { }
                Console.ReadLine();
            }
        }

        class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                //app.UseErrorPage();

                // serve files from a fs folder.
                app.Map("/loose", mapped =>
                {
                    var path = Path.Combine(Environment.CurrentDirectory, @"..\..\wwwroot");
                    var cfg = new FilesConfig(new LooseFilesDataStore(path))
                    {
                        AllowDirectoryBrowsing = true,
                        Log = new TraceLog(System.Diagnostics.TraceLevel.Verbose)
                    };
                    mapped.Use<FilesMiddleware>(cfg);
                });

                // server only a single file.
                app.Map("/single", mapped =>
                {
                    var filePath = Path.Combine(Environment.CurrentDirectory, @"..\..\wwwroot\dummy.txt");
                    if (File.Exists(filePath))
                    {
                        var store = new SingleFilesDataStore();
                        store.SetFile(filePath);
                        mapped.Use<FilesMiddleware>(new FilesConfig(store)
                        {
                            AllowDirectoryBrowsing = true
                        });
                    }
                });

                // serve files from a zip file
                app.Map("/pdfjs", mapped =>
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
                });

                // server files from windows zip (path is a\b\c)
                app.Map("/winzip", mapped =>
                {
                    var zipPath = Path.Combine(Environment.CurrentDirectory, @"help.zip");
                    if (File.Exists(zipPath))
                    {
                        var cfg = new FilesConfig(new ZippedFileDataStore(File.ReadAllBytes(zipPath)))
                        {
                            AllowDirectoryBrowsing = false,
                            Log = new TraceLog(System.Diagnostics.TraceLevel.Verbose)
                        };
                        mapped.Use<FilesMiddleware>(cfg);
                    }
                });

                // serve files from this assembly's resource
                app.Use<FilesMiddleware>(new FilesConfig(new AssemblyResourceDataStore(Assembly.GetExecutingAssembly(), "FilesRunner.Resources"))
                {
                    AllowDirectoryBrowsing = true,
                });
            }
        }
    }
}

