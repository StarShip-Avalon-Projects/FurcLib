﻿using Extentions;
using Furcadia.Drawing;
using Furcadia.Extensions;
using Furcadia.Logging;
using Furcadia.Movement;
using Furcadia.Net.DreamInfo;
using Furcadia.Net.Options;
using Furcadia.Net.Utils.ChannelObjects;
using Furcadia.Net.Utils.ServerObjects;
using Furcadia.Net.Utils.ServerParser;
using Furcadia.Text;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Furcadia.Movement.CharacterFlags;
using static Furcadia.Net.DirectConnection.ClientBase;
using static Furcadia.Net.Proxy.NetProxy;
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
    public class ProxySession : IDisposable
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

        private static Furre player;

        ///// <summary>
        ///// Beekin Badge
        ///// </summary>
        private Queue<string> BadgeTag;

        private object ChannelLock = new object();

        private RegexOptions ChannelOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;
        private bool disposed = false;
        private bool Look;

        private Queue<string> LookQue;

        /// <summary>
        /// Balance the out going load to server
        /// <para>
        /// Throat Tired Syndrome and No Endurance Control
        /// </para>
        /// </summary>
        private Utils.ServerQue ServerBalancer;

        private Queue<string> SpeciesTag;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        public ProxySession() : this(new ProxyOptions())
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="Options">
        /// ProxySession Options
        /// </param>
        public ProxySession(ProxyOptions Options)
        {
            this.Options = Options;
            Proxy = new NetProxy(this.Options);
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
        /// </summary>
        public delegate void DataEventHandler2(string data);

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
        ///
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="o">The o.</param>
        public delegate void ErrorEventHandler(Exception e, Object o);

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
        public event DataEventHandler2 ClientData2;

        /// <summary>
        ///This is triggered when the Server Disconnects
        /// </summary>
        public event ActionDelegate ClientDisconnected;

        /// <summary>
        /// Track the Furcadia Client status
        /// </summary>
        public event ClientStatusChangedEventHandler ClientStatusChanged;

        /// <summary>
        /// This is triggered when a handled Exception is thrown.
        /// </summary>
        public event ErrorEventHandler Error;

        /// <summary>
        /// Process Display chanObject.ChannelText and Channels
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
        /// Current Connection Phase
        /// </summary>
        public ConnectionPhase ClientConnectPhase { get; private set; }

        /// <summary>
        /// Client Connection status
        /// </summary>
        /// <returns>
        /// Status tog the Furcadia Client
        /// </returns>
        public ConnectionPhase ClientStatus
        {
            get => ClientConnectPhase;
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
                var furre = Furres.GetFurreByName(ConnectedFurreName);
                if (furre.FurreID <= 0)
                    furre.FurreID = ConnectedFurreId;
                return furre;
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
        public Dream Dream { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [furcadia client is running].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [furcadia client is running]; otherwise, <c>false</c>.
        /// </value>
        public bool FurcadiaClientIsRunning => Proxy.FurcadiaClientIsRunning;

        /// <summary>
        /// Gets the furres.
        /// </summary>
        /// <value>
        /// The furres.
        /// </value>
        public FurreList Furres { get; private set; }

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
        /// Gets a value indicating whether this instance is client socket connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is client socket connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsClientSocketConnected
        {
            get => Proxy.IsClientSocketConnected;
        }

        /// <summary>
        /// Gets a value indicating whether the Proxy socket is connected to the game server
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is server socket connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsServerSocketConnected
        {
            get => Proxy.IsServerSocketConnected;
        }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public ProxyOptions Options { get; set; }

        /// <summary>
        /// Current Triggering player
        /// </summary>
        public Furre Player
        {
            get => player;
            private set => player = value;
        }

        /// <summary>
        /// Gets the proxy.
        /// </summary>
        /// <value>
        /// The proxy.
        /// </value>
        public NetProxy Proxy { get; internal set; }

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
        public void Connect()
        {
            ServerConnectPhase = ConnectionPhase.Connecting;
            ClientConnectPhase = ConnectionPhase.Connecting;
            ServerStatusChanged?.Invoke(this, new NetServerEventArgs(ServerConnectPhase));
            ClientStatusChanged?.Invoke(this, new NetClientEventArgs(ClientConnectPhase));
            Proxy.Connect();
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            Furres.Clear();
            Dream.Clear();
            Proxy.DisconnectServerAndClientStreams();
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
            if (Handled)
                return;
            ChannelObject chanObject = new ChannelObject(data);
            ParseChannelArgs args = new ParseChannelArgs(data);
            Logger.Debug<ProxySession>(data);
            Regex FontColorRegex = new Regex(FontChannelFilter, RegexOptions.Compiled | RegexOptions.CultureInvariant);
            Match FontColorRegexMatch = FontColorRegex.Match(data);
            Match DescTagRegexMatch = DescTagRegex.Match(data);

            string Color = FontColorRegexMatch.Groups[1].Value;

            Regex DescRegex = new Regex(DescFilter, ChannelOptions);
            if (DescRegex.Match(data).Success)
            {
                player = Furres.GetFurreByName(DescRegex.Match(data).Groups[1].Value);
                player.FurreDescription = DescRegex.Match(data).Groups[2].Value;

                if (LookQue.Count > 0)
                {
                    player.FurreColors = new ColorString(LookQue.Dequeue());
                }

                chanObject.player = player;
                args.Channel = "desc";

                ProcessServerChannelData?.Invoke(chanObject, args);
                return;
            }
            //<a href='command://(.*?)'>click here</a> or type (`.*?) and press &lt;enter&gt;.</font>

            Match QueryMatch = QueryCommand.Match(data);
            if (QueryMatch.Success)
            {
                chanObject = new QueryChannelObject(data);
                player = Furres.GetFurreByName(chanObject.Player.ShortName);

                Furres.Update(player);

                chanObject.player = player;
                args.Channel = QueryMatch.Groups[1].Value;

                ProcessServerChannelData?.Invoke(chanObject, args);
                return;
            }

            Regex ShoutRegex = new Regex(ShoutRegexFilter, ChannelOptions);
            Match ShoutMatch = ShoutRegex.Match(data);
            if (ShoutMatch.Success)
            {
                args.Channel = ShoutMatch.Groups[2].Value;

                player = Furres.GetFurreByName(ShoutMatch.Groups[5].Value);
                player.Message = ShoutMatch.Groups[9].Value;

                Furres.Update(player);

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

                Furres.Update(player);

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
                    HasShare = true;
                }
                else if (chanObject.ChannelText.EndsWith("is now sharing control of this Dream with you."))
                {
                    HasShare = true;
                }
                else if (chanObject.ChannelText.EndsWith("has stopped sharing control of this Dream with you."))
                {
                    HasShare = false;
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
                player = ConnectedFurre;

                //TODO: Test Active Player
                Regex t = new Regex(YouSayFilter);

                player.Message = t.Match(data).Groups[2].Value;
                chanObject.player = player;
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
                        player = Furres.GetFurreByName(SayMatch.Groups[2].Value);
                        player.Message = SayMatch.Groups[3].Value;

                        Furres.Update(player);

                        chanObject.player = player;

                        ProcessServerChannelData?.Invoke(chanObject, args);
                        return;
                    }
                    else
                    {
                        // Process everything as raw text
                        args.Channel = "text";

                        player.Message = data;
                        chanObject.player = player;

                        ProcessServerChannelData?.Invoke(chanObject, args);
                        return;
                    }
                }
                else // You see [FURRE]
                {
                    var nameMatch = NameRegex.Match(data);
                    player = Furres.GetFurreByName(nameMatch.Groups[2].Value);

                    Furres.Update(player);

                    chanObject.player = player;
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
                    player = Furres.GetFurreByName(WhisperMatches.Groups[4].Value);
                    player.Message = WhisperMatches.Groups[7].Value;

                    Furres.Update(player);

                    chanObject.player = player;
                    args.Channel = WhisperMatches.Groups[2].Value;

                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
                WhisperIncoming = new Regex(YouWhisperRegex, ChannelOptions);
                WhisperMatches = WhisperIncoming.Match(data);
                if (WhisperMatches.Success)
                {
                    player = ConnectedFurre;
                    player.Message = WhisperMatches.Groups[4].Value;

                    Furres.Update(player);

                    chanObject = new ChannelObject(data, player);
                    args.Channel = WhisperMatches.Groups[2].Value;

                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
            }
            else if (Color == "emote")
            {
                Regex EmoteRegex = new Regex(EmoteRegexFilter, ChannelOptions);
                Match EmoteMatch = EmoteRegex.Match(data);

                args.Channel = EmoteMatch.Groups[1].Value;

                player = Furres.GetFurreByName(EmoteMatch.Groups[3].Value);

                player.Message = EmoteMatch.Groups[6].Value;
                chanObject.player = player;

                Furres.Update(player);

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
                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
                else if (chanObject.ChannelText.Contains("There are no Furres around right now with a name starting with "))
                {
                    //Banish <name> (Furre is not on-line)
                    //Error:>>  There are no Furres around right now with a name starting with (.*?) .

                    Regex t = new Regex("There are no Furres around right now with a name starting with (.*?) .", RegexOptions.Compiled);
                    NameStr = t.Match(data).Groups[1].Value;
                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
                else if (chanObject.ChannelText == "Sorry, this player has not been banished from your Dreams.")
                {
                    //banish-off <name> (Furre is not on-line)
                    //Error:>> Sorry, this player has not been banished from your Dreams.

                    NameStr = BanishName;
                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
                else if (chanObject.ChannelText == "You have not banished anyone.")
                {
                    //banish-off-all (empty List)
                    //Error:>> You have not banished anyone.

                    BanishList.Clear();
                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
                else if (chanObject.ChannelText.StartsWith("The banishment of player "))
                {
                    //banish-off <name> (on list)
                    //[notify> The banishment of player (.*?) has ended.

                    Regex t = new Regex("The banishment of player (.*?) has ended.", RegexOptions.Compiled);
                    NameStr = t.Match(data).Groups[1].Value;
                    player = new Furre(0, NameStr);
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
                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
            }
            else if (data.StartsWith("Communication"))
            {
                Disconnect();
            }
            else if (DescTagRegexMatch.Success)
            {
                // These are part of joining a dream. They are Game Server settings
                if (DescTagRegexMatch.Groups[1].Value == "fsh://system.fsh:86")
                {
                    player = new Furre("Furcadia game server")
                        ;
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
                    player.Message = DescTagRegexMatch.Groups[6].Value;
                    //LineCountRegex = "<img src='fsh://system.fsh:86' /> Dream Standard: <a href='http://www.furcadia.com/standards/'>(.*)</a>";
                    //LineCount = new Regex(LineCountRegex);
                    //if (LineCount.Match(data).Success)
                    //    Dream.Title = LineCount.Match(data).Groups[1].Value;
                    chanObject.player = player;
                    args.Channel = "@emit";

                    Furres.Update(player);

                    ProcessServerChannelData?.Invoke(chanObject, args);
                    return;
                }
            }
            else //Dynamic Channels
            {
                args.Channel = FontColorRegexMatch.Groups[9].Value;
                if (NameRegex.Match(data).Success)
                {
                    if (Furres.Contains(player))
                        Furres[player] = player;
                }
                else
                {
                    player = new Furre("Furcadia game server");
                }

                player.Message = FontColorRegexMatch.Groups[10].Value;

                Furres.Update(player);

                ProcessServerChannelData?.Invoke(chanObject, args);
                return;
            }
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
                            ServerStatusChanged?.Invoke(data, new NetServerEventArgs(ServerConnectPhase, ServerInstructionType.Unknown));
                            // Standalone / account
                            // SendToServer($"connect {sUsername} {sPassword}");

                            if (Proxy.IsClientSocketConnected)
                            {
                                ClientConnectPhase = ConnectionPhase.Auth;
                                ClientStatusChanged?.Invoke(data, new NetClientEventArgs(ClientConnectPhase));
                            }
                        }
                        break;

                    case ConnectionPhase.Auth:
                        if (data.StartsWith("]]"))
                        {
                            Proxy.DisconnectServerAndClientStreams();
                        }
                        else if (data == "&&&&&&&&&&&&&")
                        {
                            //We've connected to Furcadia
                            ServerConnectPhase = ConnectionPhase.Connected;

                            ServerStatusChanged?.Invoke(data, new NetServerEventArgs(ServerConnectPhase, ServerInstructionType.Unknown));
                            if (Proxy.IsClientSocketConnected)
                            {
                                ClientConnectPhase = ConnectionPhase.Connected;
                                ClientStatusChanged?.Invoke(this, new NetClientEventArgs(ClientConnectPhase));
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
                                Furres.Clear();
                                Furres.Add(new Furre(ConnectedFurreId, ConnectedFurreName));
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
                                new ParseServerArgs(ServerInstructionType.EnterDream, ServerConnectPhase)
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
                                    new ParseServerArgs(ServerInstructionType.LookResponse, ServerConnectPhase)
                                );
                            };
                        }

                        //Spawn Avatar
                        else if (data[0] == '<')
                        {
                            SpawnAvatar FurreSpawn = new SpawnAvatar(data);
                            Logger.Debug<ProxySession>(FurreSpawn);

                            player = FurreSpawn.Player;

                            //New furre Arrival to current Dream
                            if (!Furres.Contains(player) && FurreSpawn.PlayerFlags.HasFlag(CHAR_FLAG_NEW_AVATAR))
                            {
                                Furres.Add(player);
                                ProcessServerInstruction?.Invoke(
                                    FurreSpawn,
                                    new ParseServerArgs(ServerInstructionType.SpawnAvatar, ServerConnectPhase));
                            }
                            if (!Furres.Contains(player))
                                Furres.Add(player);
                            //else
                            //    Furres[Furres.IndexOf(player)] = player;
                            return;
                        }
                        //Remove Furre
                        else if (data[0] == ')' || data[0] == ' ')
                        {
                            RemoveAvatar RemoveFurre = new RemoveAvatar(data);
                            player = Furres.GetFurreByID(RemoveFurre.FurreId);
                            RemoveFurre.Player = player;
                            if (Furres.Contains(RemoveFurre.FurreId))
                            {
                                Furres.Remove(player);
                                Handled = true;
                                Logger.Debug<ProxySession>(RemoveFurre);
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

                            if (player.FurreID < 1 || player.ShortName == "unknown")
                                throw new ArgumentOutOfRangeException("Player", player, "Player not found in dream.");

                            var FurreMoved = new MoveFurre(data, ref player);

                            if (ConnectedFurre.FurreID < 1)
                                throw new ArgumentOutOfRangeException("ConnectedFurre", ConnectedFurre, "ConnectedFurre not found in dream.");

                            ViewArea ConnectedFurreVisibleRectabgle = new ViewArea(ConnectedFurre.Location);

                            player.InRange = ConnectedFurreVisibleRectabgle.InRange(player.Location);

                            Furres.Update(player);

                            Logger.Debug<ProxySession>(FurreMoved);
                            ProcessServerInstruction?.Invoke(FurreMoved,
                                  new ParseServerArgs(FurreMoved.InstructionType, ServerConnectPhase));

                            return;
                        }

                        //Update ColorString
                        else if (data[0] == 'B')
                        {
                            //fuid 4 b220 bytes
                            player = Furres.GetFurreByID(data.Substring(1, 4));
                            UpdateColorString ColorStringUpdate = new UpdateColorString(player, data);

                            if (InDream)
                            {
                                Furres.Update(player);

                                ProcessServerInstruction?.Invoke(ColorStringUpdate,
                                new ParseServerArgs(ServerInstructionType.UpdateColorString, ServerConnectPhase));
                            }
                        }
                        //Hide Avatar
                        else if (data[0] == 'C')
                        {
                            player = Furres.GetFurreByID(data.Substring(1, 4));

                            player.Location = new FurrePosition(data.Substring(5, 4));
                            player.Visible = false;

                            Furres.Update(player);
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

                            Furres.Clear();

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
                            HasShare = false;
                            NoEndurance = false;
                            LoadDream loadDream = new LoadDream(data);
                            Dream.Load(loadDream);
                            Logger.Debug<ProxySession>($"Load Dream {loadDream}");

                            ProcessServerInstruction?.Invoke(loadDream,
                               new ParseServerArgs(ServerInstructionType.LoadDreamEvent, ServerConnectPhase));

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
                                Logger.Debug<ProxySession>($"Dream Bookmark: {bookmark}");
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
                            else if (data.StartsWith("((You enter the dream of "))
                            {
                                Dream = new Dream(data.Replace("((You enter the dream of ", string.Empty).Replace(".)", string.Empty));
                                Furres.Clear();
                                Furres.Add(new Furre(ConnectedFurreId, ConnectedFurreName));
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
                                        new NetServerEventArgs(ServerConnectPhase, ServerInstructionType.None));
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
        /// Closes the furcadia client.
        /// </summary>
        public void CloseFurcadiaClient()
        {
            Proxy.CloseFurcadiaClient();
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
        public void SendToClient(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return;
            Logger.Debug<ProxySession>(data);
            if (Proxy.IsClientSocketConnected && ClientConnectPhase != ConnectionPhase.Disconnected)
                Proxy.SendToClient(data);
        }

        /// <summary>
        /// Send a raw instruction to Server through the Load Balancer
        /// </summary>
        /// <param name="message">
        /// Client to server Instruction
        /// </param>
        public void SendToServer(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            Logger.Debug<ProxySession>(message);
            if (ServerConnectPhase != ConnectionPhase.Disconnected)
                ServerBalancer.SendToServer(message);
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
                Proxy.Dispose();
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

            ServerConnectPhase = ConnectionPhase.Init;
            ClientConnectPhase = ConnectionPhase.Init;

            ServerBalancer = new Utils.ServerQue();
            ServerBalancer.OnServerSendMessage += (o, e) => OnServerQueSent(o, e);
            ServerBalancer.TroatTiredEventHandler += e => OnThroatTiredTrigger(e);

            Proxy.Error += (e, v) => OnError(e, v);

            Proxy.ServerData2 += e => OnServerDataReceived(e);
            Proxy.ServerConnected += () => OnServerConnected();
            Proxy.ServerDisconnected += () => OnServerDisonnected();

            Proxy.ClientData2 += e => ParseClientData(e);
            Proxy.ClientDisconnected += () => OnClientDisconnected();
            Proxy.ClientConnected += () => OnClientConnected();

            player = new Furre();
            if (Furres is null)
                Furres = new FurreList(100);
            if (Dream is null)
                Dream = new Dream()
                {
                    JustArrived = false
                };

            //BadgeTag = new Queue<string>(50);
            LookQue = new Queue<string>(50);

            //          SpeciesTag = new Queue<string>(50);
            //          BanishString = new List<string>(50);
        }

        private void OnClientConnected()
        {
            ClientConnectPhase = ConnectionPhase.MOTD;
            ClientStatusChanged?.Invoke(this, new NetClientEventArgs(ClientConnectPhase));
        }

        private void OnClientDisconnected()
        {
            ClientConnectPhase = ConnectionPhase.Disconnected;
            ClientDisconnected?.Invoke();
            ClientStatusChanged?.Invoke(this, new NetClientEventArgs(ClientConnectPhase));
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
            player = new Furre();
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
            ServerStatusChanged?.Invoke(this, new NetServerEventArgs(ServerConnectPhase, ServerInstructionType.Unknown));
        }

        private void OnServerQueSent(object o, EventArgs e)
        {
            if (o != null)
                Proxy.SendToServer(o.ToString());
        }

        /// <summary>
        /// Client sent us some data, Let's deal with it
        /// </summary>
        /// <param name="data">
        /// </param>
        private void ParseClientData(string data)
        {
            Logger.Debug<ProxySession>(data);

            switch (ClientConnectPhase)
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
                        ClientConnectPhase = ConnectionPhase.Disconnected;
                        ClientStatusChanged?.Invoke(this, new NetClientEventArgs(ClientConnectPhase));
                        if (Options.Standalone)
                        {
                            Proxy.DisconnectClientStream();
                        }
                        else
                            Proxy.DisconnectServerAndClientStreams();
                    }
                    if (Proxy.IsClientSocketConnected)
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