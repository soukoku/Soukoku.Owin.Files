using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav.DefaultImp
{
    class NullLog : ILog
    {
        public void LogDebug(string format, params object[] args)
        {
        }

        public void LogError(string format, params object[] args)
        {
        }

        public void LogFatal(string format, params object[] args)
        {
        }

        public void LogInfo(string format, params object[] args)
        {
        }

        public void LogWarning(string format, params object[] args)
        {
        }
    }
}
