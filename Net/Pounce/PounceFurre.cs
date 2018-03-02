using Furcadia.Net.DreamInfo;
using Furcadia.Text;

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

        /// <summary>
        /// implements the Furre;s Name Property
        /// </summary>
        public string ShortName { get { return Name.ToFurcadiaShortName(); } }

        /// <summary>
        /// Implements the FurreID or unique furre identifyer
        /// </summary>
        public Base220 FurreID { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
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