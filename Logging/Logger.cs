#region Usings

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Furcadia.Logging;
using Furcadia.Extensions;

#endregion Usings

namespace Furcadia.Logging
{
    /// <summary>
    ///
    /// </summary>
    public enum Level : byte
    {
        /// <summary>
        /// The information
        /// </summary>
        Info = 1,

        /// <summary>
        /// The warning
        /// </summary>
        Warning = 2,

        /// <summary>
        /// The error
        /// </summary>
        Error = 3,

        /// <summary>
        /// The debug
        /// </summary>
        Debug = 4
    }

    internal class LogMessageComparer : IComparer<LogMessage>
    {
        public int Compare(LogMessage x, LogMessage y)
        {
            if (x.TimeStamp > y.TimeStamp) return -1;
            if (x.TimeStamp < y.TimeStamp) return 1;
            return 0;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public struct LogMessage : IEquatable<LogMessage>
    {
        /// <summary>
        /// The message
        /// </summary>
        public string message;

        private readonly DateTime expires, timeStamp;
        private readonly Level level;

        private readonly Thread curThread;

        private bool IsEmpty
        {
            get { return string.IsNullOrWhiteSpace(message); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is spam.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is spam; otherwise, <c>false</c>.
        /// </value>
        public bool IsSpam
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public Level Level { get { return level; } }

        /// <summary>
        /// Gets the thread.
        /// </summary>
        /// <value>
        /// The thread.
        /// </value>
        public Thread Thread => curThread;

        /// <summary>
        /// Gets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        public DateTime TimeStamp => timeStamp;

        private LogMessage(Level level, string msg, TimeSpan expireDuration)
        {
            this.level = level;
            message = string.IsNullOrEmpty(msg) ? string.Empty : msg;
            var now = DateTime.Now;
            expires = now.Add(expireDuration);
            timeStamp = now;
            IsSpam = false;
            curThread = Thread.CurrentThread;
        }

        /// <summary>
        /// Froms the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="expireDuration">Duration of the expire.</param>
        /// <returns></returns>
        public static LogMessage? From(Level level, string msg, TimeSpan expireDuration)
        {
            LogMessage logMsg = new LogMessage(level, msg, expireDuration);
            var now = DateTime.Now;
            bool found = false;

            for (int i = Logger.history.Count - 1; i >= 0; i--)
            {
                var logMessage = Logger.history[i];
                if (!found && logMessage.Equals(logMsg))
                {
                    found = true;
                    break;
                }
                if (logMessage.expires < now)
                    Logger.history.RemoveAt(i);
            }

            if (found && Logger.SuppressSpam)
                logMsg.IsSpam = true;
            else logMsg.IsSpam = false;

            if (!logMsg.IsSpam)
            {
                Logger.history.Add(logMsg);
                return logMsg;
            }
            return null;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj != null && obj is LogMessage lm && Equals(lm);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return message;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(LogMessage other)
        {
            return message == other.message &&
                   timeStamp == other.timeStamp &&
                   level == other.level &&
                   EqualityComparer<Thread>.Default.Equals(curThread, other.curThread);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = -975352547;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(message);
            hashCode = hashCode * -1521134295 + timeStamp.GetHashCode();
            hashCode = hashCode * -1521134295 + level.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Thread>.Default.GetHashCode(curThread);
            return hashCode;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public static class Logger
    {
        private static ILogOutput _logOutput;
        internal static readonly ConcurrentList<LogMessage> history = new ConcurrentList<LogMessage>();
        internal static readonly ConcurrentQueue<LogMessage> queue = new ConcurrentQueue<LogMessage>();
        private static readonly ConcurrentList<Type> disabledTypes = new ConcurrentList<Type>();
        private static LogMessageComparer comparer = new LogMessageComparer();
        private static object syncObj = new object();
        private static bool _infoEnabled = true;
        private static bool _warningEnabled = true;
        private static bool _errorEnabled = true;
        private static bool _debugEnabled;
        private static bool _suppressSpam;
        private static TimeSpan _messagesExpire = TimeSpan.FromSeconds(10);

        private static bool singleThreaded; //, initialized;

        private static Task logTask;

        private static CancellationTokenSource cancelToken;

        /// <summary>
        /// Occurs when [spam found].
        /// </summary>
        public static event Action<LogMessage> SpamFound;

        static Logger()
        {
            _logOutput = new ConsoleLogOutput();
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => Error(args.ExceptionObject);

#if DEBUG
            _debugEnabled = true;
            LogCallingMethod = true;
#else
            _debugEnabled = false; // can be set via property
            LogCallingMethod = false;
#endif
            singleThreaded = true;

            Initialize();
        }

        private static void Initialize()
        {
            cancelToken = new CancellationTokenSource();
            logTask = new Task(() =>
            {
                while (true)
                {
                    Thread.Sleep(10);
                    if (!singleThreaded)
                    {
                        // take a dump
                        Dump();
                    }
                }
            }, cancelToken.Token, TaskCreationOptions.LongRunning);
            if (!singleThreaded) logTask.Start();
        }

        /// <summary>
        /// Gets or sets a value indicating whether [log calling method].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [log calling method]; otherwise, <c>false</c>.
        /// </value>
        public static bool LogCallingMethod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [information enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [information enabled]; otherwise, <c>false</c>.
        /// </value>
        public static bool InfoEnabled
        {
            get { return _infoEnabled; }
            set { _infoEnabled = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [warning enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [warning enabled]; otherwise, <c>false</c>.
        /// </value>
        public static bool WarningEnabled
        {
            get { return _warningEnabled; }
            set { _warningEnabled = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [error enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [error enabled]; otherwise, <c>false</c>.
        /// </value>
        public static bool ErrorEnabled
        {
            get { return _errorEnabled; }
            set { _errorEnabled = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [debug enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [debug enabled]; otherwise, <c>false</c>.
        /// </value>
        public static bool DebugEnabled
        {
            get { return _debugEnabled; }
            set { _debugEnabled = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [suppress spam].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [suppress spam]; otherwise, <c>false</c>.
        /// </value>
        public static bool SuppressSpam
        {
            get { return _suppressSpam; }
            set { _suppressSpam = value; }
        }

        /// <summary>
        /// Gets or sets the messages expire time limit.
        /// Messages that have expired are removed from history.
        /// This property used in conjunction with SupressSpam = true prevents
        /// too much memory from being used over time
        /// </summary>
        /// <value>
        /// The messages expire time limit.
        /// </value>
        public static TimeSpan MessagesExpire
        {
            get { return _messagesExpire; }
            set { _messagesExpire = value; }
        }

        /// <summary>
        /// Sets the <see cref="ILogOutput"/>.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">output</exception>
        public static ILogOutput LogOutput
        {
            get { return _logOutput; }
            set
            {
                _logOutput = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [single threaded].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [single threaded]; otherwise, <c>false</c>.
        /// </value>
        public static bool SingleThreaded
        {
            get
            {
                return singleThreaded;
            }
            set
            {
                singleThreaded = value;
                if (singleThreaded) cancelToken.Cancel();
                else
                {
                    if (logTask.Status == TaskStatus.Running)
                        return;
                    cancelToken.Dispose();
                    cancelToken = new CancellationTokenSource();
                    logTask.Start();
                }
            }
        }

        /// <summary>
        /// Disables logging for the specified type.
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        public static void Disable<T>()
        {
            if (!disabledTypes.Contains(typeof(T)))
                disabledTypes.Add(typeof(T));
        }

        private static bool TypeCheck(Type type, out string typeName)
        {
            if (disabledTypes.Contains(type))
            {
                typeName = null;
                return false;
            }
            typeName = type.Name;
            return true;
        }

        private static void Log(LogMessage? msg)
        {
            if (msg == null) return;
            queue.Enqueue(msg.Value);
            if (singleThreaded)
            {
                Dump();
            }
        }

        private static void Dump()
        {
            if (queue.Count == 0) return;
            if (queue.TryDequeue(out LogMessage msg))
            {
                if (msg.IsSpam)
                {
                    SpamFound?.Invoke(msg);
                    if (SuppressSpam)
                        return;
                }
                switch (msg.Level)
                {
                    case Level.Debug:
                        if (!_debugEnabled)
                            return;
                        break;

                    case Level.Error:
                        if (!_errorEnabled)
                            return;
                        break;

                    case Level.Info:
                        if (!_infoEnabled)
                            return;
                        break;

                    case Level.Warning:
                        if (!_warningEnabled)
                            return;
                        break;
                }
                _logOutput.Log(msg);
            }
        }

        /// <summary>
        /// Asserts the specified cond.
        /// </summary>
        /// <param name="cond">if set to <c>true</c> [cond].</param>
        /// <param name="failMsg">The fail MSG.</param>
        /// <returns></returns>
        public static bool Assert(bool cond, string failMsg)
        {
            if (!cond)
            {
                Error("ASSERT: " + failMsg);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Asserts the specified cond.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cond">if set to <c>true</c> [cond].</param>
        /// <param name="failMsg">The fail MSG.</param>
        /// <returns></returns>
        public static bool Assert<T>(bool cond, string failMsg)
        {
            if (!cond)
            {
                Error<T>("ASSERT: " + failMsg);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Asserts the specified cond.
        /// </summary>
        /// <param name="cond">The cond.</param>
        /// <param name="failMsg">The fail MSG.</param>
        /// <returns></returns>
        public static bool Assert(Func<bool> cond, string failMsg)
        {
            if (!cond?.Invoke() ?? false)
            {
                Error("ASSERT: " + failMsg);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Asserts the specified cond.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cond">The cond.</param>
        /// <param name="failMsg">The fail MSG.</param>
        /// <returns></returns>
        public static bool Assert<T>(Func<bool> cond, string failMsg)
        {
            if (!cond?.Invoke() ?? false)
            {
                Error<T>("ASSERT: " + failMsg);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Failses the specified cond.
        /// </summary>
        /// <param name="cond">if set to <c>true</c> [cond].</param>
        /// <param name="failMsg">The fail MSG.</param>
        /// <returns></returns>
        public static bool Fails(bool cond, string failMsg)
        {
            if (cond)
            {
                Error("FAIL: " + failMsg);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Failses the specified cond.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cond">if set to <c>true</c> [cond].</param>
        /// <param name="failMsg">The fail MSG.</param>
        /// <returns></returns>
        public static bool Fails<T>(bool cond, string failMsg)
        {
            if (cond)
            {
                Error<T>("FAIL: " + failMsg);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Failses the specified cond.
        /// </summary>
        /// <param name="cond">The cond.</param>
        /// <param name="failMsg">The fail MSG.</param>
        /// <returns></returns>
        public static bool Fails(Func<bool> cond, string failMsg)
        {
            if (cond?.Invoke() ?? false)
            {
                Error("FAIL: " + failMsg);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Failses the specified cond.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cond">The cond.</param>
        /// <param name="failMsg">The fail MSG.</param>
        /// <returns></returns>
        public static bool Fails<T>(Func<bool> cond, string failMsg)
        {
            if (cond?.Invoke() ?? false)
            {
                Error<T>("FAIL: " + failMsg);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Debugs the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="memberName">Name of the member.</param>
        public static void Debug(object msg, [CallerMemberName]string memberName = "")
        {
            if (!DebugEnabled) return;
            Log(LogMessage.From(Level.Debug, $"System{(LogCallingMethod && !memberName.IsNullOrBlank() ? $" ({memberName})" : "")}: {(msg != null ? msg.ToString() : "null")}", MessagesExpire));
        }

        /// <summary>
        /// Debugs the specified MSG.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg">The MSG.</param>
        /// <param name="memberName">Name of the member.</param>
        public static void Debug<T>(object msg, [CallerMemberName]string memberName = "")
        {
            if (DebugEnabled && TypeCheck(typeof(T), out string typeName))
                Log(LogMessage.From(Level.Debug, $"{typeName}{(LogCallingMethod && !memberName.IsNullOrBlank() ? $" ({memberName})" : "")}: {msg}", MessagesExpire));
        }

        /// <summary>
        /// Informations the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="memberName">Name of the member.</param>
        public static void Info(object msg, [CallerMemberName]string memberName = "")
        {
            if (!InfoEnabled) return;
            Log(LogMessage.From(Level.Info, $"System{(LogCallingMethod && !memberName.IsNullOrBlank() ? $" ({memberName})" : "")}: {(msg != null ? msg.ToString() : "null")}", MessagesExpire));
        }

        /// <summary>
        /// Informations the specified MSG.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg">The MSG.</param>
        /// <param name="memberName">Name of the member.</param>
        public static void Info<T>(object msg, [CallerMemberName]string memberName = "")
        {
            if (InfoEnabled && TypeCheck(typeof(T), out string typeName))
                Log(LogMessage.From(Level.Info, $"{typeName}{(LogCallingMethod && !memberName.IsNullOrBlank() ? $" ({memberName})" : "")}: {msg}", MessagesExpire));
        }

        /// <summary>
        /// Errors the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="memberName">Name of the member.</param>
        public static void Error(object msg, [CallerMemberName]string memberName = "")
        {
            if (!ErrorEnabled) return;
            Log(LogMessage.From(Level.Error, $"System{(LogCallingMethod && !memberName.IsNullOrBlank() ? $" ({memberName})" : "")}: {(msg != null ? msg.ToString() : "null")}", MessagesExpire));
        }

        /// <summary>
        /// Errors the specified MSG.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg">The MSG.</param>
        /// <param name="memberName">Name of the member.</param>
        public static void Error<T>(object msg, [CallerMemberName]string memberName = "")
        {
            if (ErrorEnabled && TypeCheck(typeof(T), out string typeName))
                Log(LogMessage.From(Level.Error, $"{typeName}{(LogCallingMethod && !memberName.IsNullOrBlank() ? $" ({memberName})" : "")}: {msg}", MessagesExpire));
        }

        /// <summary>
        /// Warns the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="memberName">Name of the member.</param>
        public static void Warn(object msg, [CallerMemberName]string memberName = "")
        {
            if (!WarningEnabled) return;
            Log(LogMessage.From(Level.Warning, $"System{(LogCallingMethod && !memberName.IsNullOrBlank() ? $" ({memberName})" : "")}: {(msg != null ? msg.ToString() : "null")}", MessagesExpire));
        }

        /// <summary>
        /// Warns the specified MSG.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg">The MSG.</param>
        /// <param name="memberName">Name of the member.</param>
        public static void Warn<T>(object msg, [CallerMemberName]string memberName = "")
        {
            if (WarningEnabled && TypeCheck(typeof(T), out string typeName))
                Log(LogMessage.From(Level.Warning, $"{typeName}{(LogCallingMethod && !memberName.IsNullOrBlank() ? $" ({memberName})" : "")}: {msg}", MessagesExpire));
        }
    }
}