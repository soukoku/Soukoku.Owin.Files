using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.LocalStore
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:9000/";

            Console.WriteLine("Staring server on {0}", baseAddress);
            using (WebApp.Start<Startup>(baseAddress))
            using (DriveMapper.MapToNextAvailableDrive(baseAddress))
            {
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }
        }

    }
}
