﻿namespace Furcadia.Net.Utils.ServerParser
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
        public string AvatarID;

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        /// Remove Avatar from the Dream Furre List by its Furre ID
        /// </summary>
        /// <param name="ServerInstruction">
        /// </param>
        public RemoveAvatar(string ServerInstruction) : base(ServerInstruction)
        {
            if (ServerInstruction.StartsWith(")"))
                base.instructionType = ServerInstructionType.RemoveAvatar;
            AvatarID = ServerInstruction.Substring(1, 4);
        }

        #endregion Public Constructors
    }
}