﻿using Furcadia.Drawing;
using Furcadia.Logging;
using Furcadia.Movement;
using Furcadia.Net.DreamInfo;
using Furcadia.Net.Options;
using Furcadia.Net.Utils;
using Furcadia.Net.Utils.ChannelObjects;
using Furcadia.Net.Utils.ServerObjects;
using Furcadia.Net.Utils.ServerParser;
using Furcadia.Text;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Furcadia.Movement.CharacterFlags;
using static Furcadia.Net.DirectConnection.ClientBase;
using static Furcadia.Text.FurcadiaMarkup;

namespace Furcadia.Net.DirectConnection
{
    /// <summary>
    /// This Instance handles the current Furcadia Connection Session. For Proxy <see cref="Proxy.ProxySession"/>
    /// <para>
    /// Part2: Furcadia connection Controls, In/Out Ports, Host, Character Ini
    ///        file. Connect, Disconnect, Reconnect
    /// </para>
    /// <para>
    /// Part3: This Class Links loosely to the GUI
    /// </para>
    /// </summary>
    public class NetConnection : IDisposable
    {
        #region Public Fields

        /// <summary>
        /// </summary>
        public Queue<Rep> Repq = new Queue<Rep>();

        /// <summary>
        /// The troat tired event handler
        /// </summary>
        public ThroatTiredEnabled TroatTiredEventHandler;

        #endregion Public Fields

        #region Private Fields

        ///// <summary>
        ///// Beekin Badge
        ///// </summary>
        private Queue<string> BadgeTag;

        private object ChannelLock = new object();
        private RegexOptions ChannelOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;
        private ClientBase Client;

        private ClientOptions clientOptions;

        private bool disposed = false;
        private bool Look;

        private Queue<string> LookQue;

        /// <summary>
        /// Balance the out going load to server
        /// <para>
        /// Throat Tired Syndrome and No Endurance Control
        /// </para>
        /// </summary>
        private ServerQue ServerBalancer;

        private Queue<string> SpeciesTag;

        private Furre player;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        public NetConnection()
        {
            Initilize();
        }

        /// <summary>
        /// </summary>
        /// <param name="Options">
        /// NetConnection Options
        /// </param>
        public NetConnection(ClientOptions Options)
        {
            Initilize();
            clientOptions = Options;
            Client.Options = clientOptions;
        }

        #endregion Public Constructors

        #region Public Delegates

        /// <summary>
        /// </summary>
        /// <param name="Sender">
        /// </param>
        /// <param name="e">
        /// </param>
        public delegate void ClientStatusChangedEventHandler(object Sender, NetClientEventArgs e);

        /// <summary>
        /// Send Data to Furcadia Client or Game Server
        /// </summary>
        /// <param name="Message">
        /// Raw instruction to send
        /// </param>
        /// <param name="e">
        /// Client or Server Event Arguments with Instruction type
        /// </param>
        public delegate void DataHandler(string Message, EventArgs e);

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        public delegate void OnErrorEventHandler(object sender, EventArgs e);

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// ChannelObject
        /// </param>
        /// <param name="Args">
        /// </param>
        public delegate void ProcessChannel(object sender, ParseChannelArgs Args);

        /// <summary>
        /// Send Server to Client Instruction object to Sub-classed for handling.
        /// </summary>
        /// <param name="sender">
        /// Server Instruction Object
        /// </param>
        /// <param name="Args">
        /// </param>
        public delegate void ProcessInstruction(object sender, ParseServerArgs Args);

        /// <summary>
        /// </summary>
        /// <param name="Sender">
        /// </param>
        /// <param name="e">
        /// </param>
        public delegate void ServerStatusChangedEventHandler(object Sender, NetServerEventArgs e);

        /// <summary>
        /// Throat Tired even handler
        /// </summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        public delegate void ThroatTiredEnabled(bool enable);

        #endregion Public Delegates

        #region Public Events

        /// <summary>
        /// This is triggered when a handled Exception is thrown.
        /// </summary>
        public event ErrorEventHandler Error;

        /// <summary>
        /// Process Display Text and Channels
        /// </summary>
        public event ProcessChannel ProcessServerChannelData;

        /// <summary>
        /// </summary>
        public event ProcessInstruction ProcessServerInstruction;

        /// <summary>
        /// Occurs when [server connected].
        /// </summary>
        public event ActionDelegate ServerConnected;

        /// <summary>
        /// Occurs when [server data2].
        /// </summary>
        public event DataEventHandler2 ServerData2;

        /// <summary>
        ///This is triggered when the Server Disconnects
        /// </summary>
        public event ActionDelegate ServerDisconnected;

        /// <summary>
        /// Track the Server Status
        /// </summary>
        public event ServerStatusChangedEventHandler ServerStatusChanged;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets the banish list.
        /// </summary>
        /// <value>
        /// The banish list.
        /// </value>
        public List<string> BanishList { get; private set; } = new List<string>();

        /// <summary>
        /// Current Name for Banish Operations
        /// <para>
        /// We mirror Furcadia's Banish system for efficiency
        /// </para>
        /// </summary>
        public string BanishName { get; private set; } = "";

