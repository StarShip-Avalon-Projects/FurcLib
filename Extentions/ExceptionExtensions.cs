#region Usings

using Furcadia.Logging;
using System;
using System.Runtime.CompilerServices;

#endregion Usings

namespace Extentions
{
    /// <summary>
    ///
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="level">The level.</param>
        /// <param name="memberName">Name of the member.</param>
        public static void Log(this Exception ex, Level level = Level.Debug, [CallerMemberName] string memberName = "")
        {
            if (ex != null)
            {
                switch (level)
                {
                    case Level.Info:
                        Logger.Info(ex.Flatten(), memberName);
                        break;

                    case Level.Error:
                        Logger.Error(ex.Flatten(), memberName);
                        break;

                    case Level.Debug:
                        Logger.Debug(ex.Flatten(), memberName);
                        break;

                    case Level.Warning:
                        Logger.Warn(ex.Flatten(), memberName);
                        break;

                    default:
                        Logger.Debug(ex.Flatten(), memberName);
                        break;
                }
            }
        }

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex">The ex.</param>
        /// <param name="level">The level.</param>
        /// <param name="memberName">Name of the member.</param>
        public static void Log<T>(this Exception ex, Level level = Level.Debug, [CallerMemberName] string memberName = "")
        {
            if (ex != null)
            {
                switch (level)
                {
                    case Level.Info:
                        Logger.Info<T>(ex.Flatten(), memberName);
                        break;

                    case Level.Error:
                        Logger.Error<T>(ex.Flatten(), memberName);
                        break;

                    case Level.Debug:
                        Logger.Debug<T>(ex.Flatten(), memberName);
                        break;

                    case Level.Warning:
                        Logger.Warn<T>(ex.Flatten(), memberName);
                        break;

                    default:
                        Logger.Debug<T>(ex.Flatten(), memberName);
                        break;
                }
            }
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="level">The level.</param>
        /// <param name="memberName">Name of the member.</param>
        public static void LogMessage(this Exception ex, Level level = Level.Debug, [CallerMemberName] string memberName = "")
        {
            if (ex != null)
            {
                switch (level)
                {
                    case Level.Info:
                        Logger.Info(ex.Message, memberName);
                        break;

                    case Level.Error:
                        Logger.Error(ex.Message, memberName);
                        break;

                    case Level.Debug:
                        Logger.Debug(ex.Message, memberName);
                        break;

                    case Level.Warning:
                        Logger.Warn(ex.Message, memberName);
                        break;
                }
            }
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex">The ex.</param>
        /// <param name="level">The level.</param>
        /// <param name="memberName">Name of the member.</param>
        public static void LogMessage<T>(this Exception ex, Level level = Level.Debug, [CallerMemberName] string memberName = "")
        {
            if (ex != null)
            {
                switch (level)
                {
                    case Level.Info:
                        Logger.Info<T>(ex.Message, memberName);
                        break;

                    case Level.Error:
                        Logger.Error<T>(ex.Message, memberName);
                        break;

                    case Level.Debug:
                        Logger.Debug<T>(ex.Message, memberName);
                        break;

                    case Level.Warning:
                        Logger.Warn<T>(ex.Message, memberName);
                        break;
                }
            }
        }

        /// <summary>
        /// Flattens the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static string Flatten(this Exception exception)
        {
            var stringBuilder = new System.Text.StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
                if (exception != null) stringBuilder.AppendLine("Inner exception:");
            }

            return stringBuilder.ToString();
        }
    }
}