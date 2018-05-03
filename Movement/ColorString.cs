using Furcadia.Text;
using System;

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
                Fur = Colors[1];
                Markings = Colors[2];
                Hair = Colors[3];
                Eye = Colors[4];
                Badge = Colors[5];
                Vest = Colors[6];
                Bracers = Colors[7];
                Cape = Colors[8];
                Boots = Colors[9];
                Trousers = Colors[10];
                Wings = Colors[11];
                Accent = Colors[12];

                if (Colors.Length > 13)
                {
                    Gender = Colors[13];
                    Species = Colors[14];
                    Avatar = Colors[15];
                }
            }
            else if (Colors[0] == 't' && Colors.Length >= 10)
            {
                Fur = Colors[1];
                Markings = Colors[2];
                Hair = Colors[3];
                Eye = Colors[4];
                Badge = Colors[5];
                Vest = Colors[6];
                Bracers = Colors[7];
                Cape = Colors[8];
                Boots = Colors[9];
                Trousers = Colors[10];

                #region Maybe Missing

                if (Colors.Length > 11)
                {
                    Gender = Colors[11];
                    Species = Colors[12];
                    Special = Colors[13];
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
            return $"w{Fur}{Markings}{Hair}{Eye}{Badge}{Vest}{Bracers}{Cape}{Boots}{Trousers}{Wings}{Accent}{Gender}{Species}{Avatar.ToString(2)}";
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
                Fur = Colors[1];
                Markings = Colors[2];
                Hair = Colors[3];
                Eye = Colors[4];
                Badge = Colors[5];
                Vest = Colors[6];
                Bracers = Colors[7];
                Cape = Colors[8];
                Boots = Colors[9];
                Trousers = Colors[10];
                Wings = Colors[11];
                Accent = Colors[12];
            }
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Acccent
        /// </summary>
        public Base220 Accent { get; set; }

        /// <summary>
        /// Avatar
        /// </summary>
        public Base220 Avatar { get; set; }

        /// <summary>
        /// Badge Color
        /// </summary>
        public Base220 Badge { get; set; }

        /// <summary>
        /// Boots Color
        /// </summary>
        public Base220 Boots { get; set; }

        /// <summary>
        /// Bracers color
        /// </summary>
        public Base220 Bracers { get; set; }

        /// <summary>
        /// cape color
        /// </summary>
        public Base220 Cape { get; set; }

        /// <summary>
        /// Eye color
        /// </summary>
        public Base220 Eye { get; set; }

        /// <summary>
        /// Fur color
        /// </summary>
        public Base220 Fur { get; set; }

        /// <summary>
        /// Avatar Gender
        /// </summary>
        public Base220 Gender { get; set; }

        /// <summary>
        /// Gets or sets the special.
        ///
        /// </summary>
        /// <value>
        /// The special.
        /// </value>
        [Obsolete("Legacy as of Furcadia V31", false)]
        public Base220 Special { get; set; }

        /// <summary>
        /// Hair color
        /// </summary>
        public Base220 Hair { get; set; }

        /// <summary>
        /// Markings color
        /// </summary>
        public Base220 Markings { get; set; }

        /// <summary>
        /// Avatar Species
        /// </summary>
        public Base220 Species { get; set; }

        /// <summary>
        /// Trousers color
        /// </summary>
        public Base220 Trousers { get; set; }

        /// <summary>
        /// /Vest Color
        /// </summary>
        public Base220 Vest { get; set; }

        /// <summary>
        /// Wings
        /// </summary>
        public Base220 Wings { get; set; }

        #endregion Public Properties

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="ColorString"/>.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ColorString(string colors)
        {
            return new ColorString(colors);
        }
    }
}