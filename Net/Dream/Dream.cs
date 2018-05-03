using System;
using System.Text;
using Furcadia.Logging;
using Furcadia.Net.Utils.ServerParser;

namespace Furcadia.Net.DreamInfo
{
    /// <summary>
    ///
    /// </summary>
    public interface IDream
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is modern.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is modern; otherwise, <c>false</c>.
        /// </value>
        bool IsModern { get; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        #endregion Public Properties
    }

    /// <summary>
    /// Current Dream information
    /// </summary>
    [CLSCompliant(true)]
    public class Dream : IDream
    {
        #region Private Fields

        private int _Lines;

        private string dreamTitle, _Rating, dreamOwner;

        private string fileName;
        private static FurreList furres;
        private bool isPermament;
        private string mode;

        /// <summary>
        /// Gets or sets a value indicating whether Bot has just arrived in a new dream;.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [just arrived]; otherwise, <c>false</c>.
        /// </value>
        public bool JustArrived { get; set; }

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Dream"/> class.
        /// </summary>
        public Dream()
        {
            furres = new FurreList();
            mode = "legacy";
            JustArrived = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dream"/> class.
        /// </summary>
        /// <param name="DreamOwner">The dream owner.</param>
        public Dream(string DreamOwner) : this()
        {
            this.dreamOwner = DreamOwner;
            JustArrived = true;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the book make.
        /// </summary>
        /// <value>
        /// The book make.
        /// </value>
        public DreamBookmark BookMark
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value.DreamOwner))
                    throw new ArgumentNullException(value.DreamOwner);
                //Modern should be set bu this point
                dreamTitle = value.Title;
                if (string.IsNullOrEmpty(dreamOwner))
                    dreamOwner = value.DreamOwner;
            }
        }

        /// <summary>
        /// Dreams uploader character
        /// </summary>
        public string DreamOwner
        {
            get => dreamOwner;
            set => dreamOwner = value;
        }

        /// <summary>
        /// Dreams full Furcadia Drean URL
        /// <para>
        /// IE: 'fdl furc://DreamOwner:DreamTitle/EntryCode#
        /// </para>
        /// </summary>
        public string DreamUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(dreamOwner) && string.IsNullOrWhiteSpace(dreamTitle))
                    return null;
                if (string.IsNullOrWhiteSpace(dreamOwner))
                    throw new ArgumentException(dreamOwner);
                var sb = new StringBuilder($"furc://");
                sb.Append($"{ dreamOwner.ToFurcadiaShortName()}");
                if (!string.IsNullOrWhiteSpace(dreamTitle))
                    sb.Append($":{dreamTitle.ToFurcadiaShortName()}");
                sb.Append("/");
                Logger.Debug<Dream>(sb);
                return sb.ToString();
            }
        }

        /// <summary>
        /// File name for the dream cache stored on disk
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }

        /// <summary>
        /// Dream List Furcadia requires Clients to handle thier own Dream
        /// Lists See
        /// <para>
        /// http://dev.furcadia.com/docs New Movement for Spawn and Remove packets
        /// </para>
        /// <para>
        /// **Spawn is out dated. New information requires a 4byte for AFK
        ///   flag at the end
        /// </para>
        /// <para>
        /// As of V31, Color code has changed.
        /// </para>
        /// </summary>
        public FurreList Furres
        {
            get => furres;
            set => furres = value;
        }

        /// <summary>
        /// Is this dream Modern Mode?
        /// </summary>
        public bool IsModern => mode == "modern";

        /// <summary>
        /// Gets a value indicating whether this dream is permament.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this dream is permament; otherwise, <c>false</c>.
        /// </value>
        public bool IsPermanent => isPermament;

        /// <summary>
        /// Number of DS Lines
        /// </summary>
        public int Lines
        {
            get => _Lines;
            set => _Lines = value;
        }

        /// <summary>
        /// Name of the dream
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(dreamOwner) && string.IsNullOrWhiteSpace(dreamTitle))
                    return null;
                var sb = new StringBuilder();
                sb.Append($"{dreamOwner}");
                if (!string.IsNullOrWhiteSpace(dreamTitle))
                    sb.Append($":{dreamTitle}");
                Logger.Debug<Dream>(sb);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Furcadia Dream rating
        /// </summary>
        public string Rating
        {
            get => _Rating;
            set => _Rating = value;
        }

        /// <summary>
        /// Dream title
        /// </summary>
        public string Title
        {
            get => dreamTitle;
            set => dreamTitle = value;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="dreamA">The dream a.</param>
        /// <param name="DreamB">The dream b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Dream dreamA, IDream dreamB)
        {
            if (dreamA is null)
            {
                return dreamB is null;
            }
            return !dreamA.Equals(dreamB);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="dreamA">The dream a.</param>
        /// <param name="dreamB">The dream b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Dream dreamA, IDream dreamB)
        {
            if (dreamA is null)
            {
                return dreamB is null;
            }

            return dreamA.Equals(dreamB);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is IDream dream)
            {
                return Name == dream.Name;
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ dreamOwner.GetHashCode(); ;
        }

        /// <summary>
        /// Loads the specified dream information from a <see cref="LoadDream"/> event.
        /// </summary>
        /// <param name="DreamInfo">The dream information.</param>
        public void Load(LoadDream DreamInfo)
        {
            if (DreamInfo.IsModern)
                mode = "modern";

            fileName = DreamInfo.CacheFileName;
            if (FileName.Length > 2)
                dreamTitle = fileName.Substring(2);
            isPermament = DreamInfo.IsPermanent;
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion Public Methods
    }
}