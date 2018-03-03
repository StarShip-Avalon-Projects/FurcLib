using System;
using static Furcadia.Text.FurcadiaMarkup;

namespace Furcadia.Net
{
    /// <summary>
    /// Connection Status
    /// </summary>
    /// <remarks>
    /// Credit to Artex for his open source projects use this method
    /// <para>
    /// Reference http://dev.furcadia.com/docs/027_movement.html
    /// </para>
    /// </remarks>
    [CLSCompliant(true)]
    public enum ConnectionPhase
    {
        /// <summary>
        /// Default Error
        /// <para>
        /// Halt Game Server and Client Connection Procedure
        /// </para>
        /// </summary>
        error = -1,

        /// <summary>
        /// Initialize Connection
        /// </summary>
        Init,

        /// <summary>
        /// Connection started
        /// </summary>
        Connecting,

        /// <summary>
        /// Message of the Day
        /// <para>
        /// IE: Good Morning Dave...
        /// </para>
        /// </summary>
        MOTD,

        /// <summary>
        /// Login Account,Password, Character Name
        /// </summary>
        Auth,

        /// <summary>
        /// Connection established
        /// </summary>
        Connected,

        /// <summary>
        /// Connection lost
        /// </summary>
        Disconnected,
    }

    /// <summary>
    /// Server to Client Instruction set. (Furcadia v31c)
    /// <para>
    /// This is the set that FF3PP understands and uses.
    /// </para>
    /// <para>
    /// these can change with each Furcadia update.
    /// </para>
    /// </summary>
    public enum ServerInstructionType
    {
        /// <summary>
        /// Unknown Instruction,
        /// <para>
        /// Needs further research
        /// </para>
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// No instruction Nessary
        /// </summary>
        None,

        /// <summary>
        /// Spawns a new Furre in the dream furre list because they have
        /// joing the Dream we're in
        /// <para/>
        /// '&lt;' + user id + x + y + shape number + name + color code +
        /// flag + linefeed
        /// <para/>
        /// sender object is Type SpawnAvatar()
        /// </summary>
        SpawnAvatar,

        /// <summary>
        /// Remove the Avatar from the Dream Furre list because they have
        /// left the dream
        /// <para/>
        /// ')' + user id + linefeed
        /// <para/>
        /// Source: Furcatia Technical Resources
        /// <para/>
        /// sender object is Type RemoveAvatar()
        /// </summary>
        RemoveAvatar,

        /// <summary>
        /// Hide Avatar from display (Invisible?)
        /// </summary>
        HideAvatar,

        /// <summary>
        /// Move and animate the Active Furre to the next location
        /// <para/>
        /// sender object is Type Furre()
        /// </summary>
        AnimatedMoveAvatar,

        /// <summary>
        /// Move the current active furre to the next locatiomn
        /// <para/>
        /// sender object is Type Furre
        /// </summary>
        MoveAvatar,

        /// <summary>
        /// Display formated Text.
        /// <para>
        /// Mostly Furcadia Markup but other stuff too
        /// </para>
        /// </summary>
        /// <remarks>
        /// Prefix "("
        /// <para>
        /// This instruction displays the specific text in the user's
        /// chat-box. The data may be formatted with HTML-equivalent and
        /// Furcadia-specific tags, as well as emoticons (stuff like "#SA").
        /// </para>
        /// </remarks>
        DisplayText,

        /// <summary>
        /// Update the Triggering Furre ColorCode
        /// <para>
        /// 'B' + user id + shape + color code + linefeed
        /// </para>
        /// </summary>
        UpdateColorString,

        /// <summary>
        /// Download Dream Data
        /// <para>
        /// IE: ]q pmnaiagreen 3318793420 modern
        /// </para>
        /// <para>
        /// respond with client command when furcadia client is not
        /// available "vasecodegamma"
        /// </para>
        /// </summary>
        LoadDreamEvent,

        /// <summary>
        /// Unique User ID
        ///  <para/>]z UID[*]
        /// <para/>This instruction is sent as a response to the uid command. The purpose of this is unclear.
        ///  <para/> Credits Artex, FTR
        /// </summary>
        UniqueUserId,

        /// <summary>
        /// Set Own ID
        /// <para/> ]BUserID[*]
        /// <para/>This instruction informs the client of which user-name is it logged into. Knowing your
        /// own UserID can help you find your own avatar within the dream.
        /// <para/>Credits Artex, FTR
        /// </summary>
        SetOwnId,

        /// <summary>
        ///  Dream Book Mark
        ///  <para/>Triggers Pounce to add the dream to the list marked temporary
        /// </summary>
        BookmarkDream,

        /// <summary>
        /// received after a look at furre command
        /// <para/>
        /// sender object is Type Furre
        /// </summary>
        LookResponse,

