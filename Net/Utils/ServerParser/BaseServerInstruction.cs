using System;

namespace Furcadia.Net.Utils.ServerParser
{
    /// <summary>
    /// Server instruction object base class
    /// </summary>
    [CLSCompliant(true)]
    public class BaseServerInstruction
    {
        #region Private Fields

        /// <summary>
        /// </summary>
        protected ServerInstructionType instructionType;

        private string rawInstruction;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Reads the raw server instruction and set this object to its settings
        /// <para>
        /// Default Server instruction type is <see cref="ServerInstructionType.Unknown"/>
        /// </para>
        /// </summary>
        /// <param name="ServerInstruction">
        /// raw server instruction
        /// </param>
        public BaseServerInstruction(string ServerInstruction)
        {
            rawInstruction = ServerInstruction;
            instructionType = ServerInstructionType.Unknown;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Which Server to Client Instruction are we?
        /// </summary>
        [CLSCompliant(false)]
        public ServerInstructionType InstructionType
        {
            get
            {
                return instructionType;
            }
            set
            {
                instructionType = value;
            }
        }

        /// <summary>
        /// Raw Server to Client instruction
        /// </summary>
        public string RawInstruction
        {
            get
            {
                return rawInstruction;
            }
        }

        #endregion Public Properties
    }
}