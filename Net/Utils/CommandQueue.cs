/*Log Header
 *Format: (date,Version) AuthorName, Changes.
 * (March 2017, 0.1) Gerolkae, Initial creation
 */

using System;
using System.Collections.Generic;

namespace Furcadia.Net.Utils
{
    /// <summary>
    /// Balance the load to the server
    /// <para>
    /// Handles Throat-Tired and No Endurance
    /// </para>
    /// </summary>
    public class CommandQueueManager : IDisposable
    {
        #region Private Fields

        private const int MASS_CRITICAL = 1024;
        private const int MASS_DECAYPS = 300;
        private const int MASS_DEFAULT = 120;
        private const int MASS_SPEECH = 1000;
        private double g_mass = 0;

        private bool pause;

        /// <summary>
        /// Queue Processing timer.
        /// </summary>
        private System.Threading.Timer QueueTimer;

        /// <summary>
        /// FIFO Stack of Server Instructions to process
        /// </summary>
        private Queue<string> CommandQueue = new Queue<string>(500);

        private object QueLock = new object();

        private DateTime TickTime = DateTime.Now;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Constructor setting Defaults
        /// </summary>
        public CommandQueueManager()
        {
            QueueTimer = new System.Threading.Timer(ProcessQueue, null, 0, 200);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Is the Queue Paused?
        /// </summary>
        public bool Pause
        {
            get { return pause; }
            set { pause = value; }
        }

        #endregion Public Properties

        #region Public Delegates

        /// <summary>
        /// Event Handler to notify calling class data has been sent to the
        /// game server
        /// </summary>
        /// <param name="sender">
        /// raw client to server instruction
        /// </param>
        /// <param name="args">
        /// System.EventArgs. (Unused)
        /// </param>
        public delegate void QueueEventHandler(object sender, System.EventArgs args);

        /// <summary>
        /// Notify subscribers were's sending an instruction to the games server
        /// </summary>
        public QueueEventHandler OnServerSendMessage;

        #endregion Public Delegates

        #region Connection Timers

        /// <summary>
        /// Incoming Messages for server processing
        /// </summary>
        /// <param name="data">
        /// Raw Client to Server Instruction.
        /// </param>
        public virtual void SendQueueItem(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            // if (string.IsNullOrEmpty(data)) return;
            CommandQueue.Enqueue(data);

            if (g_mass + MASS_SPEECH <= MASS_CRITICAL)
            {
                g_mass += MASS_CRITICAL;
                double t = 0;
                QueueTick(t);
            }
        }

        /// <summary>
        /// handle the Queue timer ticks
        /// </summary>
        /// <param name="state">
        /// </param>
        private void ProcessQueue(object state)
        {
            double seconds = DateTime.Now.Subtract(TickTime).Milliseconds;
            QueueTick(seconds);
            TickTime = DateTime.Now;
        }

        /// <summary>
        /// Load Balancing Function
        /// <para>
        /// this makes sure we don't over load what the server can handle
        /// </para>
        /// <para>
        /// Proxy has 2 modes of operation
        /// </para>
        /// <para>
        /// Mode 1 Normal. handles Throat Tired syndrome with a time out
        /// timer to resume
        /// </para>
        /// <para>
        /// Mode 2 NoEndurance. Send data to server as fast as it can handle
        /// with out overloading its buffer
        /// </para>
        /// </summary>
        /// <param name="DelayTime">
        /// Delay Time in Milliseconds
        /// </param>
        private void QueueTick(double DelayTime)
        {
            lock (QueLock)
            {
                if (!pause)
                    return;
                if (CommandQueue.Count == 0)
                    return;
                if (DelayTime != 0)
                {
                    DelayTime = Math.Round(DelayTime, 0) + 1;
                }

                /* Send buffered speech. */
                double decay = Math.Round(DelayTime * MASS_DECAYPS / 1000f, 0);
                if ((decay > g_mass))
                {
                    g_mass = 0;
                }
                else
                {
                    g_mass -= decay;
                }

                // Only send a speech line if the mass will be under the
                // limit. */
                while (CommandQueue.Count > 0 & g_mass + MASS_SPEECH <= MASS_CRITICAL)
                {
                    g_mass += CommandQueue.Peek().Length + MASS_DEFAULT;
                    OnServerSendMessage?.Invoke(CommandQueue.Dequeue(), System.EventArgs.Empty);
                }
            }
        }

        #endregion Connection Timers

        #region Protected Methods

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// This code added to correctly implement the disposable pattern.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool
            // disposing) above.
            Dispose(true);
        }

        /// <summary>
        /// </summary>
        /// <param name="disposing">
        /// </param>
        public virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (QueueTimer != null)
                        QueueTimer.Dispose();
                }

                disposedValue = true;
            }
        }

        #endregion IDisposable Support

        #endregion Protected Methods
    }
}