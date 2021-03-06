﻿#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion Usings

namespace Furcadia.Logging
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="Furcadia.Logging.ILogOutput" />
    public class ConsoleLogOutput : ILogOutput
    {
        ///private static readonly DateTime startTime = new DateTime();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogOutput"/> class.
        /// </summary>
        public ConsoleLogOutput()
        {
        }

        /// <summary>
        /// Builds the message.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns></returns>
        protected LogMessage BuildMessage(ref LogMessage msg)
        {
            var level = msg.Level;
            var text = msg.message;
            var sb = new StringBuilder();
            sb.Append('[')
              .Append(level.ToString().ToUpper())
              .Append(']')
              .Append("Thread+" + msg.Thread.ManagedThreadId)
              .Append(' ')
              //.Append(msg.TimeStamp.ToString("dd-MMM-yyyy")).Append(' ')
              .Append((msg.TimeStamp - Process.GetCurrentProcess().StartTime).ToString(@"hh\:mm\:ss\:fff"))
              .Append(" - ")
              .Append(text);
            msg.message = sb.ToString();
            return msg;
        }

        /// <summary>
        /// Logs the specified log MSG.
        /// </summary>
        /// <param name="logMsg">The log MSG.</param>
        public virtual void Log(LogMessage logMsg)
        {
            if (logMsg.message == null)
                return;

            logMsg = BuildMessage(ref logMsg);
            var msg = logMsg.message;
            try
            {
                ConsoleColor original = Console.ForegroundColor;
                ConsoleColor color = ConsoleColor.White;
                switch (logMsg.Level)
                {
                    case Level.Debug:
                    case Level.Warning:
                        color = ConsoleColor.Yellow;
                        break;

                    case Level.Error:
                        color = ConsoleColor.Red;
                        break;

                    case Level.Info:
                        color = ConsoleColor.White;
                        break;
                }
                Console.ForegroundColor = color;
            }
            catch
            {
            }
            if (Debugger.IsAttached)
                Debug.WriteLine(msg);
            Console.WriteLine(msg);
            try
            {
                Console.ResetColor();
            }
            catch { }
        }
    }
}