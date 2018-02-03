using System.Collections.Generic;
using System.Web;

namespace Furcadia.Net.Web
{
    /// <summary>
    ///General Utility functions for working with web objects
    /// </summary>
    public static class WebUtils
    {
        /// <summary>
        /// Preps the web data.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        // TODO: Convert to Monkeyspeak.IVariable
        // Filter out the % prefix
        // use Variable Table
        public static string PrepWebData(List<IVariable> list)
        {
            List<string> vars = new List<string>();
            foreach (var var in list)
            {
                if (var.Value != null)
                    vars.Add($"{HttpUtility.UrlEncode(var.Name)}={HttpUtility.UrlEncode(var.Value.ToString())}");
            }
            return string.Join("&", vars.ToArray());
        }
    }
}