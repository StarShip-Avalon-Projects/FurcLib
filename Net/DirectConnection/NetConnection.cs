using Furcadia.Net.Options;
using Furcadia.Net.Utils;
using System;
using System.Net;

namespace Furcadia.Net.DirectConnection
{
    /// <summary>
    /// Direct Furcadia game server connection with included server load balancer.
    /// </summary>
    public class NetConnection : ClientBase
    {
        #region Private Fields

        /// <summary>
        /// Message to server load balancing.
        /// </summary>
        private ServerQue ServerBalancer;

        private ClientOptions options;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public NetConnection() : base()
        {
            ServerBalancer = new ServerQue();
            ServerBalancer.OnServerSendMessage += MessageSentToServer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetConnection"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public NetConnection(ClientOptions options)
        {
            ServerBalancer = new ServerQue();
            ServerBalancer.OnServerSendMessage += MessageSentToServer;
            this.options = options;
        }

        /// <summary>
        /// Connect to game server with Host DNS and Specified port
        /// </summary>
        /// <param name="host">
        /// Game Server Host name
        /// </param>
        /// <param name="port">
        /// Game server TCP Port
        /// </param>
        public NetConnection(string host, int port) : base(host, port)
        {
            ServerBalancer = new ServerQue();
            ServerBalancer.OnServerSendMessage += MessageSentToServer;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Connect to game server
        /// </summary>
        public override void Connect()
        {
            base.Connect();
        }

        /// <summary>
        /// Send a message to the Game Server
        /// </summary>
        /// <param name="message">
        /// </param>
        public virtual void SendServer(string message)
        {
            ServerBalancer.SendToServer(message);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Let the Server Balancer control the message load to the server
        /// </summary>
        /// <param name="message">
        /// Message from the Server Queue
        /// </param>
        /// <param name="e">
        /// event Arguments
        /// </param>
        private void MessageSentToServer(object message, EventArgs e)
        {
            SendServer((string)message);
        }

        #endregion Private Methods
    }
}