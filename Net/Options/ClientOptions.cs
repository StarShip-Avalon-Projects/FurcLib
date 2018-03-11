using Furcadia.Movement;

namespace Furcadia.Net.Options
{
    /// <summary>
    /// Client options based on Character.ini settings
    /// </summary>
    public class ClientOptions : BaseConnectionOptions
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientOptions"/> class.
        /// </summary>
        public ClientOptions() : base()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Optional Account E-Mail
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Character Name
        /// </summary>
        public string CharacterName { get; set; }

        /// <summary>
        /// Gets the short name of the character.
        /// </summary>
        /// <value>
        /// The short name of the character.
        /// </value>
        public string CharacterShortName => CharacterName.ToFurcadiaShortName();

        /// <summary>
        /// Gets or sets the avatar colors.
        /// </summary>
        /// <value>
        /// The colors.
        /// </value>
        public ColorString Colors { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public double Version { get; set; }

        /// <summary>
        /// Gets or sets the avatar description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Costume to use with Selected Character
        /// </summary>
        public string Costume { get; set; }

        #endregion Public Properties
    }
}