using Furcadia.Text;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Furcadia.Net.Dream
{
    /// <summary>
    /// Furre List information for a Furcadia Dream
    /// <para>
    /// This class acts like an enhanced List(of &lt;T&gt;) because you can
    /// Select a Furre by Item as well as index
    /// </para>
    /// </summary>
    public class FurreList : IList<Furre>, ICollection
    {
        #region Protected Internal Fields

        /// <summary>
        /// </summary>
        static protected internal IList<Furre> fList;

        #endregion Protected Internal Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        public FurreList()
        {
            fList = new List<Furre>(100);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Number of Avatars in the Dream
        /// </summary>
        public int Count
        {
            get
            {
                return fList.Count;
            }
        }

        /// <summary>
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return fList.IsReadOnly;
            }
        }

        /// <summary>
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return ((ICollection)fList).IsSynchronized;
            }
        }

        /// <summary>
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return ((ICollection)fList).SyncRoot;
            }
        }

        /// <summary>
        /// Convert Furre List to <see cref=" IList"/>
        /// </summary>
        public IList<Furre> ToIList
        {
            get
            {
                return fList;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="index">
        /// </param>
        /// <returns>
        /// </returns>
        public Furre this[int index]
        {
            get
            {
                if (index < fList.Count)
                    return fList[index];
                else
                    return null;
            }
            set
            {
                if (index < fList.Count)
                    fList[index] = value;
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
        public Furre this[Furre fur]
        {
            get
            {
                if (fList.Contains(fur))
                    return fList[fList.IndexOf(fur)];
                return fur;
            }
            set
            {
                if (fList.Contains(fur))
                    fList[fList.IndexOf(fur)] = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        private object RemoveLock = new object();

        /// <summary>
        /// </summary>
        /// <param name="Furre">
        /// </param>
        public void Add(Furre Furre)
        {
            if (!fList.Contains(Furre))
                fList.Add(Furre);
            else
                fList[fList.IndexOf(Furre)] = Furre;
        }

        /// <summary>
        /// </summary>
        public void Clear()
        {
            fList.Clear();
        }

        /// <summary>
        /// </summary>
        /// <param name="FurreID">
        /// </param>
        /// <returns>
        /// </returns>
        public bool Contains(int FurreID)
        {
            foreach (Furre fur in fList)
            {
                if (fur.FurreID == FurreID)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="Furre">
        /// </param>
        /// <returns>
        /// </returns>
        public bool Contains(Furre Furre)
        {
            foreach (Furre fur in fList)
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
            ((ICollection)fList).CopyTo(array, index);
        }

        /// <summary>
        /// </summary>
        /// <param name="sname">
        /// </param>
        /// <returns>
        /// </returns>
        public Furre GerFurreByName(string sname)
        {
            foreach (Furre Character in fList)
            {
                if (Character.ShortName == Util.FurcadiaShortName(sname))
                {
                    return Character;
                }
            }
            return new Furre(sname);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return ((ICollection)fList).GetEnumerator();
        }

        /// <summary>
        /// Get's a Furre from the Dream List bu it's ID
        /// </summary>
        /// <param name="FurreID">
        /// Base220 4 byte string representing the Furre ID
        /// </param>
        /// <returns>
        /// </returns>
        public Furre GetFurreByID(string FurreID)
        {
            foreach (Furre Furre in fList)
            {
                if (Furre.FurreID == Base220.ConvertFromBase220(FurreID))
                    return Furre;
            }
            return new Furre(Base220.ConvertFromBase220(FurreID));
        }

        /// <summary>
        /// get a Furre from the Furrelist by its integer idd
        /// </summary>
        /// <param name="FurreID">
        /// Furre ID as integer
        /// </param>
        /// <returns>
        /// Furre
        /// </returns>
        public Furre GetFurreByID(int FurreID)
        {
            foreach (Furre Furre in fList)
            {
                if (Furre.FurreID == FurreID)
                    return Furre;
            }
            return new Furre(FurreID);
        }

        /// <summary>
        /// </summary>
        /// <param name="Furre">
        /// </param>
        /// <returns>
        /// </returns>
        public int IndexOf(Furre Furre)
        {
            return fList.IndexOf(Furre);
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
                Furre F = null;
                foreach (Furre Fur in fList)
                {
                    if (Fur.FurreID == FurreID)
                    {
                        F = Fur;
                        break;
                    }
                }
                if (F != null)
                    fList.Remove(F);
            }
        }

        #endregion Public Methods

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// </summary>
        /// <param name="array">
        /// </param>
        /// <param name="arrayIndex">
        /// </param>
        public void CopyTo(Furre[] array, int arrayIndex)
        {
            fList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// </summary>
        /// <param name="index">
        /// </param>
        /// <param name="item">
        /// </param>
        public void Insert(int index, Furre item)
        {
            fList.Insert(index, item);
        }

        /// <summary>
        /// </summary>
        /// <param name="item">
        /// </param>
        /// <returns>
        /// </returns>
        public bool Remove(Furre item)
        {
            foreach (Furre Furre in fList)
            {
                if (Furre.FurreID == item.FurreID)
                {
                    fList.Remove(Furre);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="index">
        /// </param>
        public void RemoveAt(int index)
        {
            fList.RemoveAt(index);
        }


        IEnumerator<Furre> IEnumerable<Furre>.GetEnumerator()
        {
            return fList.GetEnumerator();
        }

 

        #endregion IDisposable Support
    }
}