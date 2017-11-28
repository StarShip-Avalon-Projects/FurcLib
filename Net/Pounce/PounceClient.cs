using System;
using System.Collections.Generic;
using System.Threading;

namespace Furcadia.Net.Pounce
{
    /// <summary>
    /// Generic Class to use the Pounce server
    /// </summary>
    /// TODO: Read default lists(Furres, Dreams, Channels) from Furcadia Online.ini
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
                        AddFriend(Furre.ShortName);
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
                    PounceTimer.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Implement IDisposable and Dispose of PounceTimer
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }

    #endregion Private Methods
}