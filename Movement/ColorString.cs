using Furcadia.Text;

namespace Furcadia.Movement
{
    /// <summary>
    /// Furcadia (v31) Color string format.
    /// </summary>
    /// <remarks>
    /// This is derived content from the Furcadia Dev Docs and Furcadia
    /// Technical Resources
    /// <para>
    /// Update 23 Avatar Moement http://dev.furcadia.com/docs/023_new_movement.pdf
    /// </para>
    /// <para>
    /// Update 27 Movement http://dev.furcadia.com/docs/027_movement.html
    /// </para>
    /// <para>
    /// FTR http://ftr.icerealm.org/ref-instructions/
    /// </para>
    /// </remarks>
    public class ColorString
    {
        #region Public Constructors

        /// <summary>
        /// wide format ("w") String size
        /// </summary>
        public const int ColorStringSize = 13;

        /// <summary>
        /// Default Construtor
        /// </summary>
        public ColorString()
        {
        }

        /// <summary>
        /// Constructor with <see cref="Base220"/> encoded ColorStrinhg
        /// </summary>
        /// <param name="Colors">
        /// Color String in legacy "t" format or new "w" format
        /// </param>
        public ColorString(string Colors)
        {
            if (Colors[0] == 'w' && Colors.Length >= ColorStringSize)
            {
                fur = Base220.ConvertFromBase220(Colors[1].ToString());
                markings = Base220.ConvertFromBase220(Colors[2].ToString());
                hair = Base220.ConvertFromBase220(Colors[3].ToString());
                eye = Base220.ConvertFromBase220(Colors[4].ToString());
                badge = Base220.ConvertFromBase220(Colors[5].ToString());
                vest = Base220.ConvertFromBase220(Colors[6].ToString());
                bracers = Base220.ConvertFromBase220(Colors[7].ToString());
                cape = Base220.ConvertFromBase220(Colors[8].ToString());
                boots = Base220.ConvertFromBase220(Colors[9].ToString());
                trousers = Base220.ConvertFromBase220(Colors[10].ToString());
                _wings = Base220.ConvertFromBase220(Colors[11].ToString());
                _accent = Base220.ConvertFromBase220(Colors[12].ToString());

                if (Colors.Length > 13)
                {
                    gender = Base220.ConvertFromBase220(Colors[13].ToString());
                    species = Base220.ConvertFromBase220(Colors[14].ToString());
                    avatar = Base220.ConvertFromBase220(Colors[15].ToString());
                }
            }
            else if (Colors[0] == 't' && Colors.Length >= 11)
            {
                fur = Base220.ConvertFromBase220(Colors[1].ToString());
                markings = Base220.ConvertFromBase220(Colors[2].ToString());
                hair = Base220.ConvertFromBase220(Colors[3].ToString());
                eye = Base220.ConvertFromBase220(Colors[4].ToString());
                badge = Base220.ConvertFromBase220(Colors[5].ToString());
                vest = Base220.ConvertFromBase220(Colors[6].ToString());
                bracers = Base220.ConvertFromBase220(Colors[7].ToString());
                cape = Base220.ConvertFromBase220(Colors[8].ToString());
                boots = Base220.ConvertFromBase220(Colors[9].ToString());
                trousers = Base220.ConvertFromBase220(Colors[10].ToString());

                #region Maybe Missing

                if (Colors.Length > 11)
                {
                    gender = Base220.ConvertFromBase220(Colors[11].ToString());
                    species = Base220.ConvertFromBase220(Colors[12].ToString());
                    avatar = Base220.ConvertFromBase220(Colors[13].ToString());
                }

                #endregion Maybe Missing
            }
        }

        /// <summary>
        /// ColorString String Lengeth
        /// </summary>
        public int Length => ColorStringSize;

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Output the Base220 encoded color string
        /// </summary>
        /// <returns>
        /// Furcadia color-string in modern "w" format
        /// </returns>
        public override string ToString()
        {
            return string.Format("w{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}",
                Base220.ConvertToBase220(fur),
                Base220.ConvertToBase220(markings),
                Base220.ConvertToBase220(hair),
                Base220.ConvertToBase220(eye),
                Base220.ConvertToBase220(badge),
                Base220.ConvertToBase220(vest),
                Base220.ConvertToBase220(bracers),
                Base220.ConvertToBase220(cape),
                Base220.ConvertToBase220(boots),
                Base220.ConvertToBase220(trousers),

                // Thanks DD for the v31 Update
                Base220.ConvertToBase220(_wings),
                Base220.ConvertToBase220(_accent),

            #region Maybe Missing

                Base220.ConvertToBase220(gender),
                Base220.ConvertToBase220(species),
                Base220.ConvertToBase220(avatar)

            #endregion Maybe Missing

                );
        }

