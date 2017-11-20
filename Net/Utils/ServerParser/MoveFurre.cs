using Furcadia.Net.Dream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furcadia.Net.Utils.ServerParser
{
    /// <summary>
    /// Animated or noanimated move object
    /// </summary>
    public class MoveFurre : BaseServerInstruction
    {
        public Furre Player;

        /// <summary>
        ///
        /// </summary>
        /// <param name="ServerInstruction"></param>
        public MoveFurre(string ServerInstruction) : base(ServerInstruction)
        {
            if (ServerInstruction[0] == '/')
                base.instructionType = ServerInstructionType.AnimatedMoveAvatar;
            else if (ServerInstruction[0] == 'A')
                base.instructionType = ServerInstructionType.MoveAvatar;
            if (ServerInstruction.Length > 4)
                Player = new Furre();
        }
    }
}