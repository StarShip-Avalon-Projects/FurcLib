﻿using System;
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
        private const int EncoderPage = 28591;

        private readonly string defaultClient;

        /// <summary>
        /// Game server DNS (Furcadia v31c)
        /// <para>
        /// update to library config file?
        /// </para>
        /// </summary>
        private readonly string gameserverhost;

        private readonly string gameserverport;

        /// <summary>
        /// Game Server IP (Furcadia v31c)
        /// <para>
        /// update to library configuration file?
        /// </para>
        /// </summary>
        private readonly string gameserverip;

        /// <summary>
        /// Pounce Server Host (Furcadia v31c)
        /// <para>
        /// update to library configuration file?
        /// </para>
        /// </summary>
        [Legacy] private readonly string pounceserverhost;

        /// <summary>
        /// Registry path for Mono
        /// </summary>
        private readonly string RegPathMono;

        /// <summary>
        /// Registry path for Win x64 Systems
        /// </summary>
        private readonly string RegPathx64;

        /// <summary>
        /// Registry path for x86 systems
        /// </summary>
        private readonly string RegPathx86;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Furcadia Defaults with optional app.config
        /// </summary>
        public Utilities()
        {
            var appSettings = ConfigurationManager.AppSettings;
            gameserverport = appSettings["GameServerPort"] ?? "6500";
            gameserverhost = appSettings["GameServerHost"] ?? "lightbringer.furcadia.com";
            gameserverip = appSettings["GameServerIP"] ?? "72.52.134.168";
            defaultClient = appSettings["DefaultClient"] ?? "Furcadia.exe";
            pounceserverhost = appSettings["PounceServerHost"] ?? "on.furcadia.com/q";
            RegPathx86 = appSettings["RegistryPathX86"] ?? @"SOFTWARE\Dragon's Eye Productions\Furcadia\";
            RegPathx64 = appSettings["RegistryPathX64"] ?? @"SOFTWARE\Wow6432Node\Dragon's Eye Productions\Furcadia\";
            RegPathMono = appSettings["RegistryPathMono"] ?? @"Software/Dragon's Eye Productions/Furcadia/";
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Master configuration set Encoders to Win 1252 encoding.
        /// </summary>
        public static int GetEncoding { get => EncoderPage; }

        /// <summary>
        /// Furcadia Client Executable Name with extension
        /// </summary>
        public string DefaultClient { get => defaultClient; }

        /// <summary>
        /// Gets or sets the Furcadia server host (i.e
        /// lightbringer.furcadia.com). (Furcadia v31c)
        /// </summary>
        public string GameServerHost
        {
            get => gameserverhost;
        }

        /// <summary>
        /// Gets or sets the IP of the Furcadia server. (Furcadia v31c)
        /// <para>
        /// update to library config file?
        /// </para>
        /// </summary>
        public IPAddress GameServerIp
        {
            get => IPAddress.Parse(gameserverip);
        }

        /// <summary>
        /// Default Game Server Port
        /// </summary>
        /// <value>
        /// The game server port.
        /// </value>
        public string GameServerPort
        {
            get => gameserverport;
        }

        /// <summary>
        /// Gets or sets the Furcadia Pounce Server host (IE
        /// on.furcadia.com). (Furcadia v31c)
        /// </summary>
        public string PounceServerHost => pounceserverhost;

        /// <summary>
        /// Mono Registry Path
        /// </summary>
        public string ReggistryPathMono
        {
            get => RegPathMono;
        }

        /// <summary>
        /// Windows x64 Registry path
        /// </summary>
        public string ReggistryPathX64
        {
            get => RegPathx64;
        }

        /// <summary>
        /// Windows 32 Registry path
        /// </summary>
        public string ReggistryPathX86
        {
            get => RegPathx86;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Checks TCP port and scans for an available TCP port on the host
        /// system
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="port"/> is 0
        /// </exception>
        /// <param name="port">
        /// ref TCP Port
        /// </param>
        /// <returns>
        /// True when a port is found available
        /// </returns>
        public static bool PortOpen(int port)
        {
            if (port == 0)
                throw new ArgumentOutOfRangeException("port  cannot be 0");
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
    }
}