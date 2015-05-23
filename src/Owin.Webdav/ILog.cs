namespace Soukoku.Owin.Webdav
{
    /// <summary>
    /// Inteface used by the webdav componnet to do process logging.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The arguments.</param>
        void LogDebug(string format, params object[] args);

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The arguments.</param>
        void LogError(string format, params object[] args);

        /// <summary>
        /// Logs the fatal message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The arguments.</param>
        void LogFatal(string format, params object[] args);

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The arguments.</param>
        void LogInfo(string format, params object[] args);

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The arguments.</param>
        void LogWarning(string format, params object[] args);
    }
}