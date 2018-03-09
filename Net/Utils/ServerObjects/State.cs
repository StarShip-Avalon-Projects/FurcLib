using System.Net.Sockets;

namespace Furcadia.Net.Utils.ServerObjects
{
    /// <summary>
    /// State object for <see cref="TcpClient.EndConnect(System.IAsyncResult)"/>
    /// </summary>
    internal class State
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        public State()
        {
            Success = true;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public TcpClient Client { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="State"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        #endregion Public Properties
    }
}