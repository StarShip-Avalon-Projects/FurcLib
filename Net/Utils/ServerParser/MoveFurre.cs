using Furcadia.Net.DreamInfo;

namespace Furcadia.Net.Utils.ServerParser
{
    /// <summary>
    /// Animated or noanimated move object
    /// </summary>
    public class MoveFurre : BaseServerInstruction
    {
        #region Public Fields

        /// <summary>
        /// The active player
        /// </summary>
        public IFurre Player;

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveFurre"/> class.
        /// </summary>
        /// <param name="ServerInstruction">raw server instruction</param>
        public MoveFurre(string ServerInstruction) : base(ServerInstruction)
        {
            if (ServerInstruction[0] == '/')
                instructionType = ServerInstructionType.AnimatedMoveAvatar;
            else if (ServerInstruction[0] == 'A')
                instructionType = ServerInstructionType.MoveAvatar;
            if (ServerInstruction.Length > 4)
                Player = new Furre();
        }

        #endregion Public Constructors
    }
}