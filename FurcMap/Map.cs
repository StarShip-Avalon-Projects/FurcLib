using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Furcadia.FurcMap
{
    /// <summary>
    ///
    /// </summary>
    public static class MapRating
    {
        #region Public Fields

        /// <summary>
        ///
        /// </summary>
        public const String Adult = "Adult 18+";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MapRating.AdultOnly'
        public const String AdultOnly = "Adults Only";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapRating.AdultOnly'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MapRating.AOClean'
        public const String AOClean = "AOClean";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapRating.AOClean'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MapRating.Everyone'
        public const String Everyone = "Everyone";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapRating.Everyone'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MapRating.Mature'
        public const String Mature = "Mature 16+";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapRating.Mature'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MapRating.Teen'
        public const String Teen = "Teen+";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapRating.Teen'

        #endregion Public Fields
    }

    /// <summary>
    ///
    /// </summary>
    public class Map
    {
        #region Private Fields

        private bool allowjs, allowlf, allowfurl, nowho, forcesittable, allowshouts, allowlarge, notab, nonovelty, swearfilter, parentalcontrols, encoded;
        private List<String> headerLines = new List<String>();
        private Dictionary<String, String> mapData = new Dictionary<String, String>();

        private byte[] mapMatrix, floors, objects, walls, regions, effects;
        private String name, patchs, rating;
        private int width, height, revision, patcht;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Creates a new empty map with the specified width and height
        /// </summary>
        /// <param name="width">
        /// The width of the map
        /// </param>
        /// <param name="height">
        /// The height of the map
        /// </param>
        public Map(int width, int height)
        {
            this.width = width / 2;
            this.height = height;

            floors = new byte[this.bytesLayerCount];
            objects = new byte[this.bytesLayerCount];
            walls = new byte[this.bytesLayerCount];
            regions = new byte[this.bytesLayerCount];
            effects = new byte[this.bytesLayerCount];

            String mapData = String.Format(Properties.Resources.DefaultMapData, this.height, this.width, "");
            String[] mapLines = mapData.Split(new String[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (String line in mapLines)
            {
                this.headerLines.Add(line);
                if (line.Contains("="))
                {
                    String[] vals = line.Split(new char[] { '=' }, 2);
                    this.mapData.Add(vals[0], vals[1]);
                }
                else if (line == "BODY")
                    break;
            }

            SetMapHeaders(this.mapData);

            byte[] mapMatrix = new byte[this.bytesLayerCount * 5];
            this.mapMatrix = mapMatrix;
        }

        #endregion Public Constructors

        #region Internal Constructors

        internal Map()
        {
        }

        #endregion Internal Constructors

        #region Internal Properties

        internal int bytesLayerCount
        {
            get
            {
                return ((width * 2) * height);
            }
        }

        #endregion Internal Properties

        #region Public Variables

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.AllowDreamURL'

        public bool AllowDreamURL
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.AllowDreamURL'
        {
            get { return this.allowfurl; }
            set { this.allowfurl = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.AllowJoinSummon'

        public bool AllowJoinSummon
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.AllowJoinSummon'
        {
            get { return this.allowjs; }
            set { this.allowjs = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.AllowLargeDreamSize'

        public bool AllowLargeDreamSize
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.AllowLargeDreamSize'
        {
            get { return this.allowlarge; }
            set { this.allowlarge = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.AllowLeadFollow'

        public bool AllowLeadFollow
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.AllowLeadFollow'
        {
            get { return this.allowlf; }
            set { this.allowlf = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.AllowShouting'

        public bool AllowShouting
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.AllowShouting'
        {
            get { return this.allowshouts; }
            set { this.allowshouts = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.EncodeDream'

        public bool EncodeDream
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.EncodeDream'
        {
            get { return this.encoded; }
            set { this.encoded = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.EnforceParentalControls'

        public bool EnforceParentalControls
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.EnforceParentalControls'
        {
            get { return this.parentalcontrols; }
            set { this.parentalcontrols = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.ForceSitting'

        public bool ForceSitting
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.ForceSitting'
        {
            get { return this.forcesittable; }
            set { this.forcesittable = value; }
        }

        /// <summary>
        /// The actual height of the map (READ-ONLY)
        /// </summary>
        public int Height
        {
            get { return this.height; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.Name'

        public String Name
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.Name'
        {
            get { return this.name; }
            set { this.name = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.PatchArchive'

        public String PatchArchive
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.PatchArchive'
        {
            get { return this.patchs; }
            set { this.patchs = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.PreventPlayerListing'

        public bool PreventPlayerListing
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.PreventPlayerListing'
        {
            get { return this.nowho; }
            set { this.nowho = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.PreventSeasonalAvatars'

        public bool PreventSeasonalAvatars
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.PreventSeasonalAvatars'
        {
            get { return this.nonovelty; }
            set { this.nonovelty = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.PreventTabListing'

        public bool PreventTabListing
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.PreventTabListing'
        {
            get { return this.notab; }
            set { this.notab = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.Rating'

        public String Rating
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.Rating'
        {
            get { return this.rating; }
            set { this.rating = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.Revision'

        public int Revision
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.Revision'
        {
            get { return this.revision; }
            set { this.revision = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.UsePatch'

        public int UsePatch
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.UsePatch'
        {
            get { return this.patcht; }
            set { this.patcht = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Map.UseSwearFilter'

        public bool UseSwearFilter
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Map.UseSwearFilter'
        {
            get { return this.swearfilter; }
            set { this.swearfilter = value; }
        }

        /// <summary>
        /// The actual width of the map (READ-ONLY)
        /// </summary>
        public int Width
        {
            get { return this.width * 2; }
        }

        #endregion Public Variables

        #region Public Methods

        /// <summary>
        /// Loads a map from a file
        /// </summary>
        /// <param name="filename">
        /// The file to load the map from
        /// </param>
        /// <exception cref="InvalidDataException">
        /// Thrown if the width and height of the map is not known (corrupt file)
        /// </exception>
        /// <returns>
        /// The map
        /// </returns>
        public static Map LoadFrom(String filename)
        {
            Map m = new Map();

            FileStream fs = new FileStream(filename, FileMode.Open);
            BinaryReader br = new BinaryReader(fs, Encoding.GetEncoding(1252));

            String currentLine = "" + br.ReadChar();
            while (true)
            {
                currentLine += br.ReadChar();

                if (currentLine.EndsWith("\n"))
                {
                    m.headerLines.Add(currentLine.Replace("\n", ""));
                    if (currentLine.Contains("="))
                    {
                        String[] vals = currentLine.Split(new char[] { '=' }, 2);

                        if (!m.mapData.ContainsKey(vals[0]))
                            m.mapData.Add(vals[0], vals[1]);
                        else
                            m.mapData[vals[0]] = vals[1];
                    }
                    else if (currentLine == "BODY\n")
                        break;

                    currentLine = "";
                }
            }

            if (m.mapData.ContainsKey("width") && m.mapData.ContainsKey("height"))
            {
                m.width = int.Parse(m.mapData["width"]);
                m.height = int.Parse(m.mapData["height"]);
            }
            else
            {
                throw new InvalidDataException("Unable to determine width & height of the map");
            }

            m.SetMapHeaders(m.mapData);

            m.floors = new byte[m.bytesLayerCount];
            m.objects = new byte[m.bytesLayerCount];
            m.walls = new byte[m.bytesLayerCount];
            m.regions = new byte[m.bytesLayerCount];
            m.effects = new byte[m.bytesLayerCount];

            List<byte> mapMatrix = new List<byte>();
            int read = 0;
            byte[] buffer = new byte[1024];
            do
            {
                read = br.Read(buffer, 0, buffer.Length);
                for (int i = 0; i < read; i++)
                    mapMatrix.Add(buffer[i]);
            } while (read > 0);

            br.Close();
            fs.Close();

            m.ParseMatrix(mapMatrix.ToArray());

            return m;
        }

        /// <summary>
        /// Get the effect number from a tile
        /// </summary>
        /// <param name="x">
        /// x coordinate
        /// </param> <param name="y">
        /// Y coordinate
        /// </param>
        /// <returns>
        /// The effect number
        /// </returns>
        public int getEffectAt(int x, int y)
        {
            int pos = getPosFrom(x, y);

            return (int)effects[pos];
        }

        /// <summary>
        /// Get the floor number from a tile
        /// </summary>
        /// <param name="x">
        /// x coordinate
        /// </param> <param name="y">
        /// Y coordinate
        /// </param>
        /// <returns>
        /// The floor number
        /// </returns>
        public int getFloorAt(int x, int y)
        {
            int pos = getPosFrom(x, y);

            return (int)floors[pos];
        }

#pragma warning disable CS1570 // XML comment has badly formed XML -- 'Whitespace is not allowed at this location.'

        /// <summary> Get a MapPosition object from the position specified
        /// by x & y </summary> <param name="x"></param> <param
        /// name="y"></param> <returns></returns>
        public MapPosition getMapPos(int x, int y)
#pragma warning restore CS1570 // XML comment has badly formed XML -- 'Whitespace is not allowed at this location.'
        {
            return new MapPosition(x, y, this);
        }

        /// <summary>
        /// Get a MapTile object from the position specified by x
        /// &amp; y
        /// </summary>
        /// <param name="x">
        /// x coordinate
        /// </param> <param name="y">
        /// Y coordinate
        /// </param>
        /// <returns>the Tile data</returns>
        public MapTile getMapTile(int x, int y)
        {
            return new MapTile(x, y, this);
        }

        /// <summary>
        /// Get the object number from a tile
        /// </summary>
        /// <param name="x">
        /// x coordinate
        /// </param> <param name="y">
        /// Y coordinate
        /// </param>
        /// <returns>
        /// The object number
        /// </returns>
        public int getObjectAt(int x, int y)
        {
            int pos = getPosFrom(x, y);

            return (int)objects[pos];
        }

        /// <summary>
        /// Get the region number from a tile
        /// </summary>
        /// <param name="x">
        /// x coordinate
        /// </param> <param name="y">
        /// Y coordinate
        /// </param>
        /// <returns>
        /// The region number
        /// </returns>
        public int getRegionAt(int x, int y)
        {
            int pos = getPosFrom(x, y);

            return (int)regions[pos];
        }

        /// <summary>
        /// Get the wall number from a tile
        /// </summary>
        /// <param name="x">
        /// x coordinate
        /// </param> <param name="y">
        /// Y coordinate
        /// </param>
        /// <returns>
        /// The wall number
        /// </returns>
        public int getWallAt(int x, int y)
        {
            int pos = (this.height * x + y);

            return (int)walls[pos];
        }

        /// <summary>
        /// Save the map to a file
        /// </summary>
        /// <param name="filename">
        /// The filename to save to
        /// </param>
        /// <param name="overwrite">
        /// If a file with that name already exist, should we overwrite it?
        /// </param>
        /// <returns>
        /// True if the save was a success, False if not
        /// </returns>
        public bool Save(String filename, bool overwrite = true)
        {
            if (File.Exists(filename) && !overwrite)
                return false;

            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryWriter sw = new BinaryWriter(fs, Encoding.GetEncoding(1252));

            String headerData = "MAP V01.40 Furcadia\n";
            headerData += "height=" + this.height + "\n";
            headerData += "width=" + this.width + "\n";
            headerData += "revision=" + this.revision + "\n";
            headerData += "patcht=" + this.patcht + "\n";
            headerData += "name=" + this.name + "\n";
            headerData += "patchs=" + this.patchs + "\n";
            headerData += "encoded=" + (this.encoded ? "1" : "0") + "\n";
            headerData += "allowjs=" + (this.allowjs ? "1" : "0") + "\n";
            headerData += "allowlf=" + (this.allowlf ? "1" : "0") + "\n";
            headerData += "allowfurl=" + (this.allowfurl ? "1" : "0") + "\n";
            headerData += "swearfilter=" + (this.swearfilter ? "1" : "0") + "\n";
            headerData += "nowho=" + (this.nowho ? "1" : "0") + "\n";
            headerData += "forcesittable=" + (this.forcesittable ? "1" : "0") + "\n";
            headerData += "allowshouts=" + (this.allowshouts ? "1" : "0") + "\n";
            headerData += "rating=" + this.rating + "\n";
            headerData += "allowlarge=" + (this.allowlarge ? "1" : "0") + "\n";
            headerData += "notab=" + (this.notab ? "1" : "0") + "\n";
            headerData += "nonovelty=" + (this.nonovelty ? "1" : "0") + "\n";
            headerData += "parentalcontrols=" + (this.parentalcontrols ? "1" : "0") + "\n";
            headerData += "BODY\n";

            byte[] headerDataBytes = Encoding.GetEncoding(1252).GetBytes(headerData);

            sw.Write(headerDataBytes);
            sw.Write(this.floors);
            sw.Write(this.objects);
            sw.Write(this.walls);
            sw.Write(this.regions);
            sw.Write(this.effects);

            sw.Close();
            fs.Close();

            return true;
        }

        /// <summary> Set the effect number at a tile specified by x &amp; y
        /// </summary>
        /// <param name="x">
        /// x coordinate
        /// </param> <param name="y">
        /// Y coordinate
        /// </param>
        /// <param name="effectNumber"></param>
        public void setEffectAt(int x, int y, int effectNumber)
        {
            int pos = getPosFrom(x, y);

            effects[pos] = (byte)effectNumber;
        }

        /// <summary>
        /// Set the floor number at a tile specified by x and y
        /// </summary>
        /// <param name="x">
        /// x coordinate
        /// </param> <param name="y">
        /// Y coordinate
        /// </param>
        /// <param name="floorNumber">
        /// </param>
        public void setFloorAt(int x, int y, int floorNumber)
        {
            int pos = getPosFrom(x, y);

            floors[pos] = (byte)floorNumber;
        }

#pragma warning disable CS1570 // XML comment has badly formed XML -- 'Whitespace is not allowed at this location.'

        /// <summary> Set the object number at a tile specified by x & y
        /// </summary>
        /// <param name="x">
        /// x coordinate
        /// </param> <param name="y">
        /// Y coordinate
        /// </param>
        /// <param name="objectNumber"></param>
        public void setObjectAt(int x, int y, int objectNumber)
#pragma warning restore CS1570 // XML comment has badly formed XML -- 'Whitespace is not allowed at this location.'
        {
            int pos = getPosFrom(x, y);

            objects[pos] = (byte)objectNumber;
        }

#pragma warning disable CS1570 // XML comment has badly formed XML -- 'Expected an end tag for element 'summary'.'

        /// <summary> Set the region number at a tile specified by x &amp; y
        /// <param name="x">
        /// x coordinate
        /// </param> <param name="y">
        /// Y coordinate
        /// </param>
        /// <paramref name="regionNumber"/>
        public void setRegionAt(int x, int y, int regionNumber)
#pragma warning restore CS1570 // XML comment has badly formed XML -- 'Expected an end tag for element 'summary'.'
        {
            int pos = getPosFrom(x, y);

            regions[pos] = (byte)regionNumber;
        }

#pragma warning disable CS1570 // XML comment has badly formed XML -- 'End tag was not expected at this location.'
        /// <summary> Set the wall number at a tile specified by x &amp; y
        /// </summary>
        /// <param name="x">
        /// x coordinate
        /// </param> <param name="y">
        /// Y coordinate
        /// </param>
        /// </param><param name="wallNumber"></param>

        public void setWallAt(int x, int y, int wallNumber)
#pragma warning restore CS1570 // XML comment has badly formed XML -- 'End tag was not expected at this location.'
        {
            int pos = (this.height * x + y);

            walls[pos] = (byte)wallNumber;
        }

        #endregion Public Methods

        #region Private Methods

        private int getPosFrom(int x, int y)
        {
            return ((this.height * (x / 2) + y) * 2);
        }

        private bool ParseMatrix(byte[] matrix)
        {
            if (matrix.Length != this.bytesLayerCount * 5)
            {
                Console.WriteLine("Something is wrong here...");
                return false;
            }
            this.mapMatrix = matrix;

            for (int i = 0; i < this.bytesLayerCount; i++)
                floors[i] = matrix[i];

            for (int i = 0; i < this.bytesLayerCount; i++)
            {
                objects[i] = matrix[i + this.bytesLayerCount];
            }

            for (int i = 0; i < this.bytesLayerCount; i++)
            {
                walls[i] = matrix[i + (this.bytesLayerCount * 2)];
            }

            for (int i = 0; i < this.bytesLayerCount; i++)
            {
                regions[i] = matrix[i + (this.bytesLayerCount * 3)];
            }

            for (int i = 0; i < this.bytesLayerCount; i++)
            {
                effects[i] = matrix[i + (this.bytesLayerCount * 4)];
            }

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Values"></param>
        private void SetMapHeaders(Dictionary<String, String> Values)
        {
            if (Values.ContainsKey("height"))
                this.width = int.Parse(Values["height"]);

            if (Values.ContainsKey("width"))
                this.width = int.Parse(Values["width"]);

            if (Values.ContainsKey("revision"))
                this.revision = int.Parse(Values["revision"]);

            if (Values.ContainsKey("patcht"))
                this.patcht = int.Parse(Values["patcht"]);

            if (Values.ContainsKey("name"))
                this.name = Values["name"];

            if (Values.ContainsKey("patchs"))
                this.patchs = Values["patchs"];

            if (Values.ContainsKey("rating"))
                this.rating = Values["rating"];

            if (Values.ContainsKey("allowjs"))
                this.allowjs = Values["allowjs"] == "1";

            if (Values.ContainsKey("allowlf"))
                this.allowlf = Values["allowlf"] == "1";

            if (Values.ContainsKey("allowfurl"))
                this.allowfurl = Values["allowfurl"] == "1";

            if (Values.ContainsKey("swearfilter"))
                this.swearfilter = Values["swearfilter"] == "1";

            if (Values.ContainsKey("nowho"))
                this.nowho = Values["nowho"] == "1";

            if (Values.ContainsKey("forcesittable"))
                this.forcesittable = Values["forcesittable"] == "1";

            if (Values.ContainsKey("allowlarge"))
                this.allowlarge = Values["allowlarge"] == "1";

            if (Values.ContainsKey("allowshouts"))
                this.allowshouts = Values["allowshouts"] == "1";

            if (Values.ContainsKey("notab"))
                this.notab = Values["notab"] == "1";

            if (Values.ContainsKey("nonovelty"))
                this.nonovelty = Values["nonovelty"] == "1";

            if (Values.ContainsKey("parentalcontrols"))
                this.parentalcontrols = Values["parentalcontrols"] == "1";

            if (Values.ContainsKey("encoded"))
                this.encoded = Values["encoded"] == "1";
        }

        #endregion Private Methods
    }
}