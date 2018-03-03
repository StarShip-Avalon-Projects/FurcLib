/*Log Header
 *Format: (date,Version) AuthorName, Changes.
 * (Oct 27,2009,0.1) Squizzle, Initial Developer.
 *
*/

namespace Furcadia.Net
{
    /// <summary>
    ///
    /// </summary>
    public interface INetMessage
    {
        #region Public Methods

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <returns></returns>
        string GetString();

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        void Write(string data);

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        void Write(byte[] data);

        #endregion Public Methods
    }
}