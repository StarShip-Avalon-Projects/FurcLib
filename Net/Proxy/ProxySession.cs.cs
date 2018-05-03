﻿using Extentions;
using Furcadia.Drawing;
using Furcadia.Extensions;
using Furcadia.Logging;
using Furcadia.Movement;
using Furcadia.Net.DreamInfo;
using Furcadia.Net.Utils.ChannelObjects;
using Furcadia.Net.Utils.ServerObjects;
using Furcadia.Net.Utils.ServerParser;
using Furcadia.Text;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Furcadia.Movement.CharacterFlags;
using static Furcadia.Text.FurcadiaMarkup;

namespace Furcadia.Net.Proxy
{
    /// <summary>
    /// This Instance handles the current Furcadia Session.
    /// <para>
    /// Part1: Manage MonkeySpeak Engine Start,Stop,Restart. System
    ///        Variables, MonkeySpeak Execution Triggers
    /// </para>
    /// <para>
    /// Part2: Furcadia Proxy Controls, In/Out Ports, Host, Character Ini
    ///        file. Connect, Disconnect, Reconnect
    /// </para>
    /// <para>
    /// Part2a: Proxy Functions do link to Monkey Speak trigger execution
    /// </para>
    /// <para>
    /// Part3: This Class Links loosely to the GUI
    /// </para>
    /// </summary>
    public class ProxySession : NetProxy, IDisposable
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

        /// <summary>
        /// Gets or sets the name of the connected furre.
        /// </summary>
        /// <value>
        /// The name of the connected furre.
        /// </value>
        public string ConnectedFurreName { get; set; }

        /// <summary>
        /// Gets or sets the connected furre identifier.
        /// </summary>
        /// <value>
        /// The connected furre identifier.
        /// </value>
        public int ConnectedFurreId { get; set; } = -1;

        ///// <summary>
        ///// Beekin Badge
        ///// </summary>
        private Queue<string> BadgeTag;

        private string banishName = "";

        private List<string> banishString = new List<string>();

        private object ChannelLock = new object();

        private RegexOptions ChannelOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        private ConnectionPhase clientconnectionphase;

        private bool disposed = false;

        private bool hasShare;

        private bool Look;

        private Queue<string> LookQue;

        private static Furre player;

        /// <summary>
        /// Balance the out going load to server
        /// <para>
        /// Throat Tired Syndrome and No Endurance Control
        /// </para>
        /// </summary>
        private Utils.ServerQue ServerBalancer;

        private ConnectionPhase serverconnectphase;

