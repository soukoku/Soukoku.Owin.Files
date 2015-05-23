using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav
{
    /// <summary>
    /// An implementation of <see cref="ILog"/> that writes to <see cref="Trace"/>.
    /// </summary>
    public class TraceLog : ILog
    {
        private TraceLevel _level;
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceLog"/> class.
        /// </summary>
        /// <param name="level">The level.</param>
        public TraceLog(TraceLevel level)
        {
            _level = level;
        }

        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void LogDebug(string format, params object[] args)
        {
            if (_level >= TraceLevel.Verbose)
                WriteIt("Debug: " + string.Format(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void LogError(string format, params object[] args)
        {
            if (_level >= TraceLevel.Error)
                WriteIt("Error: " + string.Format(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// Logs the fatal message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void LogFatal(string format, params object[] args)
        {
            if (_level >= TraceLevel.Error)
                WriteIt("Fatal: " + string.Format(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void LogInfo(string format, params object[] args)
        {
            if (_level >= TraceLevel.Info)
                WriteIt("Info: " + string.Format(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void LogWarning(string format, params object[] args)
        {
            if (_level >= TraceLevel.Warning)
                WriteIt("Warning: " + string.Format(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// Writes it.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        static void WriteIt(string logEntry)
        {
            Trace.WriteLine(logEntry);
        }
    }

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
