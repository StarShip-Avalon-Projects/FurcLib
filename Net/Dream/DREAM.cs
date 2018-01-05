using System;
using Furcadia.Net.Utils.ServerParser;

namespace Furcadia.Net.DreamInfo
{
    /// <summary>
    ///
    /// </summary>

    public interface IDream
    {
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
    }

    /// <summary>
    /// Current Dream information
    /// </summary>
    [CLSCompliant(true)]
    public class Dream : IDream
    {
        #region Private Fields

        private string mode;

        private string fileName;

        /// <summary>
        /// private variables
        /// </summary>
        private string _Title, _Rating, owner;

        private int _Lines;

        private FurreList furres;
        private bool isPermament;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Dream"/> class.
        /// </summary>
        public Dream()
        {
            furres = new FurreList();
            mode = "legacy";
        }

        /// <summary>
        /// Loads the specified dream information from a <see cref="LoadDream"/> event.
        /// </summary>
        /// <param name="DreamInfo">The dream information.</param>
        public void Load(LoadDream DreamInfo)
        {
            furres.Clear();
            mode = "legacy";
            if (DreamInfo.IsModern)
                mode = "modern";

            fileName = DreamInfo.CacheFileName;
            if (DreamInfo.IsPermanent)
                _Title = fileName.Substring(2);
            isPermament = DreamInfo.IsPermanent;
        }

        #endregion Public Constructors

        #region Public Properties

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
            set { furres = value; }
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
            set { fileName = value; }
        }

        /// <summary>
        /// Is this dream Modern Mode?
        /// </summary>
        public bool IsModern
        {
            get => mode == "modern";
        }

        /// <summary>
        /// Gets a value indicating whether this dream is permament.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this dream is permament; otherwise, <c>false</c>.
        /// </value>
        public bool IsPermanent
        {
            get => isPermament;
        }

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
                //Modern should be set bu this point
                _Title = value.Title;
                owner = value.DreamOwner;
            }
        }

        /// <summary>
        /// Number of DS Lines
        /// </summary>
        public int Lines
        {
            get => _Lines;
            set { _Lines = value; }
        }

        /// <summary>
        /// Name of the dream
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(owner))
                    return $"{_Title}";
                return $"{owner}:{_Title}";
            }
        }

        /// <summary>
        /// Dreams uploader character
        /// </summary>
        public string DreamOwner
        {
            get => owner;
            set { owner = value; }
        }

        /// <summary>
        /// Furcadia Dream rating
        /// </summary>
        public string Rating
        {
            get { return _Rating; }
            set { _Rating = value; }
        }

        /// <summary>
        /// Dream title
        /// </summary>
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        /// <summary>
        /// Dreams full Furcadia Drean URL
        /// <para>
        /// IE: 'fdl furc://DreamOwner:DreamTitle/EntryCode#
        /// </para>
        /// </summary>
        public string URL
        {
            get
            {
                if (string.IsNullOrWhiteSpace(owner))
                {
                    return $"furc://{_Title}/";
                }
                return $"furc://{owner.ToFurcadiaShortName()}:{_Title}/";
            }
        }

        #endregion Public Properties

        #region Public Operators

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
            if (ReferenceEquals(dreamA, null))
            {
                return ReferenceEquals(dreamB, null);
            }

            return dreamA.Equals(dreamB);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="dreamA">The dream a.</param>
        /// <param name="DreamB">The dream b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Dream dreamA, IDream DreamB)
        {
            return !(dreamA == DreamB);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (other is IDream dream)
            {
                return Name.ToLower() == dream.Name.ToLower();
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
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

        #endregion Public Operators
    }
}