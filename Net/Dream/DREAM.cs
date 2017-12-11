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
        string Name { get; set; }

        /// <summary>
        /// Short name of the dream
        /// </summary>
        string ShortName
        {
            get;
        }
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
        private string name, _Title, _Rating, _URL, owner;

        private int _Lines;

        private FurreList furres;

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
        /// List of Furres in the dream.
        /// </summary>
        public Dream(LoadDream loadDream)
        {
            furres = new FurreList();
            mode = "legacy";
            if (loadDream.IsModern)
                mode = "modern";

            fileName = loadDream.Name;
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
            get { return furres; }
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
            get { return fileName; }
            set { fileName = value; }
        }

        /// <summary>
        /// Is this dream Modern Mode?
        /// </summary>
        public bool IsModern
        {
            get { return mode == "modern"; }
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
                if (value.IsModern)
                    mode = "modern";
                name = value.Name;
                _URL = value.DreamUrl;
                owner = value.DreamOwner;
            }
        }

        /// <summary>
        /// Number of DS Lines
        /// </summary>
        public int Lines
        {
            get { return _Lines; }
            set { _Lines = value; }
        }

        /// <summary>
        /// Name of the dream
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Name of the dream
        /// </summary>
        public string ShortName
        {
            get { return name.ToFurcadiaShortName(); }
        }

        /// <summary>
        /// Dreams uploader character
        /// </summary>
        public string Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        /// <summary>
        /// Dreams uploader character
        /// </summary>
        public string OwnerShortName
        {
            get { return owner.ToFurcadiaShortName(); }
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
            get { return _URL; }
            set { _URL = value; }
        }

        #endregion Public Properties

        #region Public Operators

        /// <summary>
        ///
        /// </summary>
        /// <param name="dreamA"></param>
        /// <param name="dreamB"></param>
        /// <returns></returns>
        public static bool operator ==(Dream dreamA, IDream dreamB)
        {
            if (dreamB == null || dreamA == null)
                return false;
            return dreamA.Equals(dreamB);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dreamA"></param>
        /// <param name="DreamB"></param>
        /// <returns></returns>
        public static bool operator !=(Dream dreamA, IDream DreamB)
        {
            if (DreamB == null)
            {
                return false;
            }

            return dreamA.ShortName != DreamB.ShortName;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (other is IFurre fur)
            {
                return ShortName == fur.ShortName;
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
            return ShortName.GetHashCode();
        }

        #endregion Public Operators
    }
}