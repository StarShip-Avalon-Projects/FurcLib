using Furcadia.Movement;
using static Furcadia.Text.Base220;

namespace Furcadia.Net.Utils.ServerParser
{
    /// <summary>
    /// Process the Spaw Avatar Instruction
    /// </summary>
    /// <remarks>
    /// '$glt;' + user id + x + y + shape number + name + color code + flag
    /// + linefeed
    /// <para>
    /// reference http://dev.furcadia.com/docs/027_movement.html
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
            player = new FURRE(ConvertFromBase220(ServerInstruction.Substring(1, 4)));

            // Y,Y 2 Base220 Bytes each X Coordinates are *2 in Map editor
            var Position = new Drawing.FurrePosition(ServerInstruction.Substring(5, 2),
                ServerInstruction.Substring(7, 2));
            Position.x = Position.x * 2;
            player.Position = Position;

            //Character's avatar shape (its look)
            player.Shape = ConvertFromBase220(ServerInstruction.Substring(9, 2));

            // Name is a Base220 String
            var name = ServerInstruction.Substring(11);
            int NameLength = Base220StringLengeth(ref name);
            this.player.Name = name;

            int ColTypePos = 12 + NameLength + 1;
            player.Color = new ColorString(ServerInstruction.Substring(ColTypePos, ColorString.ColorStringSize));

            int FlagPos = ServerInstruction.Length - 6;

            PlayerFlags = new CharacterFlags(ServerInstruction.Substring(FlagPos, 1));

            var AFK_Pos = ServerInstruction.Length - 5;
            var AFKStr = ServerInstruction.Substring(AFK_Pos, 4);
            player.AFK = ConvertFromBase220(AFKStr);

            // reserverd for Future updates as Character Profiles come into existance
            //if (PlayerFlags.HasFlag(CHAR_FLAG_HAS_PROFILE))
            //{
            //}
        }

        #endregion Public Constructors
    }
}