using Furcadia.Text;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Furcadia.Net.DreamInfo
{
    /// <summary>
    /// Furre List information for a Furcadia Dream
    /// <para/>
    /// Implements List
    /// </summary>
    public class FurreList : List<Furre>, ICollection, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurreList"/> class.
        /// </summary>
        public FurreList() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurreList"/> class.
        /// </summary>
        /// <param name="ie">The ie.</param>
        public FurreList(IEnumerable<Furre> ie) : base(ie)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurreList"/> class.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public FurreList(int capacity) : base(capacity)
        {
        }

        #region Private Fields

        private object RemoveLockId = new object();
        private object RemoveLock = new object();
        private object AddLock = new object();

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized => ((ICollection)this).IsSynchronized;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
        /// </summary>
        public object SyncRoot => ((ICollection)this).SyncRoot;

        #endregion Public Properties

        #region Public Indexers

        /// <summary>
        /// Gets or set the Furre at index of fur
        /// </summary>
        /// <param name="fur">
        /// Furre
        /// </param>
        /// <returns>
        /// </returns>
        public Furre this[Furre fur]
        {
            get
            {
                return this[this.IndexOf(fur)];
            }
            set
            {
                this[this.IndexOf(fur)] = value;
            }
        }

        #endregion Public Indexers

        #region Public Methods

        ///// <summary>
        ///// Adds the specified furre.
        ///// </summary>
        ///// <param name="Furre">The furre.</param>
        //public void AddOrUpdate(Furre Furre)
        //{
        //    lock (AddLock)
        //    {
        //        if (!Contains(Furre))
        //            Add(Furre);
        //        else
        //        {
        //            var idx = IndexOf(Furre);
        //            this[idx] = Furre;
        //        }
        //    }
        //}

        /// <summary>
        /// Determines whether [contains] [the specified furre name].
        /// </summary>
        /// <param name="FurreName">Name of the furre.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified furre name]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string FurreName)
        {
            foreach (Furre fur in this)
            {
                if (fur.ShortName == FurreName.ToFurcadiaShortName())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether [contains] [the specified furre identifier].
        /// </summary>
        /// <param name="FurreId">The furre identifier.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified furre identifier]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Base220 FurreId)
        {
            var found = false;
            foreach (Furre fur in this)
            {
                if (fur.FurreID == FurreId)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        /// <summary>
        /// </summary>
        /// <param name="array">
        /// </param>
        /// <param name="index">
        /// </param>
        public void CopyTo(Array array, int index)
        {
            ((ICollection)this).CopyTo(array, index);
        }

        /// <summary>
        /// Get's a Furre from the Dream List bu it's ID
        /// </summary>
        /// <param name="FurreID">
        /// Base220 4 byte string representing the Furre ID
        /// </param>
        /// <returns>
        /// </returns>
        public Furre GetFurreByID(Base220 FurreID)
        {
            var fur = new Furre(FurreID);
            foreach (Furre Furre in this)
            {
                if (Furre.FurreID == FurreID)
                    fur = Furre;
            }
            return fur;
        }

        /// <summary>
        /// Gets the furre by name.
        /// </summary>
        /// <param name="FurreName">Name of the furre.</param>
        /// <returns><see cref="Furre"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Furre GetFurreByName(string FurreName)
        {
            if (string.IsNullOrEmpty(FurreName))
                throw new ArgumentNullException(FurreName);
            foreach (Furre Furre in this)
            {
                if (Furre.ShortName == FurreName.ToFurcadiaShortName())
                {
                    return Furre;
                }
            }
            return new Furre(0, FurreName);
        }

        /// <summary>
        /// Removes a Furre based on their Furre ID
        /// </summary>
        /// <param name="FurreID">
        /// </param>
        public bool Remove(Base220 FurreID)
        {
            lock (RemoveLockId)
            {
                Furre fur = null;
                foreach (Furre Fur in this)
                {
                    if (Fur.FurreID == FurreID)
                    {
                        fur = Fur;
                        break;
                    }
                }
                return this.Remove(fur);
            }
        }

        #endregion Public Methods
    }
}