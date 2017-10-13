using System;
using System.Collections.Generic;
using System.Threading;
using static Furcadia.Util;

namespace Furcadia.Net.Pounce
{
    /// <summary>
    /// Generic Class to use the Pounce server
    /// <para>
    /// TODO: Read default lists(Furres, Dreams, Channels) from Furcadia Online.ini
    /// </para>
    /// </summary>
    public class PounceClient : PounceConnection, IDisposable
    {
        #region Public Constructors

        /// <summary>
        /// Default Constructor
        /// <para>
        /// Pounce server updates on a 30 second cron-job and returns a list
        /// of Furres Currently on-line
        /// </para>
        /// </summary>
        public PounceClient() : base("http://on.furcadia.com/q/", null, null)
        {
            PounceTimer = new Timer(SmPounceSend, this,
                TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Set up the default Pounce Client and Friends List and Dream List
        /// <para>
        /// Pounce server updates on a 30 second cron-job and returns a list
        /// of Furres Currently on-line
        /// </para>
        /// </summary>
        /// <param name="FurreList">
        /// Furre List as string array
        /// </param>
        /// <param name="DreamList">
        /// dream list as s string array
        /// </param>
        public PounceClient(string[] FurreList, string[] DreamList) : base("http://on.furcadia.com/q/", FurreList, DreamList)
        {
            PounceTimer = new Timer(SmPounceSend, this,
    TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        #endregion Public Constructors

        #region Private Fields

        private Dictionary<string, PounceFurre> _furreList = new Dictionary<string, PounceFurre>();

        // private string _onlineList;
        //  private System.DateTime lastaccess;

        /// <summary>
        /// 30 second timer to send requests to the pounce server
        /// </summary>
        private Timer PounceTimer;

        private object PounceLock = new object();

        #endregion Private Fields

        #region Public Properties

        private string onlinelist;

        /// <summary>
        /// </summary>
        public List<PounceFurre> FurreList { get; private set; }

        /// <summary>
        /// File path to List of furres to check online status
        /// </summary>
        public string OnlineList
        {
            get { return onlinelist; }
            set { onlinelist = value; }
        }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Send request to Pounce server at 30 second interval
        /// </summary>
        /// <param name="sender">
        /// </param>
        private void SmPounceSend(object sender)
        {
            lock (PounceLock)
            {
                ClearFriends();
                foreach (PounceFurre Furre in FurreList)
                {
                    if (!string.IsNullOrEmpty(Furre.Name))
                    {
                        AddFriend(FurcadiaShortName(Furre.Name));
                    }
                }
                ConnectAsync();
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        ///
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    PounceTimer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PounceClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// Implement IDisposable and Dispose of PounceTimer
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }

    #endregion Private Methods

    #region Public Classes

    /// <summary>
    /// </summary>
    public class PounceFurre
    {
        #region "Public Fields"

        private bool online;

        private bool wasOnline;

        /// <summary>
        /// Furre Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Furre Currently online
        /// </summary>
        public bool Online
        {
            get { return online; }
            set { online = value; }
        }

        /// <summary>
        /// Furre Previous Online State
        /// </summary>
        public bool WasOnline
        {
            get { return wasOnline; }
            set { wasOnline = value; }
        }

        #endregion "Public Fields"
    }

    #endregion Public Classes
}