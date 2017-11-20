using Furcadia.Net.Dream;

namespace Furcadia.Net.Pounce
{
    /// <summary>
    /// Pounce info for Furre online status
    /// </summary>
    public class PounceFurre : IFurre
    {
        #region "Public Fields"

        private bool online;

        private bool wasOnline;

        /// <summary>
        /// Furre Name
        /// </summary>
        public string Name { get; set; }

        public string ShortName { get { return Name.ToFurcadiaShortName(); } }
        public int FurreID { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// Furre Currently online
        /// </summary>
        public bool Online
        {
            get { return online; }
            set { online = value; }
        }

        /// <summary>
        /// Furre Previous Online State
        /// </summary>
        public bool WasOnline
        {
            get { return wasOnline; }
            set { wasOnline = value; }
        }

        #endregion "Public Fields"
    }
}