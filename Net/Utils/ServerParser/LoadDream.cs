﻿namespace Furcadia.Net.Utils.ServerParser
{
    internal class LoadDream : BaseServerInstruction
    {
        #region Private Fields

        private string crc;
        private string dreamName;
        private string mode;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Constructor with Dream Data definitions
        /// </summary>
        /// <param name="ServerInstruction">
        /// </param>
        public LoadDream(string ServerInstruction) : base(ServerInstruction)
        {
            string[] Options = ServerInstruction.Substring(3).Split(' ');
            dreamName = Options[0];
            crc = Options[1];
            mode = Options[4];
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// td or permanent map name
        /// </summary>
        public string DreamName
        {
            get
            {
                return dreamName;
            }
        }

        /// <summary>
        /// Current dream mode
        /// </summary>
        public bool IsModern
        {
            get
            {
                return mode == "modern";
            }
        }

        /// <summary>
        /// Is the current dream a permanent dream?
        /// </summary>
        public bool IsPermanent
        {
            get
            {
                return dreamName.Substring(0, 2) == "pm";
            }
        }

        #endregion Public Properties
    }
}