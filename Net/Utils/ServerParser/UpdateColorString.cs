using Furcadia.Movement;
using Furcadia.Net.Dream;

using static Furcadia.Text.Base220;

namespace Furcadia.Net.Utils.ServerParser
{
    /// <summary>
    /// </summary>
    public class UpdateColorString : BaseServerInstruction
    {
        #region Public Fields

        /// <summary>
        /// the Active Player
        /// </summary>
        public FURRE player;

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        /// <param name="Player">
        /// </param>
        /// <param name="ServerInstruction">
        /// </param>
        public UpdateColorString(ref FURRE Player, string ServerInstruction) : base(ServerInstruction)
        {
            if (ServerInstruction.StartsWith("B"))
                base.instructionType = ServerInstructionType.UpdateColorString;

            player = Player;

            //avatar shape 2 b220
            player.Shape = ConvertFromBase220(ServerInstruction.Substring(5, 2));

            // partial color code
            player.Color.Update(ServerInstruction.Substring(8, ColorString.ColorStringSize));
        }

        #endregion Public Constructors
    }
}