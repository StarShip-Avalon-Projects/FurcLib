using System;
using System.Configuration;
using System.Net;
using System.Net.NetworkInformation;

namespace Furcadia.Net.Utils
{
    /// <summary>
    /// Generic Furcadia Network Utilities
    /// </summary>
    public class Utilities
    {
        #region Private Fields

        /// <summary>
        /// Set Encoders to win 1252 encoding
        /// </summary>
        private const int EncoderPage = 1252;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Checks TCP port and scans for an available TCP port on the host
        /// system <paraa>
        /// TODO: Find an Available Port?</paraa>
        /// </summary>
        /// <param name="port">
        /// ref TCP Port
        /// </param>
        /// <returns>
        /// True when a port is found available
        /// </returns>
        public static bool PortOpen(int port)
        {
            if (port == 0)
                throw new ArgumentException("port  cannot be 0");
            // Evaluate current system tcp connections. This is the same
            // information provided by the netstat command line application,
            // just in .Net strongly-typed object form. We will look through
            // the list, and if our port we would like to use in our
            // TcpClient is occupied, we will set isAvailable to false.
            IPGlobalProperties ipGlobalProperties__1 = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties__1.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    return false;
                }
            }
            return true;
            // At this point, if isAvailable is true, we can proceed accordingly.
        }

        #endregion Public Methods

        #region Public Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        [Legacy]
        public Utilities()
        {
            var appSettings = ConfigurationManager.AppSettings;

            gameserverhost = appSettings["GameServerHost"] ?? "lightbringer.furcadia.com";
            gameserverip = appSettings["GameServerIP"] ?? "72.52.134.168";
            defaultClient = appSettings["DefaultClient"] ?? "Furcadia.exe";
            pounceserverhost = appSettings["PounceServerHost"] ?? "on.furcadia.com/q";
            RegPathx86 = appSettings["RegistryPathX86"] ?? @"SOFTWARE\Dragon's Eye Productions\Furcadia\";
            RegPathx64 = appSettings["RegistryPathX64"] ?? @"SOFTWARE\Wow6432Node\Dragon's Eye Productions\Furcadia\";
            RegPathMono = appSettings["RegistryPathMono"] ?? @"Software/Dragon's Eye Productions/Furcadia/";
        }

        #endregion Public Constructors

        private readonly string defaultClient;

        /// <summary>
        /// Furcadia Client Executable
        /// </summary>
        public string DefaultClient
        {
            get { return defaultClient; }
        }

        #region Private Fields

        /// <summary>
        /// Game server DNS (Furcadia v31c)
        /// <para>
        /// update to library config file?
        /// </para>
        /// </summary>
        private readonly string gameserverhost;

        /// <summary>
        /// Game Server IP (Furcadia v31c)
        /// <para>
        /// update to library config file?
        /// </para>
        /// </summary>
        private readonly string gameserverip;

        /// <summary>
        /// Pounce Server Host (Furcadia v31c)
        /// <para>
        /// update to library config file?
        /// </para>
        /// </summary>
        private readonly string pounceserverhost;

        /// <summary>
        /// Registry path for Mono
        /// </summary>
        private readonly string RegPathMono;

        /// <summary>
        /// Registry path for Win x64 Systems
        /// </summary>
        private readonly string RegPathx64;

        /// <summary>
        /// Registry path for x86 systems ///
        /// </summary>
        private readonly string RegPathx86;

        /// <summary>
        /// Mono Registry Path
        /// </summary>
        public string ReggistryPathMono
        {
            get
            {
                { return RegPathMono; }
            }
        }

        /// <summary>
        /// Windows x64 Registry path
        /// </summary>
        public string ReggistryPathX64
        {
            get
            {
                { return RegPathx64; }
            }
        }

        /// <summary>
        /// Windows 32 Registry path
        /// </summary>
        public string ReggistryPathX86
        {
            get
            {
                { return RegPathx86; }
            }
        }

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Master Config set Encoders to Win 1252 encoding.
        /// </summary>
        public static int GetEncoding { get { return EncoderPage; } }

        /// <summary>
        /// Gets or sets the Furcadia server host (i.e
        /// lightbringer.furcadia.com). (Furcadia v31c)
        /// </summary>
        public string GameServerHost
        {
            get { return gameserverhost; }
        }

        /// <summary>
        /// Gets or sets the IP of the Furcadia server. (Furcadia v31c)
        /// <para>
        /// update to library config file?
        /// </para>
        /// </summary>
        public IPAddress GameServerIp
        {
            get { return IPAddress.Parse(gameserverip); }
        }

        /// <summary>
        /// Gets or sets the Furcadia Pounce Server host (IE
        /// on.furcadia.com). (Furcadia v31c)
        /// </summary>
        public string PounceServerHost
        {
            get { return pounceserverhost; }
        }

        #endregion Public Properties
    }
}