        /// <summary>
        /// Connected Furre (Who we are)
        /// </summary>
        public Furre ConnectedFurre
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ConnectedFurreName))
                    return Furres.GetFurreByName(ConnectedFurreName);
                return new Furre(ConnectedFurreId, ConnectedFurreName);
            }
        }

        /// <summary>
        /// Gets or sets the connected furre identifier.
        /// </summary>
        /// <value>
        /// The connected furre identifier.
        /// </value>
        public int ConnectedFurreId { get; set; } = -1;

        /// <summary>
        /// Gets or sets the name of the connected furre.
        /// </summary>
        /// <value>
        /// The name of the connected furre.
        /// </value>
        public string ConnectedFurreName { get; set; }

        /// <summary>
        /// Current Dream Information with Furre List
        /// </summary>
        public Dream Dream
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the furres.
        /// </summary>
        /// <value>
        /// The furres.
        /// </value>
        public FurreList Furres { get; set; }

        /// <summary>
        /// We have Dream Share or We are Dream owner
        /// </summary>
        public bool HasShare { get; private set; }

        /// <summary>
        /// </summary>
        public bool InDream
        {
            get => !string.IsNullOrWhiteSpace(Dream.FileName);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is server socket connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is server socket connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsServerSocketConnected => Client.IsServerSocketConnected;

        /// <summary>
        /// Current Triggering player
        /// </summary>
        public Furre Player { get => player; private set => player = value; }

        /// <summary>
        /// Current server connection phase
        /// </summary>
        public ConnectionPhase ServerConnectPhase { get; private set; }

        /// <summary>
        /// Server Connection status
        /// </summary>
        /// <returns>
        /// Status of the Furcadia Game server
        /// </returns>
        public ConnectionPhase ServerStatus
        {
            get => ServerConnectPhase;
        }

        /// <summary>
        /// ServerQueue Throat Tired Mode
        /// <para>
        /// When set, a <see cref="System.Threading.Timer"/> is created to make us wait till the time is clear to resume.
        /// </para>
        /// </summary>
        /// <returns>
        /// State <see cref="Furcadia.Net.Utils.ServerQue.ThroatTired"/>
        /// </returns>
        public bool ThroatTired
        {
            get => ServerBalancer.ThroatTired;
            set => ServerBalancer.ThroatTired = value;
        }

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// Server Queue NoEndurance mode
        /// </summary>
        internal bool NoEndurance
        {
            get => ServerBalancer.NoEndurance;
            set => ServerBalancer.NoEndurance = value;
        }

        #endregion Internal Properties

        #region Public Methods

        /// <summary>
        ///Connect the Proxy to the Furcadia  Game server
        /// </summary>
        public void Connect()
        {
            ServerConnectPhase = ConnectionPhase.Connecting;
            Client.Connect();
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            Client.Disconnect();
        }

        /// <summary>
        /// implementation of Dispose pattern callable by consumers.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="enumVal">
        /// </param>
        /// <returns>
        /// </returns>
        public int GetEnumInt<T>(T enumVal)
        {
            return Convert.ToInt32(enumVal);
        }

        /// <summary>
        /// Are we the current executing character?
        /// </summary>
        public bool IsConnectedCharacter()
        {
            return ConnectedFurre == Player;
        }

        /// <summary>
        /// Is the target furre the connected characyer?
        /// </summary>
        /// <param name="Fur"><see cref="Furre"/></param>
        /// <returns>True if Fur is the Connected Furre</returns>
        public bool IsConnectedCharacter(IFurre Fur)
        {
            return ConnectedFurre == (Furre)Fur;
        }

        /// <summary>
        /// Called when [throat tired trigger].
        /// </summary>
        /// <param name="stat">if set to <c>true</c> [stat].</param>
        public void OnThroatTiredTrigger(bool stat)
        {
            TroatTiredEventHandler?.Invoke(stat);
        }

        /// <summary>
        /// Parse Channel Data
        /// </summary>
        /// <param name="data">
        /// Raw Game Server to Client instruction
        /// </param>
        /// <param name="Handled">
        /// Is this data already handled?
        /// </param>
        /// <remarks>
        /// This is derived content from the Furcadia Dev Docs and Furcadia
        /// Technical Resources
        /// <para>
        /// Update 23 Avatar Movement http://dev.furcadia.com/docs/023_new_movement.pdf
        /// </para>
        /// <para>
        /// Update 27 Movement http://dev.furcadia.com/docs/027_movement.html
        /// </para>
        /// <para>
        /// FTR http://ftr.icerealm.org/ref-instructions/
        /// </para>
        /// </remarks>
        public void ParseServerChannel(string data, bool Handled)
        {
            ChannelObject chanObject = new ChannelObject(data);
            ParseChannelArgs args = new ParseChannelArgs(data);
            Logger.Debug<NetConnection>(data);
            Regex FontColorRegex = new Regex(FontChannelFilter, RegexOptions.Compiled | RegexOptions.CultureInvariant);
            Match FontColorRegexMatch = FontColorRegex.Match(data);
            Match DescTagRegexMatch = DescTagRegex.Match(data);
            Furre ActivePlayer = new Furre(0, "Furcadia game server");
            string channel = null;

            if (NameRegex.Match(data).Success)
                ActivePlayer = Furres.GetFurreByName(NameRegex.Match(data).Groups[2].Value);

            if (DescTagRegexMatch.Success)
            {
                if (DescTagRegexMatch.Groups[1].Value == "fsh://system.fsh:86")
                {
                    string LineCountRegex = "<img src='fsh://system.fsh:86' /> Lines of DragonSpeak: ([0-9]+)";
                    Regex LineCount = new Regex(LineCountRegex, RegexOptions.Compiled);
                    if (LineCount.Match(data).Success)
                    {
                        Dream.Lines = int.Parse(LineCount.Match(data).Groups[1].Value);
                        Logger.Debug<NetConnection>($"DS Lines set to {Dream.Lines}");
                    }
                    LineCountRegex = "<img src='fsh://system.fsh:86' /> Dream Standard: <a href='http://www.furcadia.com/standards/'>(.*)</a>";
                    LineCount = new Regex(LineCountRegex, RegexOptions.Compiled);
                    if (LineCount.Match(data).Success)
                    {
                        Dream.Rating = LineCount.Match(data).Groups[1].Value;
                        Logger.Debug<NetConnection>($"Dream Rating set to {Dream.Rating}");
                    }
                    ActivePlayer.Message = DescTagRegexMatch.Groups[6].Value;
                    //LineCountRegex = "<img src='fsh://system.fsh:86' /> Dream Standard: <a href='http://www.furcadia.com/standards/'>(.*)</a>";
                    //LineCount = new Regex(LineCountRegex);
                    //if (LineCount.Match(data).Success)
                    //    Dream.Title = LineCount.Match(data).Groups[1].Value;

                    chanObject.player = ActivePlayer;
                    args.Channel = "@emit";

                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
            }
            string Color = FontColorRegexMatch.Groups[1].Value;
            try
            {
                string Text = NameRegex.Replace(data, "$2");

                Regex DescRegex = new Regex(DescFilter, ChannelOptions);
                if (DescRegex.Match(data).Success)
                {
                    ActivePlayer = Furres.GetFurreByName(DescRegex.Match(data).Groups[1].Value);
                    ((Furre)ActivePlayer).FurreDescription = DescRegex.Match(data).Groups[2].Value;

                    if (LookQue.Count > 0)
                    {
                        ((Furre)ActivePlayer).FurreColors = new ColorString(LookQue.Dequeue());
                    }
                    Player = ActivePlayer;
                    chanObject.player = ActivePlayer;
                    args.Channel = "desc";

                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
                //<a href='command://(.*?)'>click here</a> or type (`.*?) and press &lt;enter&gt;.</font>

                Match QueryMatch = QueryCommand.Match(data);
                if (QueryMatch.Success)
                {
                    var ChanObj = new QueryChannelObject(data, ((Furre)ActivePlayer));

                    args.Channel = QueryMatch.Groups[1].Value;

                    ProcessServerChannelData?.Invoke(ChanObj, args);
                    return;
                }

                Regex ShoutRegex = new Regex(ShoutRegexFilter, ChannelOptions);
                Match ShoutMatch = ShoutRegex.Match(data);
                if (ShoutMatch.Success)
                {
                    args.Channel = ShoutMatch.Groups[2].Value;

                    Player = Furres.GetFurreByName(ShoutMatch.Groups[5].Value);
                    Player.Message = ShoutMatch.Groups[9].Value;

                    chanObject.player = Player;

                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
                ShoutRegex = new Regex(YouShoutFilter, ChannelOptions);
                ShoutMatch = ShoutRegex.Match(data);
                if (ShoutMatch.Success)
                {
                    args.Channel = ShoutMatch.Groups[2].Value;

                    Player = ConnectedFurre;
                    Player.Message = ShoutMatch.Groups[4].Value;

                    chanObject.player = Player;

                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }

                if (Color == "success")
                {
                    if (Text.Contains(" has been banished from your Dreams."))
                    {
                        //banish <name> (online)
                        //Success: (.*?) has been banished from your Dreams.

                        Regex t = new Regex("(.*?) has been banished from your Dreams.", RegexOptions.Compiled);
                        BanishName = t.Match(Text).Groups[1].Value;

                        BanishList.Add(BanishName);
                        channel = "banish";
                    }
                    else if (Text == "You have canceled all banishments from your Dreams.")
                    {
                        //banish-off-all (active list)
                        //Success: You have canceled all banishments from your Dreams.
                        BanishList.Clear();
                        channel = "banish";
                    }
                    else if (Text.EndsWith(" has been temporarily banished from your Dreams."))
                    {
                        //tempbanish <name> (online)
                        //Success: (.*?) has been temporarily banished from your Dreams.

                        Regex t = new Regex("(.*?) has been temporarily banished from your Dreams.", RegexOptions.Compiled);
                        BanishName = t.Match(Text).Groups[1].Value;

                        // MainMSEngine.PageExecute(61)
                        BanishList.Add(BanishName);
                        channel = "banish";
                    }
                    else if (Text == "Control of this Dream is now being shared with you.")
                    {
                        HasShare = true;
                    }
                    else if (Text.EndsWith("is now sharing control of this Dream with you."))
                    {
                        HasShare = true;
                    }
                    else if (Text.EndsWith("has stopped sharing control of this Dream with you."))
                    {
                        HasShare = false;
                    }
                    else if (Text.StartsWith("The endurance limits of player "))
                    {
                        Regex t = new Regex("The endurance limits of player (.*?) are now toggled off.", RegexOptions.Compiled);
                        string m = t.Match(Text).Groups[1].Value;
                        if (m.ToFurcadiaShortName() == ConnectedFurre.ShortName)
                        {
                            NoEndurance = true;
                        }
                    }
                }
                else if (Color == "myspeech")
                {
                    Regex t = new Regex(YouSayFilter);
                    ConnectedFurre.Message = t.Match(data).Groups[2].Value;

                    chanObject.player = ConnectedFurre;
                    args.Channel = Color;

                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
                else if (string.IsNullOrEmpty(Color) &&
                           Regex.Match(data, NameFilter).Groups[2].Value != "forced")
                {
                    Match DescMatch = Regex.Match(data, "\\(you see(.*?)\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                    if (!DescMatch.Success)
                    {
                        Regex SayRegex = new Regex($"{NameFilter}: (.*)", RegexOptions.Compiled);
                        Match SayMatch = SayRegex.Match(data);
                        ActivePlayer.Message = SayRegex.Match(data).Groups[3].Value;
                        Player = ActivePlayer;
                        chanObject.player = ActivePlayer;
                        args.Channel = "say";

                        ProcessServerChannelData?.Invoke(chanObject, args);
                        return;
                    }
                    else // You see [FURRE]
                    {
                        ActivePlayer.Message = null;
                        Player = ActivePlayer;
                        chanObject.player = ActivePlayer;
                        // args.Channel = ??

                        ProcessServerChannelData?.Invoke(chanObject, args);
                        return;
                    }
                }
                else if (Color == "whisper")
                {
                    Regex WhisperIncoming = new Regex(WhisperRegex, ChannelOptions);
                    //'WHISPER
                    Match WhisperMatches = WhisperIncoming.Match(data);
                    if (WhisperMatches.Success)
                    {
                        ActivePlayer = Furres.GetFurreByName(WhisperMatches.Groups[4].Value);
                        ActivePlayer.Message = WhisperMatches.Groups[7].Value;

                        Player = ActivePlayer;

                        chanObject.player = ActivePlayer;
                        args.Channel = WhisperMatches.Groups[2].Value;

                        ProcessServerChannelData?.Invoke(chanObject, args);
                        return;
                    }
                    WhisperIncoming = new Regex(YouWhisperRegex, ChannelOptions);
                    WhisperMatches = WhisperIncoming.Match(data);
                    if (WhisperMatches.Success)
                    {
                        Player = ConnectedFurre;
                        //TODO: Test Player is active Furre
                        Player.Message = WhisperMatches.Groups[4].Value;
                        chanObject = new ChannelObject(data, ConnectedFurre);
                        args.Channel = WhisperMatches.Groups[2].Value;

                        ProcessServerChannelData?.Invoke(chanObject, args);
                        return;
                    }
                }
                else if (Color == "trade")
                {
                    //TODO: Verify Trade System is correct
                    channel = "trade";
                    ActivePlayer.Message = FontColorRegex.Match(data).Groups[4].Value;
                    Player = ActivePlayer;
                }
                else if (Color == "emote")
                {
                    Regex EmoteRegex = new Regex(EmoteRegexFilter, ChannelOptions);
                    Match EmoteMatch = EmoteRegex.Match(data);

                    args.Channel = EmoteMatch.Groups[1].Value;

                    Player = Furres.GetFurreByName(EmoteMatch.Groups[3].Value);
                    Player.Message = EmoteMatch.Groups[6].Value;

                    chanObject.player = Player;

                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
                else if (Color == "notify")
                {
                    string NameStr = "";
                    if (chanObject.ChannelText.StartsWith("players banished from your Dreams: "))
                    {
                        //Banish-List
                        //[notify> players banished from your Dreams:
                        //`(0:54) When the bot sees the banish list
                        BanishList.Clear();
                        string[] tmp = Text.Substring(35).Split(',');
                        foreach (string t in tmp)
                        {
                            BanishList.Add(t);
                        }
                    }
                    else if (chanObject.ChannelText.StartsWith("The banishment of player "))
                    {
                        //banish-off <name> (on list)
                        //[notify> The banishment of player (.*?) has ended.

                        Regex t = new Regex("The banishment of player (.*?) has ended.", RegexOptions.Compiled);
                        NameStr = t.Match(data).Groups[1].Value;
                        ActivePlayer = new Furre(NameStr);
                        Player = ActivePlayer;
                        bool found = false;
                        int I;
                        for (I = 0; I <= BanishList.Count - 1; I++)
                        {
                            if (BanishList[I].ToFurcadiaShortName() == Player.ShortName)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found)
                            BanishList.RemoveAt(I);
                    }
                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
                else if (Color == "error")
                {
                    channel = "error";

                    string NameStr = "";
                    if (chanObject.ChannelText.Contains("There are no Furres around right now with a name starting with "))
                    {
                        //Banish <name> (Not online)
                        //Error:>>  There are no Furres around right now with a name starting with (.*?) .

                        Regex t = new Regex("There are no Furres around right now with a name starting with (.*?) .", RegexOptions.Compiled);
                        NameStr = t.Match(data).Groups[1].Value;
                    }
                    else if (chanObject.ChannelText == "Sorry, this player has not been banished from your Dreams.")
                    {
                        //banish-off <name> (not on list)
                        //Error:>> Sorry, this player has not been banished from your Dreams.

                        NameStr = BanishName;
                    }
                    else if (chanObject.ChannelText == "You have not banished anyone.")
                    {
                        //banish-off-all (empty List)
                        //Error:>> You have not banished anyone.

                        BanishList.Clear();
                    }
                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
                else if (data.StartsWith("Communication"))
                {
                    Disconnect();
                }
                else if (channel == "@cookie")
                {
                    // <font color='emit'><img src='fsh://system.fsh:90' alt='@cookie' /><channel name='@cookie' /> Cookie <a href='http://www.furcadia.com/cookies/Cookie%20Economy.html'>bank</a> has currently collected: 0</font>
                    // <font color='emit'><img src='fsh://system.fsh:90' alt='@cookie' /><channel name='@cookie' /> All-time Cookie total: 0</font>
                    // <font color='success'><img src='fsh://system.fsh:90' alt='@cookie' /><channel name='@cookie' /> Your cookies are ready.  http://furcadia.com/cookies/ for more info!</font>
                    //<img src='fsh://system.fsh:90' alt='@cookie' /><channel name='@cookie' /> You eat a cookie.

                    Regex EatCookie = new Regex("<img src='fsh://system.fsh:90' alt='@cookie' /><channel name='@cookie' /> You eat a cookie.(.*?)", ChannelOptions);
                    if (EatCookie.Match(data).Success)
                    {
                        Player.Message = "You eat a cookie." + EatCookie.Replace(data, "");
                    }
                    //player = ConnectedFurre;
                }
                else //Dynamic Channels
                {
                    channel = FontColorRegexMatch.Groups[8].Value;
                    if (NameRegex.Match(data).Success)
                    {
                        var name = NameRegex.Match(data).Groups[2].Value;
                        Player = Furres.GetFurreByName(name);
                    }
                }
            }
            catch (Exception ex)
            {
                SendError(ex, this);
            }

            if (string.IsNullOrWhiteSpace(ActivePlayer.Message))
                ActivePlayer.Message = FontColorRegexMatch.Groups[10].Value.Substring(1);

            Player = ActivePlayer;

            chanObject.player = ActivePlayer;
            args.Channel = FontColorRegexMatch.Groups[9].Value;

            ProcessServerChannelData?.Invoke(chanObject, args);
        }

        /// <summary>
        /// Parse Server Data
        /// </summary>
        /// <param name="data">
        /// </param>
        /// <param name="Handled">
        /// </param>
        /// ///
        /// <remarks>
        /// This is derived content from the Furcadia Dev Docs and Furcadia
        /// Technical Resources
        /// <para>
        /// Update 23 Avatar Movement http://dev.furcadia.com/docs/023_new_movement.pdf
        /// </para>
        /// <para>
        /// Update 27 Movement http://dev.furcadia.com/docs/027_movement.html
        /// </para>
        /// <para>
        /// FTR http://ftr.icerealm.org/ref-instructions/
        /// </para>
        /// </remarks>
        public void ParseServerData(string data, ref bool Handled)
        {
            data = data.TrimEnd('\n');
            Logger.Debug<NetConnection>(data);
            switch (ServerConnectPhase)
            {
                case ConnectionPhase.MOTD:
                    // SSL/TLS `starttls
                    if (data.StartsWith("]s"))
                    {
                        Regex t = new Regex("\\]s(.+)1 (.*?) (.*?) 0", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        Match m = t.Match(data);
                    }
                    if (data == "Dragonroar")
                    {
                        // Login Sucessful
                        ServerConnectPhase = ConnectionPhase.Auth;

                        // connect characterName password machineID
                        if (string.IsNullOrWhiteSpace(clientOptions.Account))
                        {
                            SendToServer($"connect {clientOptions.CharacterShortName} {clientOptions.Password}");
                            ConnectedFurreName = clientOptions.CharacterName;
                        }
                        else // account email characterName password token
                        {
                            SendToServer($"account {clientOptions.Account} {clientOptions.CharacterShortName} {clientOptions.Password}");
                            SendToServer($"costume %-1");
                            ConnectedFurreName = clientOptions.CharacterName;
                        }
                        ServerStatusChanged?.Invoke(data, new NetServerEventArgs(ServerConnectPhase, ServerInstructionType.Unknown));
                    }
                    break;

                case ConnectionPhase.Auth:
                    if (data.StartsWith("]]"))
                    {
                        Disconnect();
                    }
                    else if (data == "&&&&&&&&&&&&&")
                    {
                        //We've connected to Furcadia
                        ServerConnectPhase = ConnectionPhase.Connected;

                        ServerStatusChanged?.Invoke(data, new NetServerEventArgs(ServerConnectPhase));
                    }
                    break;

                case ConnectionPhase.Connected:
                    if (string.IsNullOrEmpty(data))
                        return;
                    if (data == "]ccmarbled.pcx")
                    {
                        Dream = new Dream();
                        var instruction = new BaseServerInstruction(data)
                        {
                            InstructionType = ServerInstructionType.EnterDream
                        };

                        ProcessServerInstruction?.Invoke
                            (
                                instruction,
                                new ParseServerArgs(ServerInstructionType.EnterDream, ServerConnectPhase)
                            );
                    }
                    // Look instruction
                    if (data.StartsWith("]f") && InDream == true)
                    {
                        if (Look)
                        {
                            LookQue.Enqueue(data.Substring(2));
                        }
                        else
                        {
                            // player = NameToFurre(data.Remove(0, length +
                            // 2)); If player.ID = 0 Then Exit Sub
                            Player.FurreColors = new ColorString(data.Substring(2, ColorString.ColorStringSize));

                            if (IsConnectedCharacter())
                                Look = false;

                            UpdateColorString Instruction = new UpdateColorString(Player, data)
                            {
                                InstructionType = ServerInstructionType.LookResponse
                            };

                            ProcessServerInstruction?.Invoke
                            (
                                Instruction,
                                new ParseServerArgs(ServerInstructionType.LookResponse, ServerConnectPhase)
                            );
                        };
                    }

                    //Spawn Avatar
                    else if (data[0] == '<')
                    {
                        SpawnAvatar FurreSpawn = new SpawnAvatar(data);
                        Player = FurreSpawn.Player;
                        Furres.Add(Player);
                        Logger.Debug<NetConnection>(FurreSpawn);
                        //New furre Arrival to current Dream
                        if (FurreSpawn.PlayerFlags.HasFlag(CHAR_FLAG_NEW_AVATAR))
                        {
                            Logger.Debug<NetConnection>(FurreSpawn);
                            if (!Furres.Contains(Player))
                            {
                                ProcessServerInstruction?.Invoke(
                                    FurreSpawn,
                                    new ParseServerArgs(ServerInstructionType.SpawnAvatar, ServerConnectPhase));
                            }
                        }
                    }
                    //Remove Furre
                    else if (data[0] == ')' || data[0] == ' ')
                    {
                        RemoveAvatar RemoveFurre = new RemoveAvatar(data);
                        RemoveFurre.Player = Furres.GetFurreByID(RemoveFurre.FurreId);
                        //if (player.ShortName == "unknown" || player.FurreID < 1)
                        //    throw new ArgumentException($"Remove Avatar unknown Furre {player}");
                        if (Furres.Contains(RemoveFurre.FurreId))
                        {
                            Furres.Remove(RemoveFurre.FurreId);

                            Handled = true;
                            Logger.Debug<NetConnection>(RemoveFurre);
                            ProcessServerInstruction?.Invoke
                            (
                                RemoveFurre,
                                new ParseServerArgs(ServerInstructionType.RemoveAvatar, ServerConnectPhase)
                            );
                        }
                        return;
                    }
                    //Animated Move or Move Furre
                    else if (data[0] == '/' || data[0] == 'A')
                    {
                        player = Furres.GetFurreByID(data.Substring(1, 4));

                        if (Player.FurreID < 1 || Player.ShortName == "unknown")
                            throw new ArgumentOutOfRangeException("Player", player, "Player not found in dream.");

                        var FurreMoved = new MoveFurre(data, ref player);

                        if (ConnectedFurre.FurreID < 1)
                            throw new ArgumentOutOfRangeException("ConnectedFurre", ConnectedFurre, "ConnectedFurre not found in dream.");

                        ViewArea ConnectedFurreVisibleRectabgle = new ViewArea(ConnectedFurre.Location);

                        Player.InRange = ConnectedFurreVisibleRectabgle.InRange(player.Location);

                        Furres.Update(player);

                        Logger.Debug<NetConnection>(FurreMoved);
                        ProcessServerInstruction?.Invoke(FurreMoved,
                              new ParseServerArgs(FurreMoved.InstructionType, ServerConnectPhase));

                        return;
                    }
                    //Update ColorString
                    else if (data[0] == 'B')
                    {
                        //fuid 4 b220 bytes
                        Player = Furres.GetFurreByID(data.Substring(1, 4));
                        UpdateColorString ColorStringUpdate = new UpdateColorString(Player, data);

                        if (InDream)
                        {
                            if (ProcessServerInstruction != null)
                            {
                                ProcessServerInstruction?.Invoke(ColorStringUpdate,
                                new ParseServerArgs(ServerInstructionType.UpdateColorString, ServerConnectPhase));
                            }
                        }
                    }
                    //Hide Avatar
                    else if (data[0] == 'C')
                    {
                        Player = Furres.GetFurreByID(data.Substring(1, 4));
                        Player.Location = new FurrePosition(data.Substring(5, 4));
                        Player.InRange = false;
                    }
                    //Species Tags
                    else if (data.StartsWith("]-"))
                    {
                        if (data.StartsWith("]-#A"))
                        {
                            SpeciesTag.Enqueue(data.Substring(4));
                        }
                        else if (data.StartsWith("]-#B"))
                        {
                            BadgeTag.Enqueue(data.Substring(2));
                        }

                        //DS Variables
                    }

                    //Popup Dialogs!
                    else if (data.StartsWith("]#"))
                    {
                        //]#<idstring> <style 0-17> <message that might have spaces in>
                        Regex repqq = new Regex("^\\]#(.*?) (\\d+) (.*?)$", RegexOptions.Compiled);
                        Match m = repqq.Match(data);
                        Rep r = default(Rep);
                        r.ID = m.Groups[1].Value;
                        int.TryParse(m.Groups[2].Value, out int num);
                        r.Type = num;
                        Repq.Enqueue(r);
                        Player.Message = m.Groups[3].Value;

                        //]s(.+)1 (.*?) (.*?) 0
                    }

                    //Disconnection Error
                    else if (data[0] == '[')
                    {
                        Furres.Clear();
                    }

                    //Dream Load ]q
                    //vasecodegamma
                    else if (data.StartsWith("]q"))
                    {
                        //Set defaults (should move to some where else?
                        HasShare = false;
                        NoEndurance = false;
                        LoadDream loadDream = new LoadDream(data);
                        Dream.Load(loadDream);
                        Logger.Debug<NetConnection>($"Load Dream {loadDream}");
                        if (ProcessServerInstruction != null)
                            ProcessServerInstruction?.Invoke(loadDream,
                               new ParseServerArgs(ServerInstructionType.LoadDreamEvent, ServerConnectPhase));

                        SendToServer("vascodagama");
                        SendToServer("dreambookmark 0");

                        return;
                    }
                    // ]z UID[*]
                    // Unique User ID
                    // This instruction is sent as a response to the uid command. The purpose of this is unclear.
                    // Credits Artex, FTR
                    else if (data.StartsWith("]z"))
                    {
                        ProcessServerInstruction?.Invoke(new BaseServerInstruction(data),
                            new ParseServerArgs(ServerInstructionType.UniqueUserId, ServerConnectPhase));
                    }
                    // ]BUserID[*]
                    // Set Own ID
                    // This instruction informs the client of which user-name is it logged into. Knowing your
                    // own UserID can help you find your own avatar within the Dream.
                    // Credits Artex, FTR
                    else if (data.StartsWith("]B"))
                    {
                        var furre = data.Split(new char[] { ' ' }, 2);

                        ConnectedFurreId = int.Parse(furre[0].Substring(2));
                        ConnectedFurreName = furre[1];
                        ProcessServerInstruction?.Invoke
                        (
                            new BaseServerInstruction(data),
                            new ParseServerArgs(ServerInstructionType.SetOwnId, ServerConnectPhase)
                        );
                    }
                    else if (data.StartsWith("]c"))
                    {
                        //#if DEBUG
                        //                        Console.WriteLine(data);
                        //#endif
                    }
                    // Dream Bookmark
                    //]CBookmark Type[1]Dream URL[*]
                    // Type 0 = temporary
                    // Type 1 = Regular (per user requests)
                    // DreamUrl = "furc://uploadername:Dreamname/entrycode "
                    // Credits FTR
                    else if (data.StartsWith("]C"))
                    {
                        if (data.StartsWith("]C0") || data.StartsWith("]C1"))
                        {
                            DreamBookmark bookmark = new DreamBookmark(data);

                            Dream.BookMark = bookmark;
                            Logger.Debug<NetConnection>($"Dream Bookmark: {bookmark}");
                            ProcessServerInstruction?.Invoke
                                (
                                    bookmark,
                                    new ParseServerArgs(ServerInstructionType.BookmarkDream, ServerConnectPhase)
                                );
                            return;
                        }
                    }

                    //Process Channels Seperatly
                    else if (data[0] == '(')
                    {
                        if (ThroatTired == false && data.StartsWith("(<font color='warning'>Your throat is tired. Try again in a few seconds.</font>"))
                        {
                            //Using Furclib ServQue
                            ThroatTired = true;
                        }
                        // Send the Data to Channel Parser
                        ParseServerChannel(data.Substring(1), Handled);
                        return;
                    }

                    break;
                // Do nothing - we're disconnected...
                case ConnectionPhase.Disconnected:
                    {
                        ServerStatusChanged?.Invoke(null,
                                    new NetServerEventArgs(ServerConnectPhase, ServerInstructionType.None));
                        break;
                    }
            }
        }

        /// <summary>
        /// send errors to the error handler
        /// </summary>
        /// <param name="e"></param>
        /// <param name="o"></param>
        public virtual void SendError(Exception e, object o)
        {
            if (Error != null)
            {
                Error?.Invoke(e, o);
            }
            else
            {
                throw e;
            }
        }

        /// <summary>
        /// Format basic furcadia commands and send to server
        /// <para>
        /// We also mirror the client to server banish system.
        /// </para>
        /// <para>
        /// This maybe a good place to place Proxy/Bot commands for controls
        /// </para>
        /// <para>
        /// default to say or "normal spoken command"
        /// </para>
        /// </summary>
        /// <param name="data">
        /// Raw Client to Server instruction
        /// </param>
        public virtual void SendFormattedTextToServer(string data)
        {
            Logger.Debug<NetConnection>(data);
            if (data.StartsWith("banish "))
            {
                BanishName = data.Substring(7);
            }
            else if (data.StartsWith("banish-off ") | data.StartsWith("tempbanish "))
            {
                BanishName = data.Substring(11);
            }
            else if (data == "banish-list")
            {
                BanishName = "";
            }

            TextToServer(ref data);
        }

        /// <summary>
        /// Send a raw instruction to Server through the Load Balancer
        /// </summary>
        /// <param name="message">
        /// Client to server Instruction
        /// </param>
        public void SendToServer(string message)
        {
            Logger.Debug<NetConnection>(message);
            if (ServerConnectPhase != ConnectionPhase.Disconnected)
                ServerBalancer.SendToServer(message);
        }

        /// <summary>
        /// Text Channel Prefixes (shout,whisper emote, Raw Server command)
        /// <para>
        /// default to say or "normal spoken command"
        /// </para>
        /// </summary>
        /// <param name="data">
        /// </param>
        public void TextToServer(ref string data)
        {
            Logger.Debug<NetConnection>(data);
            if (string.IsNullOrWhiteSpace(data))
                return;
            //Clean Text input to match Client
            string result = "";
            switch (data[0])
            {
                case '`':
                    result = data.Remove(0, 1);
                    break;

                case '/':
                    result = "wh " + data.Substring(1);
                    break;

                case ':':
                    result = data;

                    break;

                case '-':
                    result = data;

                    break;

                default:
                    result = "\"" + data;
                    break;
            }
            SendToServer(result);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Client.Dispose();
                ServerBalancer.Dispose();
            }

            // Free any unmanaged objects here.
            disposed = true;
        }

        private void Initilize()
        {
#if DEBUG
            Logger.Disable<NetConnection>();
            // if (!Debugger.IsAttached)
            // Logger.Disable<NetConnection>();
#else
            Logger.Disable<NetConnection>();
#endif
            clientOptions = new ClientOptions();
            Client = new ClientBase(clientOptions);
            SpeciesTag = new Queue<string>();
            BadgeTag = new Queue<string>();

            ServerConnectPhase = ConnectionPhase.Init;

            ServerBalancer = new Utils.ServerQue();
            ServerBalancer.OnServerSendMessage += (o, e) => OnServerQueSent(o, e);
            ServerBalancer.TroatTiredEventHandler += e => OnThroatTiredTrigger(e);

            Client.ServerData2 += e => OnServerDataReceived(e);
            Client.ServerConnected += () => OnServerConnected();
            Client.ServerDisconnected += () => OnServerDisonnected();

            Client.Error += (e, o) => OnError(e, o);

            Player = new Furre();
            Dream = new Dream();
            Furres = new FurreList(100);

            //BadgeTag = new Queue<string>(50);
            LookQue = new Queue<string>(50);

            //          SpeciesTag = new Queue<string>(50);
            //          BanishString = new List<string>(50);
        }

        private void OnError(Exception e, object v)
        {
            SendError(e, v);
        }

        private void OnServerConnected()
        {
            if (ServerConnectPhase == ConnectionPhase.MOTD)
                return; //Do nothing, Signal was already sent
            ServerConnectPhase = ConnectionPhase.MOTD;
            ServerDisconnected?.Invoke();
            ServerStatusChanged?.Invoke(this, new NetServerEventArgs(ServerConnectPhase, ServerInstructionType.None));
        }

        /// <summary>
        /// </summary>
        /// <param name="data">
        /// </param>
        private void OnServerDataReceived(string data)
        {
            Player = new Furre();
            bool handled = false;
            ParseServerData(data, ref handled);

            if (!handled)
                ServerData2?.Invoke(data);
        }

        private void OnServerDisonnected()
        {
            if (ServerConnectPhase == ConnectionPhase.Disconnected)
                return; //Do nothing, Signal was already sent
            ServerConnectPhase = ConnectionPhase.Disconnected;
            ServerDisconnected?.Invoke();
            ServerStatusChanged?.Invoke(this, new NetServerEventArgs(ServerConnectPhase, ServerInstructionType.None));
        }

        private void OnServerQueSent(object sender, EventArgs e)
        {
            Client.SendToServer(sender.ToString());
        }

        #endregion Private Methods
    }
}