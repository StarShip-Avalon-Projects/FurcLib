using System;
using System.Data;
using System.IO;
using System.Reflection;

namespace Furcadia.Net.Dream
{
    /// <summary>
    /// Legacy Furre Avatar information
    /// </summary>
    [Legacy]
    public class Avatar

    {
        #region Private Fields

        private static DataTable dt = null;

        #endregion Private Fields

        #region Private Enums

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_DIR'

        public enum av_DIR
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_DIR'
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_DIR.SW'
            SW,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_DIR.SW'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_DIR.SE'
            SE,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_DIR.SE'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_DIR.NW'
            NW,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_DIR.NW'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_DIR.NE'
            NE,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_DIR.NE'
        };

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_POSE'

        public enum av_POSE
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_POSE'
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_POSE.AVPOSE_SIT'
            AVPOSE_SIT,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_POSE.AVPOSE_SIT'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_POSE.AVPOSE_WALK0'
            AVPOSE_WALK0,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_POSE.AVPOSE_WALK0'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_POSE.AVPOSE_STAND'
            AVPOSE_STAND,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_POSE.AVPOSE_STAND'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_POSE.AVPOSE_WALK1'
            AVPOSE_WALK1,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_POSE.AVPOSE_WALK1'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_POSE.AVPOSE_LIE'
            AVPOSE_LIE,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.av_POSE.AVPOSE_LIE'
        };

        /// <summary>
        /// </summary>
        [Flags]
        public enum FurrePose
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.FurrePose.None'
            None,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.FurrePose.None'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.FurrePose.Walk1'
            Walk1,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.FurrePose.Walk1'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.FurrePose.Stand1'
            Stand1,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.FurrePose.Stand1'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.FurrePose.Walk2'
            Walk2,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.FurrePose.Walk2'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.FurrePose.Stand2'
            Stand2,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.FurrePose.Stand2'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.FurrePose.Sit'
            Sit,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.FurrePose.Sit'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.FurrePose.LieDown'
            LieDown
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.FurrePose.LieDown'
        }

        #endregion Private Enums

        #region Public Methods

        /// <summary>
        /// Primes the table.
        /// </summary>
        public static void PrimeTable()
        {
            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Furcadia.Resources.AvatarFrames.csv");
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    result = result.Replace("\r", "");
                    result = result.Replace("\"", "");
                    string[] csvRows = result.Split('\n');
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
            catch
            {
            }
        }

        /// <summary>
        /// Specs the number.
        /// </summary>
        /// <param name="Frame">
        /// The frame.
        /// </param>
        /// <returns>
        /// </returns>
        public static Frame SpecNum(int Frame)
        {
            Frame _Frame = new Frame();
            int Species = Frame - (Frame % 20);
            if (dt == null)
                PrimeTable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                object test = dt.Rows[i]["FrameIndex"];
                if (Convert.IsDBNull(test) == false)
                    // "Species","FrameIndex","Gender","DS"
                    if (Species == Convert.ToInt32(dt.Rows[i]["FrameIndex"]))
                    {
                        _Frame.Spec = Species;
                        int.TryParse(dt.Rows[i]["DS"].ToString(), out _Frame.DS_Number);
                        int.TryParse(dt.Rows[i]["Gender"].ToString(), out _Frame.gender);
                        int.TryParse(dt.Rows[i]["Wings"].ToString(), out _Frame.Wings);
                        switch (Frame % 20)
                        {
                            case 0:
                                _Frame.dir = (int)av_DIR.SW;
                                _Frame.pose = (int)av_POSE.AVPOSE_SIT;
                                return _Frame;

                            case 1:
                                _Frame.dir = (int)av_DIR.SW;
                                _Frame.pose = (int)av_POSE.AVPOSE_WALK0;
                                return _Frame;

                            case 2:
                                _Frame.dir = (int)av_DIR.SW;
                                _Frame.pose = (int)av_POSE.AVPOSE_STAND;
                                return _Frame;

                            case 3:
                                _Frame.dir = (int)av_DIR.SW;
                                _Frame.pose = (int)av_POSE.AVPOSE_WALK1;
                                return _Frame;

                            case 4:
                                _Frame.dir = (int)av_DIR.SE;
                                _Frame.pose = (int)av_POSE.AVPOSE_SIT;
                                return _Frame;

                            case 5:
                                _Frame.dir = (int)av_DIR.SE;
                                _Frame.pose = (int)av_POSE.AVPOSE_WALK0;
                                return _Frame;

                            case 6:
                                _Frame.dir = (int)av_DIR.SE;
                                _Frame.pose = (int)av_POSE.AVPOSE_STAND;
                                return _Frame;

                            case 7:
                                _Frame.dir = (int)av_DIR.SE;
                                _Frame.pose = (int)av_POSE.AVPOSE_WALK1;
                                return _Frame;

                            case 8:
                                _Frame.dir = (int)av_DIR.NW;
                                _Frame.pose = (int)av_POSE.AVPOSE_SIT;
                                return _Frame;

                            case 9:
                                _Frame.dir = (int)av_DIR.NW;
                                _Frame.pose = (int)av_POSE.AVPOSE_WALK0;
                                return _Frame;

                            case 10:
                                _Frame.dir = (int)av_DIR.NW;
                                _Frame.pose = (int)av_POSE.AVPOSE_STAND;
                                return _Frame;

                            case 11:
                                _Frame.dir = (int)av_DIR.NW;
                                _Frame.pose = (int)av_POSE.AVPOSE_WALK1;
                                return _Frame;

                            case 12:
                                _Frame.dir = (int)av_DIR.NE;
                                _Frame.pose = (int)av_POSE.AVPOSE_SIT;
                                return _Frame;

                            case 13:
                                _Frame.dir = (int)av_DIR.NE;
                                _Frame.pose = (int)av_POSE.AVPOSE_WALK0;
                                return _Frame;

                            case 14:
                                _Frame.dir = (int)av_DIR.NE;
                                _Frame.pose = (int)av_POSE.AVPOSE_STAND;
                                return _Frame;

                            case 15:
                                _Frame.dir = (int)av_DIR.NE;
                                _Frame.pose = (int)av_POSE.AVPOSE_WALK1;
                                return _Frame;

                            case 16:
                                _Frame.dir = (int)av_DIR.NW;
                                _Frame.pose = (int)av_POSE.AVPOSE_LIE;
                                return _Frame;

                            case 17:
                                _Frame.dir = (int)av_DIR.NE;
                                _Frame.pose = (int)av_POSE.AVPOSE_LIE;
                                return _Frame;

                            case 18:
                                _Frame.dir = (int)av_DIR.SE;
                                _Frame.pose = (int)av_POSE.AVPOSE_LIE;
                                return _Frame;

                            case 19:
                                _Frame.dir = (int)av_DIR.SW;
                                _Frame.pose = (int)av_POSE.AVPOSE_LIE;
                                return _Frame;
                        }
                    }
                    else
                    {
                        _Frame.Clear();
                        return _Frame;
                    }
            }
            _Frame.Clear();
            return _Frame;
        }

        #endregion Public Methods

        #region Public Structs

        /// <summary>
        /// </summary>
        public struct Frame
        {
            #region Public Fields

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.Frame.dir'
            public int dir;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.Frame.dir'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.Frame.DS_Number'
            public int DS_Number;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.Frame.DS_Number'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.Frame.gender'
            public int gender;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.Frame.gender'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.Frame.pose'
            public int pose;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.Frame.pose'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.Frame.Spec'
            public int Spec;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.Frame.Spec'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.Frame.Wings'
            public int Wings;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.Frame.Wings'

            #endregion Public Fields

            #region Public Methods

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Avatar.Frame.Clear()'

            public void Clear()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Avatar.Frame.Clear()'
            {
                Spec = -1;
                dir = -1;
                pose = -1;
                gender = -1;
                DS_Number = -1;
                Wings = -1;
            }

            #endregion Public Methods
        }

        #endregion Public Structs

        //end class
    }
}