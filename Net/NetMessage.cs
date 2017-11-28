/*Log Header
 *Format: (date,Version) AuthorName, Changes.
 * (Oct 27,2009,0.1) Squizzle, Initial Developer.
 *
*/

using Furcadia.Net.Utils;
using System.Text;

namespace Furcadia.Net
{
    /// <summary>
    /// Default.
    /// </summary>
    public class NetMessage : INetMessage
    {
        #region Private Fields

        private StringBuilder _data;

        #endregion Private Fields

        #region Public Constructors

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'NetMessage.NetMessage()'

        public NetMessage()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'NetMessage.NetMessage()'
        {
            _data = new StringBuilder();
        }

        #endregion Public Constructors

        #region Public Methods

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'NetMessage.GetString()'

        public string GetString()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'NetMessage.GetString()'
        {
            return _data.ToString();
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'NetMessage.Write(string)'

        public void Write(string data)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'NetMessage.Write(string)'
        {
            _data.Append(data);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'NetMessage.Write(byte[])'

        public void Write(byte[] data)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'NetMessage.Write(byte[])'
        {
            _data.Append(Encoding.GetEncoding(Utilities.GetEncoding).GetString(data));
        }

        #endregion Public Methods
    }
}