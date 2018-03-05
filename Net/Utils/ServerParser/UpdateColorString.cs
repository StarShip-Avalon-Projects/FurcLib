using Furcadia.Movement;
using Furcadia.Net.DreamInfo;

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
        public IFurre player;

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        /// <param name="Player">
        /// </param>
        /// <param name="ServerInstruction">
        /// </param>
        public UpdateColorString(IFurre Player, string ServerInstruction) : base(ServerInstruction)
        {
            if (ServerInstruction.StartsWith("B"))
                instructionType = ServerInstructionType.UpdateColorString;

            player = Player;

            //avatar shape 2 b220
            //  player.Shape = ConvertFromBase220(ServerInstruction.Substring(5, 2));

            // partial color code
            ((Furre)player).FurreColors.Update(ServerInstruction.Substring(8, ColorString.ColorStringSize));
        }

        #endregion Public Constructors
    }
}