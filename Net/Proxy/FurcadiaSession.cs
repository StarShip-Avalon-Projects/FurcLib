using Furcadia.Drawing;
using Furcadia.Movement;
using Furcadia.Net.Utils.ServerParser;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//Furcadia Servver Parser
//Event/Delegates for server instructions
//call subsystem processor

//dream info
//Furre info
//Bot info

//Furre Update events?

using System.Threading;
using static Furcadia.Drawing.VisibleArea;
using static Furcadia.Movement.CharacterFlags;
using static Furcadia.Text.Base220;
using static Furcadia.Util;

namespace Furcadia.Net
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
    /// Part3: This Class Links loosley to the GUI
    /// </para>
    /// </summary>
    public class FurcadiaSession : NetProxy, IDisposable
    {
        #region Private Fields

        private string banishName = "";
        private List<string> banishString = new List<string>();

        /// <summary>
        /// Furre Name we connected with
        /// </summary>
        private string botName;

        private int botUID;
        private string channel;
        private object ChannelLock = new object();
        private bool clientClose = false;
        private ConnectionPhase clientconnectionphase;
        private object clientlock = new object();
        private int DataReceived = 0;
        private bool disposed = false;
        private DREAM dream;
        private string errorMsg = "";
        private short errorNum = 0;
        private bool hasShare;
        private bool inDream = false;
        private bool Look;
        private Queue<string> LookQue = new Queue<string>();
        private bool newData = false;
        private FURRE player;

        //Property?
        private bool procExit;

        /// <summary>
        /// Manage out Auto reconnects
        /// </summary>
        private Utils.ProxyReconnect ReconnectionManager;

        /// <summary>
        /// Balance thhe out going load to server
        /// <para>
        /// Throat Tired Syndrome and No Endurance Control
        /// </para>
        /// </summary>
        private Utils.ServerQue ServerBalancer;

        private Queue<string> SpeciesTag = new Queue<string>();

        #endregion Private Fields

        #region "RegEx filters"

        /// <summary>
        /// </summary>
        public const string ChannelNameFilter = "<channel name='(.*?)' />";

        /// <summary>
        /// </summary>
        public const string CookieToMeREGEX = "<name shortname='(.*?)'>(.*?)</name> just gave you";

        /// <summary>
        /// </summary>
        public const string DescFilter = "<desc shortname='([^']*)' />(.*)";

        /// <summary>
        /// </summary>
        public const string DiceFilter = "^<font color='roll'><img src='fsh://system.fsh:101' alt='@roll' /><channel name='@roll' /> <name shortname='([^ ]+)'>([^ ]+)</name> rolls (\\d+)d(\\d+)((-|\\+)\\d+)? ?(.*) & gets (\\d+)\\.</font>$";

        /// <summary>
        /// </summary>
        public const string EntryFilter = "^<font color='([^']*?)'>(.*?)</font>$";

        /// <summary>
        /// </summary>
        public const string Iconfilter = "<img src='fsh://system.fsh:([^']*)'(.*?)/>";

        /// <summary>
        /// </summary>
        public const string NameFilter = "<name shortname='([^']*)' ?(.*?)?>([\\x21-\\x3B\\=\\x3F-\\x7E]+)</name>";

        /// <summary>
        /// </summary>
        public const string YouSayFilter = "You ([\\x21-\\x3B\\=\\x3F-\\x7E]+), \"([^']*)\"";

        #endregion "RegEx filters"

        #region "Public Events"

        /// <summary>
        /// </summary>
        /// <param name="Sender">
        /// </param>
        /// <param name="e">
        /// </param>
        public delegate void ClientStatusChangedEventHandler(ref object Sender, NetClientEventArgs e);

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        public delegate void OnErrorEventHandler(object sender, EventArgs e);

        /// <summary>
        /// </summary>
        /// <param name="Sender">
        /// </param>
        /// <param name="e">
        /// </param>
        public delegate void ServerStatusChangedEventHandler(object Sender, NetServerEventArgs e);

        /// <summary>
        /// Track the Furcadia Client status
        /// </summary>
        public event ClientStatusChangedEventHandler ClientStatusChanged;

        /// <summary>
        /// Error Event Handler
        /// </summary>
        public event OnErrorEventHandler OnError;

        /// <summary>
        /// Track the Server Status
        /// </summary>
        public event ServerStatusChangedEventHandler ServerStatusChanged;

        #region "Client/Server Fata Handling"

        /// <summary>
        /// Send Data to Furcadia Client or Game Server
        /// </summary>
        /// <param name="Message">
        /// Raw instruction to send
        /// </param>
        /// <param name="e">
        /// Cliemt or Server Event Argumentss with Instruction type
        /// </param>
        public delegate void DataHandler(string Message, EventArgs e);

        /// <summary>
        /// OProcess the Data sent to the Furcadia Client
        /// </summary>
        /// <param name="Message">
        /// </param>
        /// <param name="Arg">
        /// </param>
        public delegate void ProcessClientData(string Message, EventArgs Arg);

        /// <summary>
        /// Send Server to Client Instruction object to Subclassed for handlings
        /// </summary>
        /// <param name="InstructionObject">
        /// Server Instruction Object
        /// </param>
        /// <param name="Args">
        /// </param>
        public delegate void ProcessInstruction(BaseServerInstruction InstructionObject, ParseServerArgs Args);

        /// <summary>
        /// Process Display Text and Channels
        /// </summary>
        public event DataHandler ProcessServerChannelData;

        /// <summary>
        /// Process the Data coming from the Game server
        /// </summary>
        public event DataHandler ProcessServerData;

        /// <summary>
        /// </summary>
        public event ProcessInstruction ProcessServerInstruction;

        #endregion "Client/Server Fata Handling"

        #endregion "Public Events"

        #region Public Properties

        #region Server Queue Manager

        /// <summary>
        /// ServerQueue Throat Tired Mode
        /// </summary>
        /// <returns>
        /// State <see cref="Furcadia.Net.Utils.ServerQue.ThroatTired"/>
        /// </returns>
        public bool ThroatTired
        {
            get { return ServerBalancer.ThroatTired; }
            set { ServerBalancer.ThroatTired = value; }
        }

        /// <summary>
        /// Server Queue NoEndurance modew
        /// </summary>
        internal bool NoEndurance
        {
            get { return ServerBalancer.NoEndurance; }
            set { ServerBalancer.NoEndurance = value; }
        }

        #endregion Server Queue Manager

        private ConnectionPhase serverconnectphase;

        /// <summary>
        /// Beekin Badge
        /// </summary>
        public Queue<string> BadgeTag { get; set; }

        /// <summary>
        /// Current Name for Banish Operations
        /// <para>
        /// We mirror Furcadia's Banish system for effciency
        /// </para>
        /// </summary>
        public string BanishName { get => banishName; set => banishName = value; }

        /// <summary>
        /// </summary>
        public List<string> BanishString { get => banishString; set => banishString = value; }

        /// <summary>
        /// </summary>
        public string Channel { get => channel; set => channel = value; }

        /// <summary>
        /// </summary>
        public bool ClientClose { get => clientClose; set => clientClose = value; }

        /// <summary>
        /// Current Connection Phase
        /// </summary>
        public ConnectionPhase ClientConnectPhase
        {
            get { return clientconnectionphase; }
        }

        /// <summary>
        /// Client Connection status
        /// </summary>
        /// <returns>
        /// Status tog the Furcadia Client
        /// </returns>
        public ConnectionPhase ClientStatus
        {
            get { return clientconnectionphase; }
        }

        /// <summary>
        /// </summary>
        public int ConnectedCharacterFurcadiaID
        { get => botUID; set => botUID = value; }

        /// <summary>
        /// Our Connected Character name
        /// </summary>
        public string ConnectedCharacterName { get => botName; set => botName = value; }

        /// <summary>
        /// Current Dream Information with Furre List
        /// </summary>
        public DREAM Dream
        {
            get { return dream; }
        }

        /// <summary>
        /// </summary>
        public string ErrorMsg { get => errorMsg; set => errorMsg = value; }

        /// <summary>
        /// </summary>
        public short ErrorNum { get => errorNum; set => errorNum = value; }

        /// <summary>
        /// We have Dream Share or We are Dream owner
        /// </summary>
        public bool HasShare { get { return hasShare; } }

        /// <summary>
        /// </summary>
        public bool InDream { get => inDream; set => inDream = value; }

        /// <summary>
        /// Current Triggering player
        /// </summary>
        public FURRE Player
        {
            get { return player; }
        }

        #region Dice Rolles

        public string DiceCompnentMatch { get => diceCompnentMatch; set => diceCompnentMatch = value; }
        public double DiceCount { get => diceCount; set => diceCount = value; }
        public double DiceModifyer { get => diceModifyer; set => diceModifyer = value; }
        public double DiceResult { get => diceResult; set => diceResult = value; }
        public double DiceSides { get => diceSides; set => diceSides = value; }

        #endregion Dice Rolles

        public bool ProcExit { get => procExit; set => procExit = value; }

        /// <summary>
        /// Curent server connection phase
        /// </summary>
        public ConnectionPhase ServerConnectPhase
        {
            get { return serverconnectphase; }
        }

        /// <summary>
        /// Server Connection status
        /// </summary>
        /// <returns>
        /// Status of the Furcadia Game server
        /// </returns>
        public ConnectionPhase ServerStatus
        {
            get { return serverconnectphase; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// implementation of Dispose pattern callable by consumers.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

        #endregion Public Methods

        #region "Public Methods"

        /// <summary>
        /// Are we the Current triggering furre?
        /// </summary>
        /// <param name="player">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsBot(ref FURRE player)
        {
            return player.ShortName == FurcadiaShortName(botName);
        }

        /// <summary>
        /// </summary>
        /// <param name="sname">
        /// </param>
        /// <returns>
        /// </returns>
        public FURRE NameToFurre(string sname)
        {
            foreach (FURRE Character in Dream.FurreList)
            {
                if (Character.ShortName == Furcadia.Util.FurcadiaShortName(sname))
                {
                    return Character;
                }
            }
            return null;
        }

        #endregion "Public Methods"

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
        /// Update 23 Avatar Moement http://dev.furcadia.com/docs/023_new_movement.pdf
        /// </para>
        /// <para>
        /// Update 27 Movement http://dev.furcadia.com/docs/027_movement.html
        /// </para>
        /// <para>
        /// FTR http://ftr.icerealm.org/ref-instructions/
        /// </para>
        /// </remarks>
        public void ParseServerChannel(ref string data, bool Handled)
        {
            //TODO: needs to move and refactored to ServerParser Class
            lock (ChannelLock)
            {
                data = data.Remove(0, 1);

                string SpecTag = "";
                Channel = Regex.Match(data, ChannelNameFilter).Groups[1].Value;
                string Color = Regex.Match(data, EntryFilter).Groups[1].Value;
                string User = "";
                string Desc = "";
                string Text = "";
                if (!Handled)
                {
                    Text = Regex.Match(data, EntryFilter).Groups[2].Value;
                    User = Regex.Match(data, NameFilter).Groups[3].Value;
                    if (!string.IsNullOrEmpty(User))
                        player = NameToFurre(User);
                    player.Message = "";
                    Desc = Regex.Match(data, DescFilter).Groups[2].Value;
                    Regex mm = new Regex(Iconfilter);
                    Match ds = mm.Match(Text);
                    Text = mm.Replace(Text, "[" + ds.Groups[1].Value + "] ");
                    Regex s = new Regex(ChannelNameFilter);
                    Text = s.Replace(Text, "");
                }
                else
                {
                    User = player.Name;
                    Text = player.Message;
                }
                DiceSides = 0;
                DiceCount = 0;
                DiceCompnentMatch = "";
                DiceModifyer = 0;
                DiceResult = 0;

                ErrorMsg = "";
                ErrorNum = 0;

                if (Channel == "@news" | Channel == "@spice")
                {
                    ProcessServerData?.Invoke(data, new NetServerEventArgs());

                    SendToClient("(" + data);
                    return;
                }
                else if (Color == "success")
                {
                    if (Text.Contains(" has been banished from your dreams."))
                    {
                        //banish <name> (online)
                        //Success: (.*?) has been banished from your dreams.

                        Regex t = new Regex("(.*?) has been banished from your dreams.");
                        BanishName = t.Match(Text).Groups[1].Value;

                        BanishString.Add(BanishName);
                    }
                    else if (Text == "You have canceled all banishments from your dreams.")
                    {
                        //banish-off-all (active list)
                        //Success: You have canceled all banishments from your dreams.
                        BanishString.Clear();
                    }
                    else if (Text.EndsWith(" has been temporarily banished from your dreams."))
                    {
                        //tempbanish <name> (online)
                        //Success: (.*?) has been temporarily banished from your dreams.

                        Regex t = new Regex("(.*?) has been temporarily banished from your dreams.");
                        BanishName = t.Match(Text).Groups[1].Value;

                        // MainMSEngine.PageExecute(61)
                        BanishString.Add(BanishName);
                    }
                    else if (Text == "Control of this dream is now being shared with you.")
                    {
                        hasShare = true;
                    }
                    else if (Text.EndsWith("is now sharing control of this dream with you."))
                    {
                        hasShare = true;
                    }
                    else if (Text.EndsWith("has stopped sharing control of this dream with you."))
                    {
                        hasShare = false;
                    }
                    else if (Text.StartsWith("The endurance limits of player "))
                    {
                        Regex t = new Regex("The endurance limits of player (.*?) are now toggled off.");
                        string m = t.Match(Text).Groups[1].Value;
                        if (FurcadiaShortName(m) == FurcadiaShortName(botName))
                        {
                            NoEndurance = true;
                        }
                    }
                    else if (Channel == "@cookie")
                    {
                        //(0:96) When the Bot sees "Your cookies are ready."
                        Regex CookiesReady = new Regex(string.Format("{0}", "Your cookies are ready.  http://furcadia.com/cookies/ for more info!"));
                    }
                    ProcessServerData?.Invoke(data, new NetServerEventArgs());
                    SendToClient("(" + data);
                    return;
                }
                else if (Channel == "@roll")
                {
                    Regex DiceREGEX = new Regex(DiceFilter, RegexOptions.IgnoreCase);
                    System.Text.RegularExpressions.Match DiceMatch = DiceREGEX.Match(data);

                    //Matches, in order:
                    //1:      shortname()
                    //2:      full(name)
                    //3:      dice(count)
                    //4:      sides()
                    //5: +/-#
                    //6: +/-  (component match)
                    //7:      additional(Message)
                    //8:      Final(result)

                    player = NameToFurre(DiceMatch.Groups[3].Value);
                    player.Message = DiceMatch.Groups[7].Value;
                    double.TryParse(DiceMatch.Groups[4].Value, out diceSides);
                    double.TryParse(DiceMatch.Groups[3].Value, out diceCount);
                    DiceCompnentMatch = DiceMatch.Groups[6].Value;
                    DiceModifyer = 0.0;
                    double.TryParse(DiceMatch.Groups[5].Value, out diceModifyer);
                    double.TryParse(DiceMatch.Groups[8].Value, out diceResult);

                    SendToClient("(" + data);
                    return;
                }
                else if (Channel == "@dragonspeak" || Channel == "@emit" || Color == "emit")
                {
                    SendToClient("(" + data);
                    return;
                    //'BCast (Advertisments, Announcments)
                }
                else if (Color == "bcast")
                {
                    string AdRegEx = "<channel name='(.*)' />";

                    string chan = Regex.Match(data, AdRegEx).Groups[1].Value;
                    string u;
                    switch (chan)
                    {
                        case "@advertisements":

                            AdRegEx = "\\[(.*?)\\] (.*?)</font>";
                            string adMessage = Regex.Match(data, AdRegEx).Groups[2].Value;
                            ProcessServerData?.Invoke(data, new NetServerEventArgs());

                            break;

                        case "@bcast":

                            u = Regex.Match(data, "<channel name='@(.*?)' />(.*?)</font>").Groups[2].Value;
                            ProcessServerData?.Invoke(data, new NetServerEventArgs());

                            break;

                        case "@announcements":

                            u = Regex.Match(data, "<channel name='@(.*?)' />(.*?)</font>").Groups[2].Value;
                            ProcessServerData?.Invoke(data, new NetServerEventArgs());

                            break;

                        default:
#if DEBUG
                            Console.WriteLine("Unknown ");
                            Console.WriteLine("BCAST:" + data);
#endif
                            break;
                    }

                    SendToClient("(" + data);
                    return;
                    //'SAY
                }
                else if (Color == "myspeech")
                {
                    Regex t = new Regex(YouSayFilter);
                    string u = t.Match(data).Groups[1].Value;
                    Text = t.Match(data).Groups[2].Value;
                    if (SpeciesTag.Count > 0)
                    {
                        //player.Color = SpeciesTag.Dequeue();
                        if (Dream.FurreList.Contains(player))
                            Dream.FurreList[player.ID] = player;
                    }

                    player.Message = Text;

                    SendToClient("(" + data);
                    return;
                }
                else if (!string.IsNullOrEmpty(User) & string.IsNullOrEmpty(Channel) & string.IsNullOrEmpty(Color) & Regex.Match(data, NameFilter).Groups[2].Value != "forced")
                {
                    Match tt = Regex.Match(data, "\\(you see(.*?)\\)", RegexOptions.IgnoreCase);
                    Regex t = new Regex(NameFilter);

                    if (!tt.Success)
                    {
                        Text = t.Replace(data, "");
                        Text = Text.Remove(0, 2);

                        if (SpeciesTag.Count > 0)
                        {
                            //player.Color = SpeciesTag.Dequeue();
                            if (Dream.FurreList.Contains(player))
                                Dream.FurreList[player.ID] = player;
                        }
                        Channel = "say";

                        SendToClient("(" + data);
                        return;
                    }
                    else
                    {
                        //sndDisplay("You See '" & User & "'")
                        Look = true;
                    }
                }
                else if (!string.IsNullOrEmpty(Desc))
                {
                    string DescName = Regex.Match(data, DescFilter).Groups[1].Value;

                    player = NameToFurre(DescName);
                    if (LookQue.Count > 0)
                    {
                        player.Color = new ColorString(LookQue.Dequeue());
                        string colorcode = LookQue.Peek();
                    }
                    if (BadgeTag.Count > 0)
                    {
                        player.Badge = BadgeTag.Dequeue(); ;
                    }
                    else if (!string.IsNullOrEmpty(player.Badge))
                    {
                        player.Badge = "";
                    }
                    player.Desc = Desc.Substring(6);
                    if (Dream.FurreList.Contains(player))
                        Dream.FurreList[player.ID] = player;

                    //sndDisplay)
                    ProcessServerData?.Invoke(data, new NetServerEventArgs());

                    Look = false;

                    SendToClient("(" + data);
                    return;
                }
                else if (Color == "shout")
                {
                    //'SHOUT
                    Regex t = new Regex(YouSayFilter);
                    string u = t.Match(data).Groups[1].Value;
                    Text = t.Match(data).Groups[2].Value;

                    if (!IsBot(ref player))
                    {
                        player.Message = Text;
                    }

                    SendToClient("(" + data);
                    return;
                }
                else if (Color == "query")
                {
                    string QCMD = Regex.Match(data, "<a.*?href='command://(.*?)'>").Groups[1].Value;
                    //player = NameToFurre(User, True)
                    switch (QCMD)
                    {
                        case "summon":
                            //'JOIN

                            ProcessServerData?.Invoke(data, new NetServerEventArgs());

                            break;

                        case "join":
                            //'SUMMON

                            ProcessServerData?.Invoke(data, new NetServerEventArgs());

                            break;

                        case "follow":
                            //'LEAD

                            ProcessServerData?.Invoke(data, new NetServerEventArgs());

                            break;

                        case "lead":
                            //'FOLLOW

                            ProcessServerData?.Invoke(data, new NetServerEventArgs());
                            //If Not IsBot(player) Then

                            break;

                        case "cuddle":

                            ProcessServerData?.Invoke(data, new NetServerEventArgs());

                            break;

                        default:
                            ProcessServerData?.Invoke(data, new NetServerEventArgs());

                            break;
                    }

                    //NameFilter

                    SendToClient("(" + data);
                    return;
                }
                else if (Color == "whisper")
                {
                    //'WHISPER
                    string WhisperFrom = Regex.Match(data, "whispers, \"(.*?)\" to you").Groups[1].Value;
                    string WhisperTo = Regex.Match(data, "You whisper \"(.*?)\" to").Groups[1].Value;
                    string WhisperDir = Regex.Match(data, string.Format("<name shortname='(.*?)' src='whisper-(.*?)'>")).Groups[2].Value;
                    if (WhisperDir == "from")
                    {
                        //player = NameToFurre(User, True)
                        player.Message = WhisperFrom;
                        if (BadgeTag.Count > 0)
                        {
                            player.Badge = BadgeTag.Dequeue();
                        }
                        else
                        {
                            player.Badge = "";
                        }

                        if (Dream.FurreList.Contains(player))
                            Dream.FurreList[player.ID] = player;
                    }

                    SendToClient("(" + data);
                    return;
                }
                else if (Color == "warning")
                {
                    ErrorMsg = Text;
                    ErrorNum = 1;
                    SendToClient("(" + data);
                    return;
                }
                else if (Color == "trade")
                {
                    string TextStr = Regex.Match(data, "\\s<name (.*?)</name>").Groups[0].Value;
                    Text = Text.Substring(6);
                    if (!string.IsNullOrEmpty(User))
                        Text = " " + User + Text.Replace(TextStr, "");
                    player.Message = Text;

                    SendToClient("(" + data);
                    return;
                }
                else if (Color == "emote")
                {
                    // ''EMOTE
                    if (SpeciesTag.Count > 0)
                    {
                        player.Color = new ColorString(SpeciesTag.Dequeue());
                    }
                    Regex usr = new Regex(NameFilter);
                    System.Text.RegularExpressions.Match n = usr.Match(Text);
                    Text = usr.Replace(Text, "");

                    player = NameToFurre(n.Groups[3].Value);

                    player.Message = Text;
                    if (Dream.FurreList.Contains(player))
                        Dream.FurreList[player.ID] = player;
                    bool test = IsBot(ref player);

                    SendToClient("(" + data);
                    return;
                }
                else if (Color == "channel")
                {
                    //ChannelNameFilter2
                    Regex chan = new Regex(ChannelNameFilter);
                    System.Text.RegularExpressions.Match ChanMatch = chan.Match(data);
                    Regex r = new Regex("<img src='(.*?)' alt='(.*?)' />");
                    System.Text.RegularExpressions.Match ss = r.Match(Text);
                    if (ss.Success)
                        Text = Text.Replace(ss.Groups[0].Value, "");
                    r = new Regex(NameFilter + ":");
                    ss = r.Match(Text);
                    if (ss.Success)
                        Text = Text.Replace(ss.Groups[0].Value, "");
                    ProcessServerData?.Invoke(data, new NetServerEventArgs());
                    SendToClient("(" + data);
                }
                else if (Color == "notify")
                {
                    string NameStr = "";
                    if (Text.StartsWith("players banished from your dreams: "))
                    {
                        //Banish-List
                        //[notify> players banished from your dreams:
                        //`(0:54) When the bot sees the banish list
                        BanishString.Clear();
                        string[] tmp = Text.Substring(35).Split(',');
                        foreach (string t in tmp)
                        {
                            BanishString.Add(t);
                        }
                    }
                    else if (Text.StartsWith("The banishment of player "))
                    {
                        //banish-off <name> (on list)
                        //[notify> The banishment of player (.*?) has ended.

                        Regex t = new Regex("The banishment of player (.*?) has ended.");
                        NameStr = t.Match(data).Groups[1].Value;

                        for (int I = 0; I <= BanishString.Count - 1; I++)
                        {
                            if (FurcadiaShortName(BanishString[I]) == FurcadiaShortName(NameStr))
                            {
                                BanishString.RemoveAt(I);
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }
                    }

                    SendToClient("(" + data);
                    return;
                }
                else if (Color == "error")
                {
                    ErrorMsg = Text;
                    ErrorNum = 2;

                    string NameStr = "";
                    if (Text.Contains("There are no furres around right now with a name starting with "))
                    {
                        //Banish <name> (Not online)
                        //Error:>>  There are no furres around right now with a name starting with (.*?) .

                        Regex t = new Regex("There are no furres around right now with a name starting with (.*?) .");
                        NameStr = t.Match(data).Groups[1].Value;
                    }
                    else if (Text == "Sorry, this player has not been banished from your dreams.")
                    {
                        //banish-off <name> (not on list)
                        //Error:>> Sorry, this player has not been banished from your dreams.

                        NameStr = BanishName;
                    }
                    else if (Text == "You have not banished anyone.")
                    {
                        //banish-off-all (empty List)
                        //Error:>> You have not banished anyone.

                        BanishString.Clear();
                    }
                    else if (Text == "You do not have any cookies to give away right now!")
                    {
                    }

                    ProcessServerData?.Invoke(data, new NetServerEventArgs());
                    SendToClient("(" + data);
                    return;
                }
                else if (data.StartsWith("Communication"))
                {
                    ProcessServerData?.Invoke(data, new NetServerEventArgs());
                    ProcExit = false;
                    Disconnect();
                    //LogSaveTmr.Enabled = False
                }
                else if (Channel == "@cookie")
                {
                    // <font color='emit'><img src='fsh://system.fsh:90' alt='@cookie' /><channel name='@cookie' /> Cookie <a href='http://www.furcadia.com/cookies/Cookie%20Economy.html'>bank</a> has currently collected: 0</font>
                    // <font color='emit'><img src='fsh://system.fsh:90' alt='@cookie' /><channel name='@cookie' /> All-time Cookie total: 0</font>
                    // <font color='success'><img src='fsh://system.fsh:90' alt='@cookie' /><channel name='@cookie' /> Your cookies are ready.  http://furcadia.com/cookies/ for more info!</font>
                    //<img src='fsh://system.fsh:90' alt='@cookie' /><channel name='@cookie' /> You eat a cookie.

                    Regex CookieToMe = new Regex(string.Format("{0}", CookieToMeREGEX));
                    if (CookieToMe.Match(data).Success)
                    {
                    }
                    Regex CookieToAnyone = new Regex(string.Format("<name shortname='(.*?)'>(.*?)</name> just gave <name shortname='(.*?)'>(.*?)</name> a (.*?)"));
                    if (CookieToAnyone.Match(data).Success)
                    {
                    }
                    Regex CookieFail = new Regex(string.Format("You do not have any (.*?) left!"));
                    if (CookieFail.Match(data).Success)
                    {
                    }
                    Regex EatCookie = new Regex(Regex.Escape("<img src='fsh://system.fsh:90' alt='@cookie' /><channel name='@cookie' /> You eat a cookie.") + "(.*?)");
                    if (EatCookie.Match(data).Success)
                    {
                        player.Message = "You eat a cookie." + EatCookie.Replace(data, "");
                    }
                    //Dim args As New ServerReceiveEventArgs
                    //args.Channel = Channel
                    //args.Text = data
                    //args.Handled = True
                    //RaiseEvent ServerChannelProcessed(data, args)
                    SendToClient("(" + data);
                    return;
                }
                else if (data.StartsWith("PS"))
                {
                    Color = "PhoenixSpeak";
                    ProcessServerChannelData?.Invoke(data, new NetServerEventArgs());
                    SendToClient("(" + data);
                    return;
                }
                else if (data.StartsWith("(You enter the dream of"))
                {
                    ProcessServerData?.Invoke(data, new NetServerEventArgs());
                    SendToClient("(" + data);
                    return;
                }
                else
                {
                    ProcessServerData?.Invoke(data, new NetServerEventArgs());

                    SendToClient("(" + data);
                    return;
                }
            }
        }

        /// <summary>
        /// Parse Server Data
        /// <para>
        /// TODO: Move this functionality to <see cref="Furcadia.Net.Utils.ParseServer"/>
        /// </para>
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
        /// Update 23 Avatar Moement http://dev.furcadia.com/docs/023_new_movement.pdf
        /// </para>
        /// <para>
        /// Update 27 Movement http://dev.furcadia.com/docs/027_movement.html
        /// </para>
        /// <para>
        /// FTR http://ftr.icerealm.org/ref-instructions/
        /// </para>
        /// </remarks>
        public virtual void ParseServerData(string data, bool Handled)
        {
            ServerInstructionType MyServerInstruction = ServerInstructionType.Unknown;
            // page = engine.LoadFromString(cBot.MS_Script)
            if (data == "Dragonroar")
            {
                // Login Sucessful
                SendToClient(data);
                return;

                //Logs into Furcadia
            }
            else if (data == "&&&&&&&&&&&&&")
            {
                //We've connected to Furcadia
                //Stop the reconnection manager
                serverconnectphase = ConnectionPhase.Connected;
                ServerStatusChanged?.Invoke(data, new NetServerEventArgs(serverconnectphase, MyServerInstruction));

                SendToClient(data);
                return;
            }
            // Species Tags
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

                SendToClient(data);
                return;
                //DS Variables
            }

            //Popup Dialogs!
            else if (data.StartsWith("]#"))
            {
                //]#<idstring> <style 0-17> <message that might have spaces in>
                Regex repqq = new Regex("^\\]#(.*?) (\\d+) (.*?)$");
                Match m = repqq.Match(data);
                Rep r = default(Rep);
                r.ID = m.Groups[1].Value;
                int num = 0;
                int.TryParse(m.Groups[2].Value, out num);
                r.Type = num;
                Repq.Enqueue(r);
                player.Message = m.Groups[3].Value;

                SendToClient(data);
                return;
                //]s(.+)1 (.*?) (.*?) 0
            }
            // SSL/TLS `starttls
            else if (data.StartsWith("]s"))
            {
                Regex t = new Regex("\\]s(.+)1 (.*?) (.*?) 0", RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Match m = t.Match(data);

                SendToClient(data);
                return;
            }
            //Look response
            else if (data.StartsWith("]f") & serverconnectphase == ConnectionPhase.Connected & InDream == true)
            {
                short length = 14;
                if (Look)
                {
                    LookQue.Enqueue(data.Substring(2));
                }
                else
                {
                    if (data.Substring(2, 1) != "t")
                    {
                        length = 30;
                    }
                    else
                    {
                        length = 14;
                    }

                    player = NameToFurre(data.Remove(0, length + 2));
                    // If player.ID = 0 Then Exit Sub
                    player.Color = new ColorString(data.Substring(2, length));
                    if (IsBot(ref player))
                        Look = false;
                    if (Dream.FurreList.Contains(player))
                        Dream.FurreList[Dream.FurreList.IndexOf(player)] = player;
                }
                SendToClient(data);
                return;
            }
            //Spawn Avatar
            else if (data.StartsWith("<") & serverconnectphase == ConnectionPhase.Connected)
            {
                var FurreSpawn = new SpawnFurre(data);
                player = FurreSpawn.player;

                if (FurreSpawn.PlayerFlags.HasFlag(CHAR_FLAG_NEW_AVATAR) & !Dream.FurreList.Contains(player))
                {
                    Dream.FurreList.Add(player);
                }
                else
                    Dream.FurreList[Dream.FurreList.IndexOf(player)] = player;

                if (FurreSpawn.PlayerFlags.HasFlag(CHAR_FLAG_SET_VISIBLE))
                {
                    FURRE Bot = NameToFurre(botName);
                    ViewArea VisableRectangle = getTargetRectFromCenterCoord(Bot.Position.x, Bot.Position.y);
                    if (VisableRectangle.X <= player.Position.x & VisableRectangle.Y <=
                        player.Position.y & VisableRectangle.height >=
                        player.Position.y & VisableRectangle.length >= player.Position.x)
                    {
                        player.Visible = true;
                    }
                    else
                    {
                        player.Visible = false;
                    }
                }
                //if (FurreSpawn.PlayerFlags.HasFlag(CHAR_FLAG_HAS_PROFILE))
                //{
                //}
                ProcessServerInstruction?.Invoke(FurreSpawn,
                    new ParseServerArgs(ServerInstructionType.SpawnAvatar, serverconnectphase));
            }
            //Remove Furre
            else if (data.StartsWith(")") & serverconnectphase == ConnectionPhase.Connected)
            {
                int remID = ConvertFromBase220(data.Substring(1, 4));
                // remove departure from List
                if (Dream.FurreList.Contains(remID) == true)
                {
                    player = Dream.FurreList[remID];
                    Dream.FurreList.Remove(remID);
                }

                SendToClient(data);
                return;
            }
            //Animated Move
            else if (data.StartsWith("/") & serverconnectphase == ConnectionPhase.Connected)
            {
                player = Dream.FurreList[ConvertFromBase220(data.Substring(1, 4))];
                player.Position.x = ConvertFromBase220(data.Substring(5, 2)) * 2;
                player.Position.y = ConvertFromBase220(data.Substring(7, 2));
                player.Shape = ConvertFromBase220(data.Substring(9, 2));
                FURRE Bot = Dream.FurreList[ConnectedCharacterFurcadiaID];
                ViewArea VisableRectangle = getTargetRectFromCenterCoord(Bot.Position.x, Bot.Position.y);
                if (VisableRectangle.X <= player.Position.x & VisableRectangle.Y <= player.Position.y &
                    VisableRectangle.height >= player.Position.y & VisableRectangle.length >=
                    player.Position.x)
                {
                    player.Visible = true;
                }
                else
                {
                    player.Visible = false;
                }
                if (Dream.FurreList.Contains(player))
                    Dream.FurreList[player.ID] = player;

                SendToClient(data);
                return;
            }
            // Move Avatar
            else if (data.StartsWith("A") & serverconnectphase == ConnectionPhase.Connected)
            {
                player = Dream.FurreList[ConvertFromBase220(data.Substring(1, 4))];
                player.Position.x = ConvertFromBase220(data.Substring(5, 2)) * 2;
                player.Position.y = ConvertFromBase220(data.Substring(7, 2));
                player.Shape = ConvertFromBase220(data.Substring(9, 2));

                FURRE Bot = Dream.FurreList[ConnectedCharacterFurcadiaID];
                ViewArea VisableRectangle = getTargetRectFromCenterCoord(Bot.Position.x, Bot.Position.y);

                if (VisableRectangle.X <= player.Position.x & VisableRectangle.Y <=
                    player.Position.y & VisableRectangle.height >= player.Position.y &
                    VisableRectangle.length >= player.Position.x)
                {
                    player.Visible = true;
                }
                else
                {
                    player.Visible = false;
                }

                SendToClient(data);
                return;
            }
            //Update ColorString
            else if (data.StartsWith("B") & serverconnectphase == ConnectionPhase.Connected & InDream)
            {
                //fuid size 4 b220 bytes
                player = Dream.FurreList.GetFurreByID(data.Substring(1, 4));
                UpdateColorString ColorStringUpdate = new UpdateColorString(ref player, data);

                ProcessServerInstruction?.Invoke(ColorStringUpdate,
                        new ParseServerArgs(ServerInstructionType.UpdateColorString, serverconnectphase));
            }
            //Hide Avatar
            else if (data.StartsWith("C") != false & serverconnectphase == ConnectionPhase.Connected)
            {
                player = Dream.FurreList[ConvertFromBase220(data.Substring(1, 4))];
                player.Position.x = ConvertFromBase220(data.Substring(5, 2)) * 2;
                player.Position.y = ConvertFromBase220(data.Substring(7, 2));
                player.Visible = false;
                if (Dream.FurreList.Contains(player))
                {
                    Dream.FurreList[player.ID] = player;
                }
                IsBot(ref player);

                SendToClient(data);
                return;
                //Display Disconnection Dialog
            }
            else if (data.StartsWith("["))
            {
#if DEBUG
                Console.WriteLine("Disconnection Dialog:" + data);
#endif
                InDream = false;
                Dream.FurreList.Clear();
                // RaiseEvent UpDateDreamList("")

                SendToClient(data);

                return;

                //;{mapfile}	Load a local map (one in the furcadia folder)
                //]q {name} {id}	Request to download a specific patch
            }
            else if (data.StartsWith(";") || data.StartsWith("]q") || data.StartsWith("]r"))
            {
#if DEBUG

                Console.WriteLine("Entering new Dream" + data);
#endif

                hasShare = false;
                NoEndurance = false;

                Dream.FurreList.Clear();
                //RaiseEvent UpDateDreamList("")
                InDream = false;

                SendToClient(data);
                return;
            }
            else if (data.StartsWith("]z"))
            {
                ConnectedCharacterFurcadiaID = int.Parse(data.Remove(0, 2));
                //Snag out UID
            }
            else if (data.StartsWith("]B"))
            {
                ConnectedCharacterFurcadiaID = int.Parse(data.Substring(2, data.Length - botName.Length - 3));
                SendToClient(data);
                return;
            }
            else if (data.StartsWith("]c"))
            {
#if DEBUG
                Console.WriteLine(data);
#endif
                SendToClient(data);
                return;
            }
            else if (data.StartsWith("]C"))
            {
                if (data.StartsWith("]C0"))
                {
                    string dname = data.Substring(10);
                    if (dname.Contains(":"))
                    {
                        string NameStr = dname.Substring(0, dname.IndexOf(":"));
                        if (FurcadiaShortName(NameStr) == FurcadiaShortName(botName))
                        {
                            hasShare = true;
                        }
                    }
                    else if (dname.EndsWith("/") && !dname.Contains(":"))
                    {
                        string NameStr = dname.Substring(0, dname.IndexOf("/"));
                        if (FurcadiaShortName(NameStr) == FurcadiaShortName(botName))
                        {
                            hasShare = true;
                        }
                    }
                }
#if DEBUG
                Console.WriteLine(data);
#endif
                SendToClient(data);
                return;
                //Process Channels Seperatly
            }
            else if (data.StartsWith("("))
            {
                if (ThroatTired == false & data.StartsWith("(<font color='warning'>Your throat is tired. Try again in a few seconds.</font>"))
                {
                    //Using Furclib ServQue
                    ThroatTired = true;

                    //(0:92) When the bot detects the "Your throat is tired. Please wait a few seconds" message,
                    SendToClient(data);
                }

                return;
            }
            else
            {
                SendToClient(data);
                ServerStatusChanged?.Invoke(data, new NetServerEventArgs(serverconnectphase, MyServerInstruction));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="data">
        /// </param>
        public override void SendToClient(string data)
        {
            if (IsClientConnected)
                base.SendToClient(data);
        }

        /// <summary>
        /// Send Message to Server through the Load Ballancer
        /// </summary>
        /// <param name="message">
        /// Client to server Instruction
        /// </param>
        public override void SendToServer(string message)
        {
            ServerBalancer.SendToServer(message);
        }

        /// <summary>
        /// Format basic furcadia commands and send to server
        /// <para>
        /// We also mirror the client to server banish system.
        /// </para>
        /// <para>
        /// This maybe a good place to place Proxy/Bot commands for controls
        /// </para>
        /// </summary>
        /// <param name="data">
        /// Raw Client to Server instruction
        /// </param>
        public virtual void sndServer(ref string data)
        {
            if (!base.IsServerConnected)
                return;

            if (data.StartsWith("`m "))
            {
                switch (data.Substring(2, 1))
                {
                    case "7":

                        break;

                    case "9":

                        break;

                    case "1":

                        break;

                    case "3":

                        break;
                }
            }
            else if (data == "`use")
            {
            }
            else if (data == "`get")
            {
            }
            else if (data == "`>")
            {
            }
            else if (data == "`<")
            {
            }
            else if (data == "`lie")
            {
            }
            else if (data == "`liedown")
            {
            }
            else if (data == "`sit")
            {
            }
            else if (data == "`stand")
            {
            }
            else if (data.StartsWith("banish "))
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
        /// Text Channel Prefixes (shout,whisper emote, Raw Server command)
        /// </summary>
        /// <param name="arg">
        /// </param>
        public void TextToServer(ref string arg)
        {
            if (string.IsNullOrWhiteSpace(arg))
                return;
            //Clean Text input to match Client
            string result = "";
            switch (arg.Substring(0, 1))
            {
                case "`":
                    result = arg.Remove(0, 1);
                    break;

                case "/":
                    result = "wh " + arg.Substring(1);
                    break;

                case ":":
                    result = arg;

                    break;

                case "-":
                    result = arg;

                    break;

                default:
                    result = (char)34 + arg;
                    break;
            }
            SendToServer(result);
        }

        /// <summary>
        /// Client sent us some data, Let's deal with it
        /// </summary>
        /// <param name="data">
        /// </param>
        private void onClientDataReceived(string data)
        {
            //TODO Raise Client Data Received event

            try
            {
                if ((Monitor.TryEnter(clientlock)))
                {
                    if (data.StartsWith("connect"))
                    {
                        string test = data.Replace("connect ", "").TrimStart(' ');
                        botName = test.Substring(0, test.IndexOf(" "));
                        botName = botName.Replace("|", " ");

                        botName = botName.Replace("[^a-zA-Z0-9\\0x0020_.| ]+", "").ToLower();
                    }
                    else if (data == "vascodagama" & serverconnectphase == ConnectionPhase.Connected)
                    {
                        // serverconnectphase = ConnectionPhase.Conne
                    }
                    SendToServer(data);
                }
            }
            finally
            {
                Monitor.Exit(clientlock);
            }
        }

        private void onServerDataReceived(string data)
        {
            try
            {
                Monitor.Enter(DataReceived);
                player = new FURRE();
                Channel = "";
                ParseServerData(data, false);
            }
            finally
            {
                Monitor.Exit(DataReceived);
            }
        }

        #region "Dice Rolls"

        private string diceCompnentMatch;

        private double diceCount = 0.0;

        private double diceModifyer = 0.0;
        private double diceResult = 0.0;
        private double diceSides = 0.0;
        //TODO Check MS Engine Dice lines

        #endregion "Dice Rolls"

        #region "Popup Dialogs"

        /// <summary>
        /// </summary>
        public Queue<Rep> Repq = new Queue<Rep>();

        //TODO Check Furcadoia Popup Windows
        /// <summary>
        /// </summary>
        public struct Rep
        {
            #region "Public Fields"

            private string iD;

            private int type;

            /// <summary>
            /// </summary>
            public string ID { get => iD; set => iD = value; }

            /// <summary>
            /// </summary>
            public int Type { get => type; set => type = value; }

            #endregion "Public Fields"
        }

        #endregion "Popup Dialogs"

        #region "Constructors"

        /// <summary>
        /// </summary>
        public FurcadiaSession() : base()
        {
            ServerBalancer = new Utils.ServerQue();
            ServerBalancer.OnServerSendMessage += onServerQueSent;

            ServerData2 += onServerDataReceived;
            ClientData2 += onClientDataReceived;
            ReconnectionManager = new Furcadia.Net.Utils.ProxyReconnect();
        }

        /// <summary>
        /// </summary>
        /// <param name="ServerHost">
        /// </param>
        /// <param name="ServerPort">
        /// </param>
        /// <param name="LocalHostPort">
        /// </param>
        /// <param name="ReconnectAttempts">
        /// </param>
        /// <param name="ReconnectTimeOutDelay">
        /// </param>
        public FurcadiaSession(string ServerHost, int ServerPort, int LocalHostPort, int ReconnectAttempts, int ReconnectTimeOutDelay) : base(ServerHost, ServerPort, LocalHostPort)
        {
            ServerBalancer = new Utils.ServerQue();
            ServerBalancer.OnServerSendMessage += onServerQueSent;
            ServerData2 += onServerDataReceived;
            ClientData2 += onClientDataReceived;

            ReconnectionManager = new Furcadia.Net.Utils.ProxyReconnect(ReconnectAttempts, ReconnectTimeOutDelay);
        }

        /// <summary>
        /// </summary>
        /// <param name="options">
        /// Proxy Options
        /// </param>
        public FurcadiaSession(Options.ProxySessionOptions options) : base(options)
        {
            ServerBalancer = new Utils.ServerQue();
            ServerBalancer.OnServerSendMessage += onServerQueSent;
            ServerData2 += onServerDataReceived;
            ClientData2 += onClientDataReceived;

            ReconnectionManager = new Furcadia.Net.Utils.ProxyReconnect(options.ReconnectAttempts, options.ReconnectTimeOutDelay);
        }

        #endregion "Constructors"

        private void onServerQueSent(object o, EventArgs e)
        {
            base.SendToServer(o.ToString());
        }

        #region "Dispose"

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                base.Dispose();
            }

            // Free any unmanaged objects here.
            disposed = true;
        }

        #endregion "Dispose"
    }
}