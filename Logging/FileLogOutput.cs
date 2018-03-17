using Furcadia.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Furcadia.Logging
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="Furcadia.Logging.ILogOutput" />
    public class FileLogOutput : ILogOutput, IEquatable<FileLogOutput>
    {
        private readonly Level level;
        private readonly string filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogOutput"/> class.
        /// </summary>
        /// <param name="rootFolder">The root folder.</param>
        /// <param name="level">The level.</param>
        public FileLogOutput(string rootFolder, Level level = Level.Error)
        {
            if (Assembly.GetEntryAssembly() != null)
                filePath = Path.Combine(rootFolder, $"{Assembly.GetEntryAssembly().GetName().Name}.{level}.log");
            else if (Assembly.GetCallingAssembly() != null)
                filePath = Path.Combine(rootFolder, $"{Assembly.GetCallingAssembly().GetName().Name}.{level}.log");
            if (!Directory.Exists(Path.GetDirectoryName(filePath))) Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            if (File.Exists(filePath)) File.WriteAllText(filePath, ""); // make sure it is a clean file
            this.level = level;
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
            return Equals(obj as FileLogOutput);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(FileLogOutput other)
        {
            return other != null &&
                   level == other.level &&
                   filePath == other.filePath;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = 1789646697;
            hashCode = hashCode * -1521134295 + level.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(filePath);
            return hashCode;
        }

        /// <summary>
        /// Logs the specified log message to the file.
        /// </summary>
        /// <param name="logMsg">The log MSG.</param>
        public void Log(LogMessage logMsg)
        {
            if (logMsg.Level != level) return;
            logMsg = BuildMessage(ref logMsg);
            using (var mutex = new Mutex(false, GetType().Name))
            {
                if (mutex.WaitOne())
                    try
                    {
                        using (FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Write, 4096))
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.WriteLine(logMsg.message);
                        }
                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }
            }
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
    }
}