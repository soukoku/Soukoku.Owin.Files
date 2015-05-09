using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sample.LocalStore
{
    class DriveMapper
    {
        public static IDisposable MapToNextAvailableDrive(string url)
        {
            var nextLetter = GetFreeDriveLetters().LastOrDefault();
            if (nextLetter == 0)
            {
                Console.WriteLine("No free drive available!");
                return null;
            }
            else
            {
                Console.WriteLine("Mapping to drive {0}...", nextLetter);
                return new MappedDrive(nextLetter, url);
            }
        }

        private static IEnumerable<char> GetFreeDriveLetters()
        {
            List<char> available = new List<char>();
            for (char i = 'A'; i <= 'Z'; i++)
            {
                available.Add(i);
            }

            foreach (var d in Directory.GetLogicalDrives())
            {
                available.Remove(d[0]);
            }
            return available;
        }

        class MappedDrive : IDisposable
        {
            string _letterPath;
            bool _mapped;
            public MappedDrive(char driveLetter, string url)
            {
                _letterPath = driveLetter + ":";

                WshNetwork net = null;
                try
                {
                    net = new WshNetworkClass();
                    object updateProfile = false;
                    net.MapNetworkDrive(_letterPath, url, ref updateProfile);
                    _mapped = true;
                    // use windows explorer to test it!
                    using (Process.Start(_letterPath)) { }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to connect drive: {0}.", ex.Message);
                }
                finally
                {
                    if (net != null)
                    {
                        Marshal.ReleaseComObject(net);
                    }
                }
            }

            public void Dispose()
            {
                if (!_mapped) { return; }

                WshNetwork net = null;
                try
                {
                    net = new WshNetworkClass();
                    object force = true;
                    object updateProfile = false;
                    net.RemoveNetworkDrive(_letterPath, ref force, ref updateProfile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to disconnect drive: {0}.", ex.Message);
                }
                finally
                {
                    if (net != null)
                    {
                        Marshal.ReleaseComObject(net);
                    }
                }
            }
        }
    }
}
