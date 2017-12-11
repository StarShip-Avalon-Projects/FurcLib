using Furcadia.Net.DreamInfo;
using static Furcadia.Text.Base220;

namespace Furcadia.Net.Utils.ServerParser
{
    /// <summary>
    /// Remove Avatar Server Instruction Object
    /// </summary>
    public class RemoveAvatar : BaseServerInstruction
    {
        #region Public Fields

        /// <summary>
        /// Avatar ID
        /// <para>
        /// 4 byte Base220 string
        /// </para>
        /// </summary>
        public int FurreId;

        public Furre Player;

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        /// Remove Avatar from the Dream Furre List by its Furre ID
        /// </summary>
        /// <param name="ServerInstruction">
        /// </param>
        public RemoveAvatar(string ServerInstruction) : base(ServerInstruction)
        {
            if (ServerInstruction[0] == ')')
                base.instructionType = ServerInstructionType.RemoveAvatar;
            if (ServerInstruction.Length > 4)
                FurreId = ConvertFromBase220(ServerInstruction.Substring(1, 4));
            Player = new Furre(FurreId);
        }

        #endregion Public Constructors
    }
}