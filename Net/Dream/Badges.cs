/* Author: Gerolkae
 *
 * Uses: Displaying proper badge in Look responce  and whisper
 * Logging text equivelant "[Group-Level]" IE: [Bugge-Elder]
 *
*/

using System;
using System.Data;
using System.IO;
using System.Reflection;

namespace Furcadia.Net.Dream
{
    /// <summary>
    /// Beekin Badges
    /// </summary>
    public class Badges
    {
        #region Private Fields

        private static DataTable dt = null;

        #endregion Private Fields

        #region Private Enums

        /// <summary>
        /// Beekin Group
        /// </summary>
        private enum Group
        {
            none,
            Guardian = 1,
            Channel,
            Mason,
            Scribe,
            Bugge,
            Pixel,
            Welcomer,
            Helper,
            Teacher,
            Mentor,
            DEP,
            Irregular,
            Eventer
        };

        /// <summary>
        /// Group Rank
        /// </summary>
        private enum Level
        {
            none,
            Trainee = 1,
            Member,
            Elder,
            Head,
            AIC,
            Associate,
            Creator
        };

        #endregion Private Enums

        #region Public Methods

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Badges.GetGroup(string)'

        public static int GetGroup(string tag)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Badges.GetGroup(string)'
        {
            int result = 0;
            if (tag == "")
                return result;
            if (dt == null)
                PrimeTable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                object test = dt.Rows[i]["Hash"];
                if (Convert.IsDBNull(test) == false)
                    if (tag == dt.Rows[i]["Hash"].ToString())
                    {
                        int.TryParse(dt.Rows[i]["Group"].ToString(), out result);
                        return result;
                    }
                    else
                        return result;
            }
            return result;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Badges.GetLevel(string)'

        public static int GetLevel(string tag)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Badges.GetLevel(string)'
        {
            int result = 0;
            if (tag == "")
                return result;
            if (dt == null)
                PrimeTable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                object test = dt.Rows[i]["Hash"];
                if (Convert.IsDBNull(test) == false)
                    if (tag == dt.Rows[i]["Hash"].ToString())
                    {
                        int.TryParse(dt.Rows[i]["Level"].ToString(), out result);
                        return result;
                    }
                    else
                        return result;
            }
            return result;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Badges.GetTag(string)'

        public static string GetTag(string tag)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Badges.GetTag(string)'
        {
            string result = null;
            if (tag == "")
                return result;
            if (dt == null)
                PrimeTable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                object test = dt.Rows[i]["Hash"];
                if (Convert.IsDBNull(test) == false)
                    if (tag == (string)test)
                    {
                        int lvl = 0;
                        int grp = 0;
                        int.TryParse(dt.Rows[i]["Level"].ToString(), out lvl);
                        int.TryParse(dt.Rows[i]["Group"].ToString(), out grp);
                        Group g = NumToEnum<Group>(grp);
                        Level l = NumToEnum<Level>(lvl);
                        result = "[" + g.ToString() + "-" + l.ToString() + "]";
                        break;
                    }
            }
            return result;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="number">
        /// </param>
        /// <returns>
        /// </returns>
        public static T NumToEnum<T>(int number)
        {
            return (T)Enum.ToObject(typeof(T), number);
        }

        /// <summary>
        /// Preload the badge table
        /// </summary>
        public static void PrimeTable()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Furcadia.Resources.badges.csv");
            try
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    result = result.Replace("\"", "");
                    string[] csvRows = result.Split('\n', '\r');
                    string[] fields = null;
                    dt = new DataTable();

                    foreach (string Field in csvRows[0].Split(new char[] { ',' }))
                        dt.Columns.Add(new DataColumn(Field, typeof(string)));
                    for (int i = 1; i < csvRows.Length; i++)
                    {
                        fields = csvRows[i].Split(',');
                        DataRow row = dt.NewRow();
                        row.ItemArray = fields;
                        dt.Rows.Add(row);
                    }
                }
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }

        #endregion Public Methods
    }
}