        private Queue<string> SpeciesTag;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        public ProxySession() : this(new Options.ProxyOptions())
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="Options">
        /// ProxySession Options
        /// </param>
        public ProxySession(Options.ProxyOptions Options) : base(Options)
        {
            Initilize();
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
        /// This is triggered when the Client sends data to the server.
        /// Expects a return value.
        /// </summary>
        public override event DataEventHandler2 ClientData2;

        /// <summary>
        /// Track the Furcadia Client status
        /// </summary>
        public event ClientStatusChangedEventHandler ClientStatusChanged;

        /// <summary>
        /// Process Display chanObject.ChannelText and Channels
        /// </summary>
        public event ProcessChannel ProcessServerChannelData;

        /// <summary>
        /// </summary>
        public event ProcessInstruction ProcessServerInstruction;

        /// <summary>
        /// This is triggered when the Server sends data to the client.
        /// Doesn't expect a return value.
        /// </summary>
        public override event DataEventHandler2 ServerData2;

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
        public List<string> BanishList
        {
            get => banishString;
            private set => banishString = value;
        }

        /// <summary>
        /// Current Name for Banish Operations
        /// <para>
        /// We mirror Furcadia's Banish system for efficiency
        /// </para>
        /// </summary>
        public string BanishName
        {
            get => banishName;
            private set => banishName = value;
        }

        /// <summary>
        /// Current Connection Phase
        /// </summary>
        public ConnectionPhase ClientConnectPhase
        {
            get => clientconnectionphase;
        }

        /// <summary>
        /// Client Connection status
        /// </summary>
        /// <returns>
        /// Status tog the Furcadia Client
        /// </returns>
        public ConnectionPhase ClientStatus
        {
            get => clientconnectionphase;
        }

        /// <summary>
        /// Connected Furre (Who we are)
        /// </summary>
        public Furre ConnectedFurre
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ConnectedFurreName))
                    return new Furre(ConnectedFurreId);
                return Dream.Furres.GetFurreByName(ConnectedFurreName);
            }
        }

        /// <summary>
        /// Current Dream Information with Furre List
        /// </summary>
        public Dream Dream
        {
            get => dream; private set => dream = value;
        }

        private static Dream dream;

        /// <summary>
        /// We have Dream Share or We are Dream owner
        /// </summary>
        public bool HasShare
        {
            get => hasShare;
        }

        /// <summary>
        /// </summary>
        public bool InDream
        {
            get => !string.IsNullOrWhiteSpace(Dream.FileName);
        }

        /// <summary>
        /// Current Triggering player
        /// </summary>
        public Furre Player
        {
            get => player;
            private set => player = value;
        }

        /// <summary>
        /// Current server connection phase
        /// </summary>
        public ConnectionPhase ServerConnectPhase
        {
            get => serverconnectphase;
        }

        /// <summary>
        /// Server Connection status
        /// </summary>
        /// <returns>
        /// Status of the Furcadia Game server
        /// </returns>
        public ConnectionPhase ServerStatus
        {
            get => serverconnectphase;
        }

        /// <summary>
        /// Allows the Furcadia Client to Disconnect from the session,
        /// allowing the session to remain connected to the game server
        /// </summary>
        public bool StandAlone
        {
            get => Options.Standalone;
            set => Options.Standalone = value;
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
        public override void Connect()
        {
            serverconnectphase = ConnectionPhase.Connecting;
            clientconnectionphase = ConnectionPhase.Connecting;
            base.Connect();
        }

        /// <summary>
        /// implementation of Dispose pattern callable by consumers.
        /// </summary>
        public override void Dispose()
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
            return ConnectedFurre == player;
        }

        /// <summary>
        /// Is the target furre the connected characyer?
        /// </summary>
        /// <param name="Fur"><see cref="Furre"/></param>
        /// <returns>True if Fur is the Connected Furre</returns>
        public bool IsConnectedCharacter(IFurre Fur)
        {
            return ConnectedFurre == Fur;
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
            if (Handled)
                return;
            ChannelObject chanObject = new ChannelObject(data);
            ParseChannelArgs args = new ParseChannelArgs(data);
            Logger.Debug<ProxySession>(data);
            Regex FontColorRegex = new Regex(FontChannelFilter, RegexOptions.Compiled | RegexOptions.CultureInvariant);
            Match FontColorRegexMatch = FontColorRegex.Match(data);
            Match DescTagRegexMatch = DescTagRegex.Match(data);
            Furre ActivePlayer = new Furre("Furcadia game server");

            if (NameRegex.Match(data).Success)
                ActivePlayer = Dream.Furres.GetFurreByName(NameRegex.Match(data).Groups[2].Value);

            if (DescTagRegexMatch.Success)
            {
                if (DescTagRegexMatch.Groups[1].Value == "fsh://system.fsh:86")
                {
                    string LineCountRegex = "<img src='fsh://system.fsh:86' /> Lines of DragonSpeak: ([0-9]+)";
                    Regex LineCount = new Regex(LineCountRegex, RegexOptions.Compiled);
                    if (LineCount.Match(data).Success)
                    {
                        Dream.Lines = int.Parse(LineCount.Match(data).Groups[1].Value);
                        Logger.Debug<ProxySession>($"DS Lines set to {Dream.Lines}");
                    }
                    LineCountRegex = "<img src='fsh://system.fsh:86' /> Dream Standard: <a href='http://www.furcadia.com/standards/'>(.*)</a>";
                    LineCount = new Regex(LineCountRegex, RegexOptions.Compiled);
                    if (LineCount.Match(data).Success)
                    {
                        Dream.Rating = LineCount.Match(data).Groups[1].Value;
                        Logger.Debug<ProxySession>($"Dream Rating set to {Dream.Rating}");
                    }
                    ActivePlayer.Message = DescTagRegexMatch.Groups[6].Value;
                    //LineCountRegex = "<img src='fsh://system.fsh:86' /> Dream Standard: <a href='http://www.furcadia.com/standards/'>(.*)</a>";
                    //LineCount = new Regex(LineCountRegex);
                    //if (LineCount.Match(data).Success)
                    //    Dream.Title = LineCount.Match(data).Groups[1].Value;
                    player = ActivePlayer;
                    chanObject.player = player;
                    args.Channel = "@emit";

                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
            }
            string Color = FontColorRegexMatch.Groups[1].Value;

            Regex DescRegex = new Regex(DescFilter, ChannelOptions);
            if (DescRegex.Match(data).Success)
            {
                ActivePlayer = Dream.Furres.GetFurreByName(DescRegex.Match(data).Groups[1].Value);
                ((Furre)ActivePlayer).FurreDescription = DescRegex.Match(data).Groups[2].Value;

                if (LookQue.Count > 0)
                {
                    ((Furre)ActivePlayer).FurreColors = new ColorString(LookQue.Dequeue());
                }
                player = ActivePlayer;
                chanObject.player = ActivePlayer;
                args.Channel = "desc";

                ProcessServerChannelData?.Invoke(chanObject, args);
                return;
            }
            //<a href='command://(.*?)'>click here</a> or type (`.*?) and press &lt;enter&gt;.</font>

            Match QueryMatch = QueryCommand.Match(data);
            if (QueryMatch.Success)
            {
                chanObject = new QueryChannelObject(data, ((Furre)ActivePlayer));

                args.Channel = QueryMatch.Groups[1].Value;

                ProcessServerChannelData?.Invoke(chanObject, args);
                return;
            }

            Regex ShoutRegex = new Regex(ShoutRegexFilter, ChannelOptions);
            Match ShoutMatch = ShoutRegex.Match(data);
            if (ShoutMatch.Success)
            {
                args.Channel = ShoutMatch.Groups[2].Value;

                player = Dream.Furres.GetFurreByName(ShoutMatch.Groups[5].Value);
                player.Message = ShoutMatch.Groups[9].Value;

                chanObject.player = player;

                ProcessServerChannelData?.Invoke(chanObject, args);
                return;
            }
            ShoutRegex = new Regex(YouShoutFilter, ChannelOptions);
            ShoutMatch = ShoutRegex.Match(data);
            if (ShoutMatch.Success)
            {
                args.Channel = ShoutMatch.Groups[2].Value;

                player = ConnectedFurre;
                player.Message = ShoutMatch.Groups[4].Value;

                chanObject.player = player;

                ProcessServerChannelData?.Invoke(chanObject, args);
                return;
            }

            if (Color == "success")
            {
                if (chanObject.ChannelText.Contains(" has been banished from your Dreams."))
                {
                    //banish <name> (online)
                    //Success: (.*?) has been banished from your Dreams.

                    Regex t = new Regex("(.*?) has been banished from your Dreams.", RegexOptions.Compiled);
                    BanishName = t.Match(chanObject.ChannelText).Groups[1].Value;

                    BanishList.Add(BanishName);
                }
                else if (chanObject.ChannelText == "You have canceled all banishments from your Dreams.")
                {
                    //banish-off-all (active list)
                    //Success: You have canceled all banishments from your Dreams.
                    BanishList.Clear();
                }
                else if (chanObject.ChannelText.EndsWith(" has been temporarily banished from your Dreams."))
                {
                    //tempbanish <name> (online)
                    //Success: (.*?) has been temporarily banished from your Dreams.

                    Regex t = new Regex("(.*?) has been temporarily banished from your Dreams.", RegexOptions.Compiled);
                    BanishName = t.Match(chanObject.ChannelText).Groups[1].Value;

                    // MainMSEngine.PageExecute(61)
                    BanishList.Add(BanishName);
                }
                else if (chanObject.ChannelText == "Control of this Dream is now being shared with you.")
                {
                    hasShare = true;
                }
                else if (chanObject.ChannelText.EndsWith("is now sharing control of this Dream with you."))
                {
                    hasShare = true;
                }
                else if (chanObject.ChannelText.EndsWith("has stopped sharing control of this Dream with you."))
                {
                    hasShare = false;
                }
                else if (chanObject.ChannelText.StartsWith("The endurance limits of player "))
                {
                    Regex t = new Regex("The endurance limits of player (.*?) are now toggled off.", RegexOptions.Compiled);
                    string m = t.Match(chanObject.ChannelText).Groups[1].Value;
                    if (m.ToFurcadiaShortName() == ConnectedFurre.ShortName)
                    {
                        NoEndurance = true;
                    }
                }
            }
            else if (Color == "myspeech")
            {
                //TODO: Test Active Player
                Regex t = new Regex(YouSayFilter);
                ActivePlayer.Message = t.Match(data).Groups[2].Value;
                chanObject.player = ActivePlayer;
                args.Channel = Color;

                ProcessServerChannelData?.Invoke(chanObject, args);
                return;
            }
            else if (string.IsNullOrEmpty(Color) && Regex.Match(data, NameFilter).Groups[2].Value != "forced")
            {
                Match DescMatch = Regex.Match(data, "\\(you see(.*?)\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                if (!DescMatch.Success)
                {
                    Regex SayRegex = new Regex($"{NameFilter}: (.*)", RegexOptions.Compiled);
                    Match SayMatch = SayRegex.Match(data);
                    if (SayMatch.Success)
                    {
                        args.Channel = "say";
                        player = Dream.Furres.GetFurreByName(SayMatch.Groups[2].Value);
                        player.Message = SayMatch.Groups[3].Value;

                        chanObject.player = player;

                        ProcessServerChannelData?.Invoke(chanObject, args);
                        return;
                    }
                    else
                    {
                        // Process everything as raw text
                        args.Channel = "text";

                        ActivePlayer.Message = data;
                        chanObject.player = ActivePlayer;

                        ProcessServerChannelData?.Invoke(chanObject, args);
                        return;
                    }
                }
                else // You see [FURRE]
                {
                    ActivePlayer.Message = null;
                    player = ActivePlayer;
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
                    ActivePlayer = Dream.Furres.GetFurreByName(WhisperMatches.Groups[4].Value);
                    ActivePlayer.Message = WhisperMatches.Groups[7].Value;

                    player = ActivePlayer;

                    chanObject.player = ActivePlayer;
                    args.Channel = WhisperMatches.Groups[2].Value;

                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
                WhisperIncoming = new Regex(YouWhisperRegex, ChannelOptions);
                WhisperMatches = WhisperIncoming.Match(data);
                if (WhisperMatches.Success)
                {
                    player = ConnectedFurre;
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

                ActivePlayer.Message = FontColorRegex.Match(data).Groups[4].Value;
                player = ActivePlayer;
            }
            else if (Color == "emote")
            {
                Regex EmoteRegex = new Regex(EmoteRegexFilter, ChannelOptions);
                Match EmoteMatch = EmoteRegex.Match(data);

                args.Channel = EmoteMatch.Groups[1].Value;

                player = Dream.Furres.GetFurreByName(EmoteMatch.Groups[3].Value);
                player.Message = EmoteMatch.Groups[6].Value;

                chanObject.player = player;

                ProcessServerChannelData?.Invoke(chanObject, args);
                return;
            }
            else if (Color == "notify" || Color == "error")
            {
                string NameStr = "";
                if (chanObject.ChannelText.StartsWith("Players banished from your dreams: "))
                {
                    //Banish-List
                    //[notify> players banished from your Dreams:
                    //`(0:54) When the bot sees the banish list
                    BanishList.Clear();
                    string[] tmp = chanObject.ChannelText.Substring(35).Split(',');
                    foreach (string t in tmp)
                    {
                        if (!string.IsNullOrWhiteSpace(t))
                            BanishList.Add(t);
                    }
                }
                else if (chanObject.ChannelText.Contains("There are no Furres around right now with a name starting with "))
                {
                    //Banish <name> (Furre is not on-line)
                    //Error:>>  There are no Furres around right now with a name starting with (.*?) .

                    Regex t = new Regex("There are no Furres around right now with a name starting with (.*?) .", RegexOptions.Compiled);
                    NameStr = t.Match(data).Groups[1].Value;
                }
                else if (chanObject.ChannelText == "Sorry, this player has not been banished from your Dreams.")
                {
                    //banish-off <name> (Furre is not on-line)
                    //Error:>> Sorry, this player has not been banished from your Dreams.

                    NameStr = BanishName;
                }
                else if (chanObject.ChannelText == "You have not banished anyone.")
                {
                    //banish-off-all (empty List)
                    //Error:>> You have not banished anyone.

                    BanishList.Clear();
                }
                else if (chanObject.ChannelText.StartsWith("The banishment of player "))
                {
                    //banish-off <name> (on list)
                    //[notify> The banishment of player (.*?) has ended.

                    Regex t = new Regex("The banishment of player (.*?) has ended.", RegexOptions.Compiled);
                    NameStr = t.Match(data).Groups[1].Value;
                    ActivePlayer = new Furre(NameStr);
                    player = ActivePlayer;
                    bool found = false;
                    int I;
                    for (I = 0; I <= BanishList.Count - 1; I++)
                    {
                        if (BanishList[I].ToFurcadiaShortName() == player.ShortName)
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
            else if (data.StartsWith("Communication"))
            {
                DisconnectServerAndClientStreams();
            }
            else if (Color == "@cookie")
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
                chanObject.player = Player;
                ProcessServerChannelData?.Invoke(chanObject, args);
                return;
            }
            else //Dynamic Channels
            {
                args.Channel = FontColorRegexMatch.Groups[8].Value;
                if (NameRegex.Match(data).Success)
                {
                    var name = NameRegex.Match(data).Groups[2].Value;
                    player = Dream.Furres.GetFurreByName(name);
                }
            }

            if (string.IsNullOrWhiteSpace(ActivePlayer.Message))
                ActivePlayer.Message = FontColorRegexMatch.Groups[10].Value.Substring(1);

            player = ActivePlayer;

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
            Logger.Debug<ProxySession>(data);
            try
            {
                switch (serverconnectphase)
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
                            serverconnectphase = ConnectionPhase.Auth;
                            ServerStatusChanged?.Invoke(data, new NetServerEventArgs(serverconnectphase, ServerInstructionType.Unknown));
                            // Standalone / account
                            // SendToServer($"connect {sUsername} {sPassword}");

                            if (IsClientSocketConnected)
                            {
                                clientconnectionphase = ConnectionPhase.Auth;
                                ClientStatusChanged?.Invoke(data, new NetClientEventArgs(clientconnectionphase));
                            }
                        }
                        break;

                    case ConnectionPhase.Auth:
                        if (data.StartsWith("]]"))
                        {
                            DisconnectServerAndClientStreams();
                        }
                        else if (data == "&&&&&&&&&&&&&")
                        {
                            //We've connected to Furcadia
                            serverconnectphase = ConnectionPhase.Connected;

                            ServerStatusChanged?.Invoke(data, new NetServerEventArgs(serverconnectphase, ServerInstructionType.Unknown));
                            if (IsClientSocketConnected)
                            {
                                clientconnectionphase = ConnectionPhase.Connected;
                                ClientStatusChanged?.Invoke(this, new NetClientEventArgs(clientconnectionphase));
                            }
                        }
                        break;

                    case ConnectionPhase.Connected:
                        if (string.IsNullOrEmpty(data))
                            return;
                        if (data == "]ccmarbled.pcx")
                        {
                            // safety Check,
                            // dream could be set with (you have arrived in the dream of [FURRE])
                            if (!Dream.JustArrived)
                            {
                                Dream = new Dream();
                                Dream.Furres.Add(new Furre(ConnectedFurreId, ConnectedFurreName));
                            }
                            else
                                Dream.JustArrived = false;

                            var instruction = new BaseServerInstruction(data)
                            {
                                InstructionType = ServerInstructionType.EnterDream
                            };

                            ProcessServerInstruction?.Invoke
                            (
                                instruction,
                                new ParseServerArgs(ServerInstructionType.EnterDream, serverconnectphase)
                            );

                            if (Options.Standalone)
                                CloseFurcadiaClient();
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
                                player.FurreColors = new ColorString(data.Substring(2, ColorString.ColorStringSize));

                                if (IsConnectedCharacter())
                                    Look = false;

                                UpdateColorString Instruction = new UpdateColorString(player, data)
                                {
                                    InstructionType = ServerInstructionType.LookResponse
                                };

                                ProcessServerInstruction?.Invoke
                                (
                                    Instruction,
                                    new ParseServerArgs(ServerInstructionType.LookResponse, serverconnectphase)
                                );
                            };
                        }

                        //Spawn Avatar
                        else if (data[0] == '<')
                        {
                            SpawnAvatar FurreSpawn = new SpawnAvatar(data);
                            Logger.Debug<ProxySession>(FurreSpawn);

                            player = FurreSpawn.Player;

                            Dream.Furres.Add(Player);

                            //New furre Arrival to current Dream
                            if (FurreSpawn.PlayerFlags.HasFlag(CHAR_FLAG_NEW_AVATAR))
                            {
                                if (!Dream.Furres.Contains(player))
                                {
                                    ProcessServerInstruction?.Invoke(
                                        FurreSpawn,
                                        new ParseServerArgs(ServerInstructionType.SpawnAvatar, serverconnectphase));
                                }
                            }
                        }
                        //Remove Furre
                        else if (data[0] == ')' || data[0] == ' ')
                        {
                            RemoveAvatar RemoveFurre = new RemoveAvatar(data);
                            RemoveFurre.Player = Dream.Furres.GetFurreByID(RemoveFurre.FurreId);

                            if (Dream.Furres.Contains(RemoveFurre.Player))
                            {
                                Dream.Furres.Remove(RemoveFurre.Player);

                                Handled = true;
                                Logger.Debug<ProxySession>(RemoveFurre);
                                ProcessServerInstruction?.Invoke
                                (
                                    RemoveFurre,
                                    new ParseServerArgs(ServerInstructionType.RemoveAvatar, serverconnectphase)
                                );
                            }
                            return;
                        }
                        //Animated Move
                        else if (data[0] == '/')
                        {
                            player = Dream.Furres.GetFurreByID(data.Substring(1, 4));

                            if (player.FurreID < 1 || player.ShortName == "unknown")
                                throw new ArgumentOutOfRangeException("Player", player, "Player not found in dream.");
                            if (ConnectedFurre.FurreID < 1)
                                throw new ArgumentOutOfRangeException("ConnectedFurre", ConnectedFurre, "ConnectedFurre not found in dream.");

                            player.Location = new FurrePosition(data.Substring(5, 4));

                            ViewArea VisableRectangle = new ViewArea(ConnectedFurre.Location);

                            // TODO: Re-factor tis mess some how... -Gero
                            if (
                                VisableRectangle.FurreLocation.X <= player.Location.X
                                && VisableRectangle.FurreLocation.Y <= player.Location.Y
                                && VisableRectangle.Height >= player.Location.Y
                                && VisableRectangle.Length >= player.Location.X
                                )
                            {
                                player.InRange = true;
                            }
                            else
                            {
                                player.InRange = false;
                            }

                            var FurreMoved = new MoveFurre(data)
                            {
                                Player = player
                            };

                            Logger.Debug<ProxySession>(FurreMoved);
                            ProcessServerInstruction?.Invoke(FurreMoved,
                                  new ParseServerArgs(ServerInstructionType.AnimatedMoveAvatar, serverconnectphase));

                            return;
                        }
                        // Move Avatar
                        else if (data[0] == 'A')
                        {
                            player = Dream.Furres.GetFurreByID(data.Substring(1, 4));

                            if (player.FurreID < 1 || player.ShortName == "unknown")
                                throw new ArgumentOutOfRangeException("Player", player, "Player not found in dream.");

                            player.Location = new FurrePosition(data.Substring(5, 4));

                            if (ConnectedFurre.FurreID < 1)
                                throw new ArgumentOutOfRangeException("ConnectedFurre", ConnectedFurre, "ConnectedFurre not found in dream.");

                            //connectedFurre = (Furre)Dream.Furres[connectedFurre];
                            ViewArea VisableRectangle = new ViewArea(ConnectedFurre.Location);

                            // TODO: Refactor this mess some how... -Gero
                            if (VisableRectangle.FurreLocation.X <= player.Location.X
                                && VisableRectangle.FurreLocation.Y <= player.Location.Y
                                && VisableRectangle.Height >= player.Location.Y
                                && VisableRectangle.Length >= player.Location.X
                                )
                            {
                                player.InRange = true;
                            }
                            else
                            {
                                player.InRange = false;
                            }
                            MoveFurre FurreMoved = new MoveFurre(data)
                            {
                                Player = player
                            };
                            Logger.Debug<ProxySession>(FurreMoved);
                            ProcessServerInstruction?.Invoke(FurreMoved,
                                  new ParseServerArgs(ServerInstructionType.MoveAvatar, serverconnectphase));
                            return;
                        }
                        //Update ColorString
                        else if (data[0] == 'B')
                        {
                            //fuid 4 b220 bytes
                            player = Dream.Furres.GetFurreByID(data.Substring(1, 4));
                            UpdateColorString ColorStringUpdate = new UpdateColorString(player, data);

                            if (InDream)
                            {
                                ProcessServerInstruction?.Invoke(ColorStringUpdate,
                                new ParseServerArgs(ServerInstructionType.UpdateColorString, serverconnectphase));
                            }
                        }
                        //Hide Avatar
                        else if (data[0] == 'C')
                        {
                            player = Dream.Furres.GetFurreByID(data.Substring(1, 4));
                            player.Location = new FurrePosition(data.Substring(5, 4));
                            // player.InRange = false;
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
                            player.Message = m.Groups[3].Value;

                            //]s(.+)1 (.*?) (.*?) 0
                        }

                        //Disconnection Error
                        else if (data[0] == '[')
                        {
                            //#if DEBUG
                            //                        Console.WriteLine("Disconnection Dialog:" + data);
                            //#endif

                            Dream.Furres.Clear();

                            //;{mapfile}	Load a local map (one in the furcadia folder)
                            //]q {name} {id}	Request to download a specific patch
                        }
                        //else if (data.StartsWith(";") || data.StartsWith("]r"))
                        //{
                        //}

                        //Dream Load ]q
                        //vasecodegamma
                        else if (data.StartsWith("]q"))
                        {
                            //Set defaults (should move to some where else?
                            hasShare = false;
                            NoEndurance = false;
                            LoadDream loadDream = new LoadDream(data);
                            Dream.Load(loadDream);
                            Logger.Debug<ProxySession>($"Load Dream {loadDream}");

                            ProcessServerInstruction?.Invoke(loadDream,
                               new ParseServerArgs(ServerInstructionType.LoadDreamEvent, serverconnectphase));

                            // Set Proxy to Stand-Alone Operation

                            if (Options.Standalone)
                            {
                                SendToServer("vascodagama");
                                SendToServer("dreambookmark 0");
                            }
                            return;
                        }
                        // ]z UID[*]
                        // Unique User ID
                        // This instruction is sent as a response to the uid command. The purpose of this is unclear.
                        // Credits Artex, FTR
                        else if (data.StartsWith("]z"))
                        {
                            ProcessServerInstruction?.Invoke(new BaseServerInstruction(data),
                                new ParseServerArgs(ServerInstructionType.UniqueUserId, serverconnectphase));
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
                                new ParseServerArgs(ServerInstructionType.SetOwnId, serverconnectphase)
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
                                Logger.Debug<ProxySession>($"Dream Bookmark: {bookmark}");
                                ProcessServerInstruction?.Invoke
                                    (
                                        bookmark,
                                        new ParseServerArgs(ServerInstructionType.BookmarkDream, serverconnectphase)
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
                            else if (data.StartsWith("((You enter the dream of "))
                            {
                                Dream = new Dream(data.Replace("((You enter the dream of ", string.Empty).Replace(".)", string.Empty));
                                Dream.Furres.Add(new Furre(ConnectedFurreId, ConnectedFurreName));
                                // Handled = true;
                            }
                            // Send the Data to Channel Parser
                            ParseServerChannel(data.Substring(1), Handled);
                            return;
                        }

                        break;

                    case ConnectionPhase.Disconnected:
                        {
                            ServerStatusChanged?.Invoke(null,
                                        new NetServerEventArgs(serverconnectphase, ServerInstructionType.None));
                            break;
                        }
                    // Do nothing - we're disconnected...
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                SendError(ex, this);
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
            Logger.Debug<ProxySession>(data);
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

            FormatTextToServer(ref data);
            SendToServer(data);
        }

        /// <summary>
        /// Send a raw instruction to the client
        /// </summary>
        /// <param name="data">
        /// </param>
        public override void SendToClient(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return;
            Logger.Debug<ProxySession>(data);
            if (IsClientSocketConnected && clientconnectionphase != ConnectionPhase.Disconnected)
                base.SendToClient(data);
        }

        /// <summary>
        /// Send a raw instruction to Server through the Load Balancer
        /// </summary>
        /// <param name="message">
        /// Client to server Instruction
        /// </param>
        public override void SendToServer(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            Logger.Debug<ProxySession>(message);
            if (serverconnectphase != ConnectionPhase.Disconnected)
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
        public void FormatTextToServer(ref string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return;
            Logger.Debug<ProxySession>(data);
            //Clean Text input to match Client handling
            string result = "";
            switch (data[0])
            {
                case '`':
                    result = data.Remove(0, 1);
                    break;

                case '/':
                    result = $"wh {data.Substring(1)}";
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
            data = result;
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
                base.Dispose();
                ServerBalancer.Dispose();
            }

            // Free any unmanaged objects here.
            disposed = true;
        }

        private void Initilize()
        {
#if DEBUG
            Logger.Disable<NetProxy>();
            // if (!Debugger.IsAttached)
            // Logger.Disable<ProxySession>();
#else
            Logger.Disable<ProxySession>();
#endif
            SpeciesTag = new Queue<string>();
            BadgeTag = new Queue<string>();

            serverconnectphase = ConnectionPhase.Init;
            clientconnectionphase = ConnectionPhase.Init;

            ServerBalancer = new Utils.ServerQue();
            ServerBalancer.OnServerSendMessage += (o, e) => OnServerQueSent(o, e);
            ServerBalancer.TroatTiredEventHandler += e => OnThroatTiredTrigger(e);
            base.ServerData2 += e => OnServerDataReceived(e);
            ServerConnected += () => OnServerConnected();
            ServerDisconnected += () => OnServerDisonnected();

            base.ClientData2 += e => ParseClientData(e);
            ClientDisconnected += () => OnClientDisconnected();
            ClientConnected += () => OnClientConnected();

            player = new Furre();
            Dream = new Dream()
            {
                JustArrived = false
            };

            //BadgeTag = new Queue<string>(50);
            LookQue = new Queue<string>(50);

            //          SpeciesTag = new Queue<string>(50);
            //          BanishString = new List<string>(50);
        }

        /// <summary>
        /// Called when [throat tired trigger].
        /// </summary>
        /// <param name="stat">if set to <c>true</c> [stat].</param>
        public void OnThroatTiredTrigger(bool stat)
        {
            TroatTiredEventHandler?.Invoke(stat);
        }

        private void OnClientConnected()
        {
            clientconnectionphase = ConnectionPhase.MOTD;
            ClientStatusChanged?.Invoke(this, new NetClientEventArgs(clientconnectionphase));
        }

        private void OnClientDisconnected()
        {
            clientconnectionphase = ConnectionPhase.Disconnected;
            ClientStatusChanged?.Invoke(this, new NetClientEventArgs(clientconnectionphase));
        }

        private void OnServerConnected()
        {
            serverconnectphase = ConnectionPhase.MOTD;
            ServerStatusChanged?.Invoke(this, new NetServerEventArgs(serverconnectphase, ServerInstructionType.Unknown));
        }

        /// <summary>
        /// </summary>
        /// <param name="data">
        /// </param>
        private void OnServerDataReceived(string data)
        {
            player = new Furre();
            bool handled = false;
            ParseServerData(data, ref handled);

            if (!handled)
                ServerData2?.Invoke(data);
        }

        private void OnServerDisonnected()
        {
            serverconnectphase = ConnectionPhase.Disconnected;
            ServerStatusChanged?.Invoke(this, new NetServerEventArgs(serverconnectphase, ServerInstructionType.Unknown));
        }

        private void OnServerQueSent(object o, EventArgs e)
        {
            if (o != null)
                base.SendToServer(o.ToString());
        }

        /// <summary>
        /// Client sent us some data, Let's deal with it
        /// </summary>
        /// <param name="data">
        /// </param>
        private void ParseClientData(string data)
        {
            Logger.Debug<ProxySession>(data);

            switch (clientconnectionphase)
            {
                case ConnectionPhase.Auth:
                    var ConnectString = data.Split(new char[] { ' ' }, 5);
                    switch (ConnectString[0])
                    {
                        // connect characterName password machineID
                        case "connect":
                            ConnectedFurreName = ConnectString[1];
                            break;

                        // account email characterName password token
                        case "account":
                            ConnectedFurreName = ConnectString[2];
                            break;
                    }

                    ClientData2?.Invoke(data);
                    break;

                // Disconnected? Do Nohing
                case ConnectionPhase.Disconnected:
                    break;

                case ConnectionPhase.Connected:
                    // Snag the quit command from the client so we can
                    // handle the connection properly
                    if (data == "quit")
                    {
                        clientconnectionphase = ConnectionPhase.Disconnected;
                        ClientStatusChanged?.Invoke(this, new NetClientEventArgs(clientconnectionphase));
                        if (Options.Standalone)
                        {
                            DisconnectClientStream();
                        }
                        else
                            DisconnectServerAndClientStreams();
                    }
                    if (IsClientSocketConnected)
                        ClientData2?.Invoke(data);
                    break;

                case ConnectionPhase.Connecting:
                default:
                    ClientData2?.Invoke(data);
                    break;
            }
        }

        #endregion Private Methods
    }
}