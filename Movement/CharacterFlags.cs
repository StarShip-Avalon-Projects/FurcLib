using System.Collections.Specialized;
using static Furcadia.Text.Base220;

namespace Furcadia.Movement
{
    /// <summary>
    /// Furcadia reconmended Material to support thier Protocol Standards
    /// </summary>
    public class CharacterFlags
    {
        #region Public Fields

        /// <summary>
        /// Character has a Web Profile page.
        /// <para>
        /// As of Furcadia V31 This is not yet used
        /// </para>
        /// </summary>
        public const int CHAR_FLAG_HAS_PROFILE = 1;

        /// <summary>
        /// Shown for new arrivals to the dream
        /// </summary>
        public const int CHAR_FLAG_NEW_AVATAR = 4;

        /// <summary>
        /// Character has no Flags set
        /// </summary>
        public const int CHAR_FLAG_NONE = 0;

        /// <summary>
        /// Set Character Visable
        /// </summary>
        public const int CHAR_FLAG_SET_VISIBLE = 2;

        #endregion Public Fields

        #region Private Fields

        private static BitVector32 CharFlags;

        #endregion Private Fields

        #region Public Constructors

        ///// <summary>
        ///// Using flags with integer
        ///// </summary>
        ///// <param name="flags">
        ///// Integer Flags
        ///// </param>
        //public CharacterFlags(int flags)
        //{
        //    CharFlags = new BitVector32(flags);
        //}

        ///// <summary>
        ///// default construtor
        ///// </summary>
        //public CharacterFlags()
        //{
        //    CharFlags = new BitVector32(0);
        //}

        /// <summary>
        /// Build Flags with Base220 string
        /// </summary>
        /// <param name="flags">
        /// Base220 String
        /// </param>
        public CharacterFlags(string flags)
        {
            int test = ConvertFromBase220(flags);
            CharFlags = new BitVector32(ConvertFromBase220(flags));
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Does the triggering furre have flags set?
        /// </summary>
        /// <param name="FlagToCheck">
        /// Any one of the CHAR_FLAG_ set
        /// </param>
        /// <returns>
        /// true if the flag is set
        /// </returns>
        public bool HasFlag(int FlagToCheck)
        {
            bool test = CharFlags[FlagToCheck];
            return CharFlags[FlagToCheck];
        }

        #endregion Public Methods
    }
}