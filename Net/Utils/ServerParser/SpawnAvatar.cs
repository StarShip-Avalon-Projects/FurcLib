using Furcadia.Movement;
using Furcadia.Net.Dream;
using static Furcadia.Net.Dream.Avatar;
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
        public FURRE player;

        /// <summary>
        /// Spawing plags
        /// </summary>
        public CharacterFlags PlayerFlags;

        #endregion Protected Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        /// <param name="ServerInstruction">
        /// </param>
        public SpawnAvatar(string ServerInstruction) : base(ServerInstruction)
        {
            //Update What type we are
            if (ServerInstruction.StartsWith("<"))
                base.instructionType = ServerInstructionType.SpawnAvatar;

            //FUID Furre ID 4 Base220 bytes
            var id = ConvertFromBase220(ServerInstruction.Substring(1, 4));

            var PosX = ServerInstruction.Substring(5, 2);
            var PosY = ServerInstruction.Substring(7, 2);
            var Position = new Drawing.FurrePosition(PosX, PosY);

            var FurreDirection = ConvertFromBase220(ServerInstruction.Substring(9, 1));
            var FurrePose = ConvertFromBase220(ServerInstruction.Substring(10, 1));
            var NameIdx = ConvertFromBase220(ServerInstruction.Substring(11, 1));
            var NameLength = NameIdx + 12;

            var name = ServerInstruction.Substring(12, NameIdx);

            var ColTypePos = 12 + NameIdx;
            var ColorLength = (ServerInstruction[ColTypePos] == 'w') ? 16 : 14;

            player = new FURRE(id, name);
            player.Position = Position;
            player.Direction = (av_DIR)FurreDirection;
            player.Pose = (FurrePose)FurrePose;

            player.Color = new ColorString(ServerInstruction.Substring(ColTypePos, ColorLength));

            int FlagPos = ServerInstruction.Length - 6;

            ColTypePos += ColorLength;
            PlayerFlags = new CharacterFlags(ServerInstruction.Substring(ColTypePos, 1));

            player.AFK = ConvertFromBase220(ServerInstruction.Substring(ColTypePos, 1));
            //player.kittersize

            // reserverd for Future updates as Character Profiles come into existance
            //if (PlayerFlags.HasFlag(CHAR_FLAG_HAS_PROFILE))
            //{
            //}
        }

        #endregion Public Constructors
    }
}