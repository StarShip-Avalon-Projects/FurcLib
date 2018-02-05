using System.Text;

namespace Furcadia.Net.Utils.ServerParser
{
    /// <summary>
    /// Triggered when the connection enters a new dream.
    /// <para>
    /// This instruction tells the client to download the specified dream
    /// data from the file server.
    /// </para>
    /// <para>
    /// Respond with client command when furcadia client is not available "vasecodegamma"
    /// </para>
    /// </summary>
    public class LoadDream : BaseServerInstruction
    {
        #region Private Fields

        private string crc;
        private string fileName;
        private string mode;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///
        /// </summary>
        public LoadDream() : base()
        {
            base.InstructionType = ServerInstructionType.LoadDreamEvent;
            fileName = null;
            crc = null;
            mode = "legacy";
        }

        /// <summary>
        /// Constructor with Dream Data definitions
        /// </summary>
        /// <param name="ServerInstruction">
        /// Raw server instruction from the game server
        /// </param>
        public LoadDream(string ServerInstruction) : base(ServerInstruction)
        {
            base.InstructionType = ServerInstructionType.LoadDreamEvent;
            string[] Options = ServerInstruction.Substring(3).Split(' ');
            if (Options.Length >= 2)
            {
                fileName = Options[0];
                crc = Options[1];
            }
            if (Options.Length == 5)
                mode = Options[4];
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// td (Temporary Dream) or permanent map name
        /// </summary>
        public string CacheFileName
        {
            get => fileName;
            set => fileName = value;
        }

        /// <summary>
        /// Current dream mode
        /// </summary>
        public bool IsModern
        {
            get => mode == "modern";
        }

        /// <summary>
        /// Is the current dream a permanent dream?
        /// </summary>
        public bool IsPermanent
        {
            get
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    return false;
                return fileName.Substring(0, 2) == "pm";
            }
        }

        #endregion Public Properties

        /// <summary>

        /// Returns a <see cref="System.String" /> that
        /// represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine($"CacheFileName: '{fileName}' IsPermanent: '{IsPermanent}' IsModern: '{IsModern}'");
            return sb.ToString();
        }
    }
}