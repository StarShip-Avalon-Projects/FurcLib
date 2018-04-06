using Furcadia.Text;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Furcadia.Net.DreamInfo
{
    /// <summary>
    /// Furre List information for a Furcadia Dream
    /// <para>
    /// This class acts like an enhanced List(of &lt;T&gt;) because you can
    /// Select a Furre by Item as well as index
    /// </para>
    /// </summary>
    public class FurreList : IList<IFurre>, ICollection
    {
        #region Protected Internal Fields

        private static IList<IFurre> furreList;

        #endregion Protected Internal Fields

        #region Private Fields

        private object RemoveLock = new object();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        public FurreList()
        {
            furreList = new List<IFurre>(100);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Number of Avatars in the Dream
        /// </summary>
        public int Count => furreList.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly => furreList.IsReadOnly;

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized => ((ICollection)furreList).IsSynchronized;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
        /// </summary>
        public object SyncRoot => ((ICollection)furreList).SyncRoot;

        /// <summary>
        /// Gets to i list.
        /// </summary>
        /// <value>
        /// To i list.
        /// </value>
        public IList<IFurre> ToIList => furreList;

        #endregion Public Properties

        #region Public Indexers

        /// <summary>
        /// Gets or sets the <see cref="Furre"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="Furre"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public IFurre this[int index]
        {
            get
            {
                return furreList[index];
            }
            set
            {
                furreList[index] = value;
            }
        }

        /// <summary>
        /// Gets or set the Furre at index of fur
        /// </summary>
        /// <param name="fur">
        /// Furre
        /// </param>
        /// <returns>
        /// </returns>
        public IFurre this[IFurre fur]
        {
            get
            {
                if (furreList.Contains(fur))
                    return furreList[furreList.IndexOf(fur)];
                fur.FurreID = 0;
                return fur;
            }
            set
            {
                if (furreList.Contains(fur))
                    furreList[furreList.IndexOf(fur)] = value;
            }
        }

        #endregion Public Indexers

        #region Public Methods

        /// <summary>
        /// Adds the specified furre.
        /// </summary>
        /// <param name="Furre">The furre.</param>
        public void Add(IFurre Furre)
        {
            if (!furreList.Contains(Furre))
                furreList.Add(Furre);
            else
                furreList[furreList.IndexOf(Furre)] = Furre;
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            furreList.Clear();
        }

        /// <summary>
        /// Determines whether [contains] [the specified furre identifier].
        /// </summary>
        /// <param name="FurreID">The furre identifier.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified furre identifier]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(int FurreID)
        {
            foreach (var fur in furreList)
            {
                if (fur.FurreID == FurreID)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether [contains] [the specified furre name].
        /// </summary>
        /// <param name="FurreName">Name of the furre.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified furre name]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string FurreName)
        {
            foreach (var fur in furreList)
            {
                if (fur.ShortName == FurreName.ToFurcadiaShortName())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether [contains] [the specified furre].
        /// </summary>
        /// <param name="Furre">The furre.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified furre]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(IFurre Furre)
        {
            foreach (Furre fur in furreList)
            {
                if (fur == Furre)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="array">
        /// </param>
        /// <param name="index">
        /// </param>
        public void CopyTo(Array array, int index)
        {
            ((ICollection)furreList).CopyTo(array, index);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(IFurre[] array, int arrayIndex)
        {
            furreList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return ((ICollection)furreList).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<IFurre> IEnumerable<IFurre>.GetEnumerator()
        {
            return furreList.GetEnumerator();
        }

        /// <summary>
        /// Get's a Furre from the Dream List bu it's ID
        /// </summary>
        /// <param name="FurreID">
        /// Base220 4 byte string representing the Furre ID
        /// </param>
        /// <returns>
        /// </returns>
        public IFurre GetFurreByID(Base220 FurreID)
        {
            foreach (var Furre in furreList)
            {
                if (Furre.FurreID == FurreID)
                    return Furre;
            }
            return new Furre(FurreID);
        }

        /// <summary>
        /// get a Furre from the Furrelist by its integer idd
        /// </summary>
        /// <param name="FurreID">
        /// Furre ID as integer
        /// </param>
        /// <returns>
        /// Furre name with a real Furcadia ID if the furre is in the dream
        /// Other wise, Furre with with name unknown and the given Furre Id
        /// <para/>
        /// Furre Id of -1 is Undefined
        /// </returns>
        public IFurre GetFurreByID(int FurreID)
        {
            foreach (var Furre in furreList)
            {
                if (Furre.FurreID == FurreID)
                    return Furre;
            }
            return new Furre(FurreID);
        }

        /// <summary>
        /// Gets a Furre object by the Furre's Name
        /// </summary>
        /// <param name="sname">The sname.</param>
        /// <returns>
        /// Furre name with a real Furcadia ID if the furre is in the dream
        /// Other wise, Furre with Furre Id 0
        /// <para/>
        /// Furre Id of -1 is Undefined
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public IFurre GetFurreByName(string sname)
        {
            if (string.IsNullOrEmpty(sname))
                throw new ArgumentNullException(sname);
            foreach (IFurre Character in furreList)
            {
                if (Character.ShortName == sname.ToFurcadiaShortName())
                {
                    return Character;
                }
            }
            return new Furre(0, sname);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="Furre">The furre.</param>
        /// <returns></returns>
        public int IndexOf(IFurre Furre)
        {
            return furreList.IndexOf(Furre);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        public void Insert(int index, IFurre item)
        {
            furreList.Insert(index, item);
        }

        /// <summary>
        /// Removes a Furre based on their Furre ID
        /// </summary>
        /// <param name="FurreID">
        /// </param>
        public void Remove(int FurreID)
        {
            lock (RemoveLock)
            {
                IFurre fur = null;
                foreach (var Fur in furreList)
                {
                    if (Fur.FurreID == FurreID)
                    {
                        fur = Fur;
                        break;
                    }
                }
                if (fur != null)
                    furreList.Remove(fur);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public bool Remove(IFurre item)
        {
            foreach (var Furre in furreList)
            {
                if (Furre.FurreID == item.FurreID)
                {
                    furreList.Remove(Furre);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            furreList.RemoveAt(index);
        }

        #endregion Public Methods
    }
}