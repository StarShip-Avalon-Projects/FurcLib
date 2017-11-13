using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Furcadia.Net.Web
{
    /// <summary>
    ///General Utility functions for working with web objects
    /// </summary>
    public static class WebUtils
    {
        public static string PrepWebData(List<IVariable> list)
        {
            List<string> vars = new List<string>();
            foreach (var var in list)
                vars.Add(string.Format("{0}={1}"
                    , HttpUtility.UrlEncode(var.Name)
                    , HttpUtility.UrlEncode(var.Value.ToString())
                    ));
        }
    }
}