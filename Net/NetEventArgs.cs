using System;

namespace Furcadia.Net
{
    /// <summary>
    /// Connection Status
    /// </summary>
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
        /// Spawns a new Furre in the dream
        /// </summary>
        /// <remarks>
        /// Prefix "&gt;"
        /// <para>
        /// </para>
        /// </remarks>
        SpawnAvatar,

        /// <summary>
        /// </summary>
        RemoveAvatar,

        /// <summary>
        /// </summary>
        AnimatedMoveAvatar,

        /// <summary>
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
        DisplayText
    }

    /// <summary>
    /// Client Status Event Arguments.
    /// </summary>
    public class NetClientEventArgs : EventArgs
    {
        #region Public Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public NetClientEventArgs()
        {
            ConnectPhase = ConnectionPhase.error;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">
        /// Optional Message
        /// </param>
        /// <param name="phase">
        /// Connection Phase
        /// </param>
        public NetClientEventArgs(ConnectionPhase phase, string message = null)
        {
            ConnectPhase = phase;
            Message = message;
        }

        #endregion Public Constructors

        #region Public Fields

        /// <summary>
        /// Status of the Furcadia Client Connection
        /// </summary>
        public ConnectionPhase ConnectPhase;

        /// <summary>
        /// optional string message
        /// </summary>
        public string Message;

        #endregion Public Fields
    }

    /// <summary>
    /// Server Status Event Arguments
    /// </summary>
    public class NetServerEventArgs : EventArgs
    {
        #region Public Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="phase">
        /// Server Connection Phase
        /// </param>
        /// <param name="Instruction">
        /// Current Server to Client Instruction Type
        /// </param>
        public NetServerEventArgs(ConnectionPhase phase, ServerInstructionType Instruction)
        {
            ConnectPhase = phase;
            serverinstruction = Instruction;
        }

        /// <summary>
        /// default Constructor
        /// </summary>
        public NetServerEventArgs()
        {
            serverinstruction = ServerInstructionType.Unknown;
            ConnectPhase = ConnectionPhase.error;
        }

        /// <summary>
        /// Server to Client instructions
        /// </summary>
        public ServerInstructionType ServerInstruction
        {
            get { return serverinstruction; }
            set { serverinstruction = value; }
        }

        #endregion Public Constructors

        #region Public Fields

        /// <summary>
        /// Status of the Server Connection
        /// </summary>
        public ConnectionPhase ConnectPhase;

        private ServerInstructionType serverinstruction;

        #endregion Public Fields
    }

    /// <summary>
    /// Parse Server Instruction set
    /// </summary>
    public class ParseServerArgs : EventArgs
    {
        #region Private Fields

        private ServerInstructionType serverinstruction;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Server to Client instructions
        /// </summary>
        public ServerInstructionType ServerInstruction
        {
            get { return serverinstruction; }
            set { serverinstruction = value; }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// </summary>
        public ParseServerArgs()
        {
            serverinstruction = ServerInstructionType.Unknown;
        }

        /// <summary>
        /// </summary>
        public ParseServerArgs(ServerInstructionType ServerInstruction)
        {
            serverinstruction = ServerInstruction;
        }

        #endregion Public Constructors
    }
}