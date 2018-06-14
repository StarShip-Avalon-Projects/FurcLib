using Furcadia.Drawing;
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
            switch (ServerInstruction[0])
            {
                case '/':
                    instructionType = ServerInstructionType.AnimatedMoveAvatar;
                    break;

                case 'A':
                    instructionType = ServerInstructionType.MoveAvatar;
                    break;
            }

            if (ServerInstruction.Length > 4)
                Player = new Furre(0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveFurre"/> class.
        /// </summary>
        /// <param name="ServerInstruction">The server instruction.</param>
        /// <param name="ActiveFurre">The active furre.</param>
        public MoveFurre(string ServerInstruction, ref Furre ActiveFurre) : this(ServerInstruction)
        {
            ActiveFurre.Location = new FurrePosition(ServerInstruction.Substring(5, 4));
            Player = ActiveFurre;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Server Instruction Type: '{InstructionType}' Furre: '{Player}' Server: '{RawInstruction}'";
        }

        #endregion Public Constructors
    }
}