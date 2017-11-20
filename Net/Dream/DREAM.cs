using System;

namespace Furcadia.Net.Dream
{
    /// <summary>
    /// Current Dream information
    /// </summary>
    [CLSCompliant(true)]
    public class DREAM
    {
        #region Public Fields

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

        private FurreList furres;

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// private variables
        /// </summary>
        private string name, _Title, _Rating, _URL, owner;

        private int _Lines;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// List of Furres in the dream.
        /// </summary>
        public DREAM()
        {
            furres = new FurreList();
        }

        #endregion Public Constructors

        #region Public Properties

        private bool isModern;

        /// <summary>
        /// Is this dream Modern Mode?
        /// </summary>
        public bool IsModern
        {
            get { return isModern; }
            set { isModern = value; }
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

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="dreamA"></param>
        ///// <param name="DreamB"></param>
        ///// <returns></returns>
        //public static bool operator ==(DREAM dreamA, DREAM dreamB)
        //{
        //    if (dreamB == null || dreamA == null)
        //        return false;
        //    return dreamA.Equals(dreamB);
        //}

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="dreamA"></param>
        ///// <param name="DreamB"></param>
        ///// <returns></returns>
        //public static bool operator !=(DREAM dreamA, DREAM DreamB)
        //{
        //    if (DreamB == null)
        //    {
        //        return false;
        //    }

        //    return dreamA.ShortName != DreamB.ShortName;
        //}

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="other"></param>
        ///// <returns></returns>
        //public bool Equals(DREAM other)
        //{
        //    if (other == null)
        //        return false;
        //    return ShortName == other.ShortName;
        //}

        #endregion Public Properties
    }
}