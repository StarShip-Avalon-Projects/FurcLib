/*Log Header
 *Format: (date,Version) AuthorName, Changes.
 * (March 2017, 0.1) Gerolkae, Initial creation
 */

using System;
using System.Collections.Generic;
using System.Threading;

namespace Furcadia.Net.Utils
{
    /// <summary>
    /// Balance the load to the server
    /// <para>
    /// Handles Throat-Tired and No Endurance
    /// </para>
    /// </summary>
    public class ServerQue : IDisposable
    {
        #region Private Fields

        private int pingdelaytime;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Is the connect `noendurance enabled?
        /// </summary>
        public bool NoEndurance
        {
            get { return noendurance; }
            set { noendurance = value; }
        }

        /// <summary>
        /// Ping the server Time in Seconds
        /// </summary>
        public int PingDelayTime
        {
            get { return pingdelaytime; }
            set
            {
                pingdelaytime = value;
                NewPingTimer(value);
            }
        }

        /// <summary>
        /// If Proxy get "Your throat is tired" Pause for a number of seconds
        /// </summary>
        public bool ThroatTired
        {
            get { return throattired; }
            set
            {
                throattired = value;
                TroatTiredDelay = new Timer(TroatTiredDelayTick, null,
                    TimeSpan.Zero, TimeSpan.FromSeconds(throattireddelaytime));
            }
        }

        /// <summary>
        /// When "Your throat is tired appears, Pause processing of client
        /// to server instructions,
        /// </summary>
        public int ThroatTiredDelayTime
        {
            get { return throattireddelaytime; }
            set { throattireddelaytime = value; }
        }

        /// <summary>
        /// Set the Ping timer
        /// </summary>
        /// <param name="DelayTime">
        /// Delay Time in Seconds
        /// </param>
        private void NewPingTimer(int DelayTime)
        {
            if (DelayTime > 0)
                PingTimer = new Timer(PingTimerTick, null,
                    TimeSpan.FromSeconds(DelayTime), TimeSpan.FromSeconds(DelayTime));
            else if (PingTimer != null)
                PingTimer.Dispose();
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Constructor setting Defaults
        /// </summary>
        public ServerQue()
        {
            throattireddelaytime = 45;
            QueueTimer = new System.Threading.Timer(ProcessQueue, null, 0, 200);
            PingTimer = new Timer(PingTimerTick, null,
                TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// </summary>
        /// <param name="ThroatTiredTime">
        /// Delay time to pause for Throat Tired Syndrome
        /// </param>
        /// <param name="PingTimerTime">
        /// Optional Server Ping timme in seconds
        /// </param>
        public ServerQue(int ThroatTiredTime, int PingTimerTime = 30)
        {
            throattireddelaytime = ThroatTiredTime;
            QueueTimer = new System.Threading.Timer(ProcessQueue, null, 0, 75);
            NewPingTimer(PingTimerTime);
        }

        #endregion Public Constructors

        #region Internal Fields

        internal const int MASS_NOENDURANCE = 2048;

        #endregion Internal Fields

        #region Private Fields

        private const int MASS_CRITICAL = 2046;
        private const int MASS_DECAYPS = 400;
        private const int MASS_DEFAULT = 80;
        private const int MASS_SPEECH = 1000;
        private double g_mass = 0;

        private System.Threading.Timer PingTimer;

        /// <summary>
        /// Queue Processing timer.
        /// </summary>
        private System.Threading.Timer QueueTimer;

        /// <summary>
        /// FIFO Stack of Server Instructions to process
        /// </summary>
        private Queue<string> ServerStack = new Queue<string>(500);

        /// <summary>
        /// How long to wait till we resume processing the ServerStack?
        /// </summary>
        private System.Threading.Timer TroatTiredDelay;

        #endregion Private Fields

        #region Public Delegates

        /// <summary>
        /// Event Handler to notify calling class data has been sent to the
        /// game server
        /// </summary>
        /// <param name="message">
        /// raw client to server instruction
        /// </param>
        /// <param name="args">
        /// System.EventArgs. (Unused)
        /// </param>
        public delegate void SendServerEventHandler(string message, System.EventArgs args);

        #endregion Public Delegates

        #region Connection Timers

        /// <summary>
        /// NoEndurance. Send data at the speed of the server
        /// </summary>
        private bool noendurance;

        private object QueLock = new object();

        /// <summary>
        /// Throat Tired System.
        /// <para>
        /// Pause sending data to server if we get a message we tried to
        /// send too much at one time
        /// </para>
        /// </summary>
        private bool throattired;

        /// <summary>
        /// Throat tired delay in seconds
        /// </summary>
        private int throattireddelaytime;

        private DateTime TickTime = DateTime.Now;

        /// <summary>
        /// ping interlock exchange
        /// </summary>
        private int usingPing = 0;

#pragma warning disable CS0414 // The field 'ServerQue.usingProcessQueue' is assigned but its value is never used

        /// <summary>
        /// Queue processing <see cref=" Interlocked.Exchange(ref int, int)"/>
        /// </summary>
        private int usingProcessQueue = 0;

#pragma warning restore CS0414 // The field 'ServerQue.usingProcessQueue' is assigned but its value is never used

        /// <summary>
        /// Incoming Messages for server processing
        /// </summary>
        /// <param name="data">
        /// Raw Client to Server Instruction.
        /// </param>
        public void SendToServer(ref string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            // if (string.IsNullOrEmpty(data)) return;
            ServerStack.Enqueue(data);
            if (g_mass + MASS_SPEECH <= MASS_CRITICAL)
            {
                double t = 0;
                QueueTick(ref t);
            }
        }

        /// <summary>
        /// Ping the server with a random packet to maintain connection
        /// </summary>
        /// <param name="state">
        /// </param>
        private void PingTimerTick(object state)
        {
            //if ((0 == Interlocked.Exchange(ref usingPing, 1)))
            //{
            if (g_mass + MASS_SPEECH <= MASS_CRITICAL)
            {
                ServerStack.Enqueue("Ping");
            }
            Interlocked.Exchange(ref usingPing, 0);
            //}
        }

        /// <summary>
        /// </summary>
        /// <param name="state">
        /// </param>
        private void ProcessQueue(object state)
        {
            //if (0 == Interlocked.Exchange(ref usingProcessQueue, 1))
            //{
            double seconds = DateTime.Now.Subtract(TickTime).Milliseconds;
            QueueTick(ref seconds);
            TickTime = DateTime.Now;
            //    Interlocked.Exchange(ref usingProcessQueue, 0);
            //}
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
        private void QueueTick(ref double DelayTime)
        {
            lock (QueLock)
            {
                if (ServerStack.Count == 0)
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

                if (noendurance)
                {
                    /* just send everything right away */
                    while (ServerStack.Count > 0 & g_mass <= MASS_CRITICAL)
                    {
                        g_mass += ServerStack.Peek().Length + MASS_DEFAULT;
                        OnServerSendMessage?.Invoke(ServerStack.Dequeue(), System.EventArgs.Empty);
                    }
                }
                else if (!ThroatTired)
                {
                    // Only send a speech line if the mass will be under the
                    // limit. */
                    while (ServerStack.Count > 0 & g_mass + MASS_SPEECH <= MASS_CRITICAL)
                    {
                        g_mass += ServerStack.Peek().Length + MASS_DEFAULT;
                        OnServerSendMessage?.Invoke(ServerStack.Dequeue(), System.EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Throat Tired Delay function
        /// </summary>
        /// <param name="state">
        /// </param>
        private void TroatTiredDelayTick(object state)
        {
            ThroatTired = false;
            TroatTiredDelay.Dispose();
        }

        #endregion Connection Timers

        #region Protected Methods

        /// <summary>
        /// Notify subscribers were's sending an instruction to the games server
        /// </summary>
        public EventHandler OnServerSendMessage;

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
            // TODO: uncomment the following line if the finalizer is
            //       overridden above. GC.SuppressFinalize(this);
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
                    // TODO: dispose managed state (managed objects).
                    if (TroatTiredDelay != null)
                        TroatTiredDelay.Dispose();
                    if (PingTimer != null)
                        PingTimer.Dispose();
                    if (QueueTimer != null)
                        QueueTimer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and
                //       override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above
        //       has code to free unmanaged resources. ~ServerQue() { // Do
        //       not change this code. Put cleanup code in Dispose(bool
        // disposing) above. Dispose(false); }

        #endregion IDisposable Support

        #endregion Protected Methods
    }
}