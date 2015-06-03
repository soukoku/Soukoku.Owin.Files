using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Process
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = IsAdministrator ? "http://+:9000" : "http://localhost:9000";

            Console.WriteLine("Staring server on {0}", baseAddress);
            using (WebApp.Start<Startup>(baseAddress))
            //using (DriveMapper.MapToNextAvailableDrive(baseAddress))
            {
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }
        }

        static bool IsAdministrator
        {
            get
            {
                var wi = WindowsIdentity.GetCurrent();
                var wp = new WindowsPrincipal(wi);

                return wp.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

    }
}