        /// <summary>
        /// Entering a new dream
        /// </summary>
        EnterDream
    }

    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="Furcadia.Net.NetServerEventArgs" />
    public class NetChannelEventArgs : NetServerEventArgs
    {
        #region Private Fields

        private string channel;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        public NetChannelEventArgs() : base(ConnectionPhase.Connected, ServerInstructionType.DisplayText)
        {
            channel = "Unknown";
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Server Text Channel
        /// </summary>
        public string Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        #endregion Public Properties
    }

    /// <summary>
    /// Client Status Event Arguments.
    /// </summary>
    [Serializable]
    public class NetClientEventArgs : EventArgs
    {
        #region Private Fields

        private string message;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Default Constructor <see cref="ConnectionPhase.error"/>
        /// </summary>
        public NetClientEventArgs()
        {
            ConnectPhase = ConnectionPhase.error;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientdata">
        /// Optional Message
        /// </param>
        /// <param name="phase">
        /// Connection Phase
        /// </param>
        public NetClientEventArgs(ConnectionPhase phase, string clientdata = null)
        {
            ConnectPhase = phase;
            message = clientdata;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// optional string message
        /// </summary>
        public string ClientData
        {
            get
            {
                return message;
            }
            private set
            {
                message = value;
            }
        }

        /// <summary>
        /// Status of the Furcadia Client Connection
        /// </summary>
        public ConnectionPhase ConnectPhase
        { get; private set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Game Server Status Event Arguments
    /// </summary>
    [Serializable]
    public class NetServerEventArgs : EventArgs
    {
        #region Public Fields

        /// <summary>
        /// Status of the Server Connection
        /// </summary>
        public ConnectionPhase ConnectPhase;

        #endregion Public Fields

        #region Private Fields

        private ServerInstructionType serverinstruction;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Game Server Status Event Arguments
        /// </summary>
        /// <param name="phase">
        /// Server <see cref="ConnectionPhase"/>
        /// </param>
        /// <param name="Instruction">
        /// Game <see cref="ServerInstructionType"/> to client
        /// </param>
        public NetServerEventArgs(ConnectionPhase phase, ServerInstructionType Instruction)
        {
            ConnectPhase = phase;
            serverinstruction = Instruction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetServerEventArgs"/> class.
        /// </summary>
        public NetServerEventArgs()
        {
            serverinstruction = ServerInstructionType.Unknown;
            ConnectPhase = ConnectionPhase.error;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Server to Client instructions
        /// </summary>
        public ServerInstructionType ServerInstruction
        {
            get => serverinstruction;
            set => serverinstruction = value;
        }

        #endregion Public Properties
    }

    /// <summary>
    /// Parse Server Instruction set
    /// </summary>
    [Serializable]
    public class ParseChannelArgs : ParseServerArgs
    {
        #region Private Fields

        private string channel;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseChannelArgs"/> class.
        /// </summary>
        /// <param name="ServerInstruction">The server instruction.</param>
        public ParseChannelArgs(string ServerInstruction) : base(ServerInstructionType.DisplayText, ConnectionPhase.Connected)
        {
            var m = ChannelRegex.Match(ServerInstruction);
            channel = m.Groups[1].Value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseChannelArgs"/> class.
        /// </summary>
        public ParseChannelArgs() : base(ServerInstructionType.DisplayText, ConnectionPhase.Connected)
        {
            channel = "Unknown";
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Server Text Channel
        /// </summary>
        public string Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        #endregion Public Properties
    }

    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    [Serializable]
    public class ParseServerArgs : EventArgs
    {
        #region Private Fields

        private string message;

        private ConnectionPhase serverConnectedPhase;

        private ServerInstructionType serverinstruction;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Default Constructor <see cref="ServerInstructionType.Unknown"/>
        /// because we don't know wich one it is yet
        /// </summary>
        public ParseServerArgs()
        {
            serverinstruction = ServerInstructionType.Unknown;
            serverConnectedPhase = ConnectionPhase.error;
        }

        /// <summary>
        /// Constructor setting the current Server to Client Instruction type
        /// </summary>
        /// <param name="ServerInstruction">
        /// Current Execuring <see cref="ServerInstructionType"/>
        /// </param>
        /// <param name="phase">
        /// </param>
        public ParseServerArgs(ServerInstructionType ServerInstruction, ConnectionPhase phase)
        {
            serverinstruction = ServerInstruction;
            serverConnectedPhase = phase;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// </summary>
        public ConnectionPhase ServerConnectedPhase
        {
            get { return serverConnectedPhase; }
        }

        /// <summary>
        /// optional string message
        /// </summary>
        public string ServerData
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }

        /// <summary>
        /// Server to Client Instruction Type
        /// </summary>
        public ServerInstructionType ServerInstruction
        {
            get { return serverinstruction; }
            set { serverinstruction = value; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Instruction '{serverinstruction}' : Phase: '{serverConnectedPhase}'";
        }

        #endregion Public Methods
    }
}