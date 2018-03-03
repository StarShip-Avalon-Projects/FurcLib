namespace Furcadia.Logging
{
    /// <summary>
    ///
    /// </summary>
    public interface ILogOutput
    {
        /// <summary>
        /// Logs the specified log MSG.
        /// </summary>
        /// <param name="logMsg">The log MSG.</param>
        void Log(LogMessage logMsg);
    }
}