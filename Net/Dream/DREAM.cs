using System;

namespace Furcadia.Net
{
    /// <summary>
    /// Current Dream information
    /// </summary>
    [CLSCompliant(true)]
    public class DREAM
    {
        #region Public Fields

        /// <summary>
        /// Dream List Furcadia requires Clients to handle thier own Dream Lists See
        /// <para>
        /// http://dev.furcadia.com/docs New Movement for Spawn and Remove packets
        /// </para>
        /// <para>
        /// **Spawn is out dated. New information requires a 4byte for AFK flag at the end
        /// </para>
        /// <para>
        /// As of V31, Color code has changed.
        /// </para>
        /// </summary>
        public FURREList FurreList = new FURREList();

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// private variables
        /// </summary>
        private static string _Name, _Title, _Lines, _Rating, _URL, _Owner;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Number of DS Lines
        /// </summary>
        public static string Lines
        {
            get { return _Lines; }
            set { _Lines = value; }
        }

        /// <summary>
        /// Name of the dream
        /// </summary>
        public static string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// Dreams uploader character
        /// </summary>
        public static string Owner
        {
            get { return _Owner; }
            set { _Owner = value; }
        }

        /// <summary>
        /// Furcadia Dream rating
        /// </summary>
        public static string Rating
        {
            get { return _Rating; }
            set { _Rating = value; }
        }

        /// <summary>
        /// Dream title
        /// </summary>
        public static string Title
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
        public static string URL
        {
            get { return _URL; }
            set { _URL = value; }
        }

        #endregion Public Properties
    }
}