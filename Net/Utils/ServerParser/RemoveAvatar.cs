using Furcadia.Net.DreamInfo;
using Furcadia.Text;

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
        public Base220 FurreId;

        /// <summary>
        /// Triggering furre being removed
        /// </summary>
        /// <value>
        /// Triggering furre.
        /// </value>
        public Furre Player { get; set; } = null;

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
                FurreId = ServerInstruction.Substring(1, 4);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Server Instruction Type: '{InstructionType}' FurreId: '{(int)FurreId}' Server: '{RawInstruction}'";
        }

        #endregion Public Constructors
    }
}