using Furcadia.Drawing;
using Furcadia.Movement;
using Furcadia.Net.DreamInfo;
using static Furcadia.Net.DreamInfo.Avatar;
using static Furcadia.Text.Base220;

namespace Furcadia.Net.Utils.ServerParser
{
    /// <summary>
    /// Process the Spaw Avatar Instruction
    /// </summary>
    /// <remarks>
    /// "&lt;" + user id + x + y + shape number + name + color code + flag
    /// + linefeed
    /// <para>
    /// <see href="http://dev.furcadia.com/docs/027_movement.html"/>
    /// </para>
    /// </remarks>
    public class SpawnAvatar : BaseServerInstruction
    {
        #region Protected Fields

        /// <summary>
        /// the Active Player
        /// </summary>
        public Furre player
        { get; internal set; }

        /// <summary>
        /// Spawing plags
        /// </summary>
        public CharacterFlags PlayerFlags
        { get; internal set; }

        #endregion Protected Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        /// <param name="ServerInstruction">
        /// </param>
        public SpawnAvatar(string ServerInstruction) : base(ServerInstruction)
        {
            //Update What type we are
            if (ServerInstruction[0] == '<')
                instructionType = ServerInstructionType.SpawnAvatar;

            int ColTypePos = (ServerInstruction[ConvertFromBase220(ServerInstruction[11])] == 'w') ? 16 : 14;

            PlayerFlags = new CharacterFlags(ServerInstruction[ColTypePos]);

            player = new Furre(ConvertFromBase220(ServerInstruction.Substring(1, 4)))
            {
                Name = ServerInstruction.Substring(12, ConvertFromBase220(ServerInstruction[11])),
                Position = new FurrePosition(ServerInstruction.Substring(5, 4)),
                Direction = (av_DIR)ConvertFromBase220(ServerInstruction.Substring(9, 1)),
                Pose = (FurrePose)ConvertFromBase220(ServerInstruction.Substring(10, 1)),
                AfkTime = ConvertFromBase220(ServerInstruction.Substring(ColTypePos + 1, 4)),
                FurreColors = new ColorString(ServerInstruction.Substring(ColTypePos, (ServerInstruction[ColTypePos] == 'w') ? 16 : 14))
            };

            //player.kittersize

            // reserverd for Future updates as Character Profiles come into existance
            //if (PlayerFlags.HasFlag(CHAR_FLAG_HAS_PROFILE))
            //{
            //}
        }

        #endregion Public Constructors
    }
}