using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav
{
    public class TraceLog : ILog
    {
        private TraceLevel _level;
        public TraceLog(TraceLevel level)
        {
            _level = level;
        }
        
        public void Debug(string format, params object[] args)
        {
            if (_level >= TraceLevel.Verbose)
                WriteIt("Debug: " + string.Format(format, args));
        }
        
        public void Error(string format, params object[] args)
        {
            if (_level >= TraceLevel.Error)
                WriteIt("Error: " + string.Format(format, args));
        }
        
        public void Fatal(string format, params object[] args)
        {
            if (_level >= TraceLevel.Error)
                WriteIt("Fatal: " + string.Format(format, args));
        }
        
        public void Info(string format, params object[] args)
        {
            if (_level >= TraceLevel.Info)
                WriteIt("Info: " + string.Format(format, args));
        }
        
        public void Warn(string format, params object[] args)
        {
            if (_level >= TraceLevel.Warning)
                WriteIt("Warning: " + string.Format(format, args));
        }

        void WriteIt(string message)
        {
            Trace.WriteLine(message);
        }
    }

    class NullLog : ILog
    {
        public void Debug(string format, params object[] args)
        {
        }

        public void Error(string format, params object[] args)
        {
        }

        public void Fatal(string format, params object[] args)
        {
        }

        public void Info(string format, params object[] args)
        {
        }

        public void Warn(string format, params object[] args)
        {
        }
    }
}