        /// <summary>
        /// Update the Furre's color-code
        /// </summary>
        /// <param name="Colors">
        /// Partial Color String
        /// </param>
        public void Update(string Colors)
        {
            if (Colors[0] == 'w' && Colors.Length == ColorStringSize)
            {
                fur = Base220.ConvertFromBase220(Colors[1].ToString());
                markings = Base220.ConvertFromBase220(Colors[2].ToString());
                hair = Base220.ConvertFromBase220(Colors[3].ToString());
                eye = Base220.ConvertFromBase220(Colors[4].ToString());
                badge = Base220.ConvertFromBase220(Colors[5].ToString());
                vest = Base220.ConvertFromBase220(Colors[6].ToString());
                bracers = Base220.ConvertFromBase220(Colors[7].ToString());
                cape = Base220.ConvertFromBase220(Colors[8].ToString());
                boots = Base220.ConvertFromBase220(Colors[9].ToString());
                trousers = Base220.ConvertFromBase220(Colors[10].ToString());
                _wings = Base220.ConvertFromBase220(Colors[11].ToString());
                _accent = Base220.ConvertFromBase220(Colors[12].ToString());
            }
        }

        #endregion Public Methods

        #region Private Fields

        private int _accent;
        private int _wings;
        private int avatar;
        private int badge;
        private int boots;
        private int bracers;
        private int cape;
        private int eye;
        private int fur;
        private int gender;
        private int hair;
        private int markings;
        private int species;
        private int trousers;
        private int vest;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Acccent
        /// </summary>
        public int Accent
        {
            get
            {
                return _accent;
            }

            set
            {
                _accent = value;
            }
        }

        /// <summary>
        /// Avatar
        /// </summary>
        public int Avatar
        {
            get
            {
                return avatar;
            }
            set
            {
                avatar = value;
            }
        }

        /// <summary>
        /// Badge Color
        /// </summary>
        public int Badge
        {
            get
            {
                return badge;
            }
            set
            {
                badge = value;
            }
        }

        /// <summary>
        /// Boots Color
        /// </summary>
        public int Boots
        {
            get
            {
                return boots;
            }
            set
            {
                boots = value;
            }
        }

        /// <summary>
        /// Bracers color
        /// </summary>
        public int Bracers
        {
            get
            {
                return bracers;
            }
            set
            {
                bracers = value;
            }
        }

        /// <summary>
        /// cape color
        /// </summary>
        public int Cape
        {
            get
            {
                return cape;
            }
            set
            {
                cape = value;
            }
        }

        /// <summary>
        /// Eye color
        /// </summary>
        public int Eye
        {
            get
            {
                return eye;
            }
            set
            {
                eye = value;
            }
        }

        /// <summary>
        /// Fur color
        /// </summary>
        public int Fur
        {
            get
            {
                return fur;
            }
            set
            {
                fur = value;
            }
        }

        /// <summary>
        /// Avatar Gender
        /// </summary>
        public int Gender
        {
            get
            {
                return gender;
            }
            set
            {
                gender = value;
            }
        }

        /// <summary>
        /// Hair color
        /// </summary>
        public int Hair
        {
            get
            {
                return hair;
            }
            set
            { hair = value; }
        }

        /// <summary>
        /// Markings color
        /// </summary>
        public int Markings
        {
            get
            {
                return markings;
            }
            set
            {
                markings = value;
            }
        }

        /// <summary>
        /// Avatar Species
        /// </summary>
        public int Species
        {
            get
            {
                return species;
            }
            set
            {
                species = value;
            }
        }

        /// <summary>
        /// Trousers color
        /// </summary>
        public int Trousers
        {
            get
            {
                return trousers;
            }
            set
            {
                trousers = value;
            }
        }

        /// <summary>
        /// /Vest Color
        /// </summary>
        public int Vest
        {
            get
            {
                return vest;
            }
            set
            {
                vest = value;
            }
        }

        /// <summary>
        /// Wings
        /// </summary>
        public int Wings
        {
            get
            {
                return _wings;
            }
            set
            {
                _wings = value;
            }
        }

        #endregion Public Properties
    }
}