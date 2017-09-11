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
    public class FURREList : IList<FURRE>, ICollection, IDisposable
    {
        #region Protected Internal Fields

        /// <summary>
        /// </summary>
        static protected internal IList<FURRE> fList;

        #endregion Protected Internal Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        public FURREList()
        {
            fList = new List<FURRE>(100);
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
        public IList<FURRE> ToIList
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
        public FURRE this[int index]
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
        /// Gets or set the furre at index of fur
        /// </summary>
        /// <param name="fur">
        /// Furre
        /// </param>
        /// <returns>
        /// </returns>
        public FURRE this[FURRE fur]
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
        public void Add(FURRE Furre)
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
            foreach (FURRE fur in fList)
            {
                if (fur.ID == FurreID)
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
        public bool Contains(FURRE Furre)
        {
            foreach (FURRE fur in fList)
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
        public FURRE GerFurreByName(string sname)
        {
            foreach (FURRE Character in fList)
            {
                if (Character.ShortName == Util.FurcadiaShortName(sname))
                {
                    return Character;
                }
            }
            return new FURRE(sname);
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
        public FURRE GetFurreByID(string FurreID)
        {
            foreach (FURRE furre in fList)
            {
                if (furre.ID == Base220.ConvertFromBase220(FurreID))
                    return furre;
            }
            return new FURRE(Base220.ConvertFromBase220(FurreID));
        }

        /// <summary>
        /// get a furre from the furrelist by its integer idd
        /// </summary>
        /// <param name="FurreID">
        /// Furre ID as integer
        /// </param>
        /// <returns>
        /// Furre
        /// </returns>
        public FURRE GetFurreByID(int FurreID)
        {
            foreach (FURRE furre in fList)
            {
                if (furre.ID == FurreID)
                    return furre;
            }
            return new FURRE(FurreID);
        }

        /// <summary>
        /// </summary>
        /// <param name="Furre">
        /// </param>
        /// <returns>
        /// </returns>
        public int IndexOf(FURRE Furre)
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
                FURRE F = null;
                foreach (FURRE Fur in fList)
                {
                    if (Fur.ID == FurreID)
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
        public void CopyTo(FURRE[] array, int arrayIndex)
        {
            fList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// </summary>
        /// <param name="index">
        /// </param>
        /// <param name="item">
        /// </param>
        public void Insert(int index, FURRE item)
        {
            fList.Insert(index, item);
        }

        /// <summary>
        /// </summary>
        /// <param name="item">
        /// </param>
        /// <returns>
        /// </returns>
        public bool Remove(FURRE item)
        {
            foreach (FURRE furre in fList)
            {
                if (furre.ID == item.ID)
                {
                    fList.Remove(furre);
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

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool
            // disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is
            //       overridden above. GC.SuppressFinalize(this);
        }

        IEnumerator<FURRE> IEnumerable<FURRE>.GetEnumerator()
        {
            return fList.GetEnumerator();
        }

        /// <summary>
        /// </summary>
        /// <param name="disposing">
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and
                //       override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        #endregion IDisposable Support
    }
}