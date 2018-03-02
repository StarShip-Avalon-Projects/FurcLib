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

            floors = new byte[BytesLayerCount];
            objects = new byte[BytesLayerCount];
            walls = new byte[BytesLayerCount];
            regions = new byte[BytesLayerCount];
            effects = new byte[BytesLayerCount];

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

            byte[] mapMatrix = new byte[this.BytesLayerCount * 5];
            this.mapMatrix = mapMatrix;
        }

        #endregion Public Constructors

        #region Internal Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Map"/> class.
        /// </summary>
        internal Map()
        {
        }

        #endregion Internal Constructors

        #region Internal Properties

        /// <summary>
        /// Gets the bytes layer count.
        /// </summary>
        /// <value>
        /// The bytes layer count.
        /// </value>
        internal int BytesLayerCount
        {
            get
            {
                return ((width * 2) * height);
            }
        }

        #endregion Internal Properties

        #region Public Variables

        /// <summary>
        /// Allow furres to enter via the Dream URL?
        /// </summary>
        public bool AllowDreamURL
        {
            get { return allowfurl; }
            set { allowfurl = value; }
        }

        /// <summary>
        /// Allow Furres to Sommon and join each other in this dream?
        /// </summary>
        public bool AllowJoinSummon
        {
            get { return this.allowjs; }
            set { this.allowjs = value; }
        }

        /// <summary>
        /// Allow Dream Pack sizes?
        /// </summary>
        public bool AllowLargeDreamSize
        {
            get { return allowlarge; }
            set { allowlarge = value; }
        }

        /// <summary>
        /// Allow Lead and Follow commands in the dream?
        /// </summary>
        public bool AllowLeadFollow
        {
            get { return allowlf; }
            set { allowlf = value; }
        }

        /// <summary>
        /// Allow Furres to use the shout channel in the dream?
        /// </summary>
        public bool AllowShouting

        {
            get { return allowshouts; }
            set { allowshouts = value; }
        }

        /// <summary>
        /// Encrypt the dream?
        /// </summary>
        public bool EncodeDream
        {
            get { return encoded; }
            set { encoded = value; }
        }

        /// <summary>
        /// enforce parental controls
        /// </summary>
        public bool EnforceParentalControls
        {
            get { return parentalcontrols; }
            set { parentalcontrols = value; }
        }

        /// <summary>
        /// force sitting ?
        /// </summary>
        public bool ForceSitting
        {
            get { return forcesittable; }
            set { forcesittable = value; }
        }

        /// <summary>
        /// The actual height of the map (READ-ONLY)
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// Name of the dream. Dream  Title?
        /// </summary>
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Link to the Patch archive
        /// </summary>
        public String PatchArchive
        {
            get { return patchs; }
            set { patchs = value; }
        }

        /// <summary>
        /// prevent F4 to see the player list?
        /// </summary>
        public bool PreventPlayerListing
        {
            get { return nowho; }
            set { nowho = value; }
        }

        /// <summary>
        /// Allow Seasonal Avatars in the dream?
        /// </summary>
        public bool PreventSeasonalAvatars
        {
            get { return nonovelty; }
            set { nonovelty = value; }
        }

        /// <summary>
        /// prevent Tab from showing furre names
        /// </summary>
        public bool PreventTabListing
        {
            get { return notab; }
            set { notab = value; }
        }

        /// <summary>
        /// Dream Rating
        /// </summary>
        public String Rating
        {
            get { return rating; }
            set { rating = value; }
        }

        /// <summary>
        /// revision?
        /// </summary>
        public int Revision
        {
            get { return revision; }
            set { revision = value; }
        }

        /// <summary>
        /// type of dream patch to use
        /// </summary>
        public int UsePatch
        {
            get { return patcht; }
            set { patcht = value; }
        }

        /// <summary>
        /// Swear Filter
        /// </summary>
        public bool UseSwearFilter
        {
            get { return swearfilter; }
            set { swearfilter = value; }
        }

        /// <summary>
        /// The actual width of the map (READ-ONLY)
        /// </summary>
        public int Width
        {
            get { return width * 2; }
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

            m.floors = new byte[m.BytesLayerCount];
            m.objects = new byte[m.BytesLayerCount];
            m.walls = new byte[m.BytesLayerCount];
            m.regions = new byte[m.BytesLayerCount];
            m.effects = new byte[m.BytesLayerCount];

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
        public int GetEffectAt(int x, int y)
        {
            int pos = GetPosFrom(x, y);

            return effects[pos];
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
        public int GetFloorAt(int x, int y)
        {
            int pos = GetPosFrom(x, y);

            return floors[pos];
        }

        /// <summary> Get a MapPosition object from the position specified
        /// by x &amp; y </summary>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        /// <returns></returns>
        public MapPosition GetMapPos(int x, int y)
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
        public MapTile GetMapTile(int x, int y)
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
        public int GetObjectAt(int x, int y)
        {
            int pos = GetPosFrom(x, y);

            return objects[pos];
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
        public int GetRegionAt(int x, int y)
        {
            int pos = GetPosFrom(x, y);

            return regions[pos];
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
        public int GetWallAt(int x, int y)
        {
            int pos = (height * x + y);

            return walls[pos];
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

            StringBuilder headerData = new StringBuilder()
            .Append("MAP V01.40 Furcadia\n")
            .Append("height=" + height + "\n")
            .Append("width=" + width + "\n")
            .Append("revision=" + revision + "\n")
            .Append("patcht=" + patcht + "\n")
            .Append("name=" + name + "\n")
            .Append("patchs=" + patchs + "\n")
            .Append("encoded=" + (encoded ? "1" : "0") + "\n")
            .Append("allowjs=" + (allowjs ? "1" : "0") + "\n")
            .Append("allowlf=" + (allowlf ? "1" : "0") + "\n")
            .Append("allowfurl=" + (allowfurl ? "1" : "0") + "\n")
            .Append("swearfilter=" + (swearfilter ? "1" : "0") + "\n")
            .Append("nowho=" + (nowho ? "1" : "0") + "\n")
            .Append("forcesittable=" + (forcesittable ? "1" : "0") + "\n")
            .Append("allowshouts=" + (allowshouts ? "1" : "0") + "\n")
            .Append("rating=" + rating + "\n")
            .Append("allowlarge=" + (allowlarge ? "1" : "0") + "\n")
            .Append("notab=" + (notab ? "1" : "0") + "\n")
            .Append("nonovelty=" + (nonovelty ? "1" : "0") + "\n")
            .Append("parentalcontrols=" + (parentalcontrols ? "1" : "0") + "\n")
            .Append("BODY\n");

            byte[] headerDataBytes = Encoding.GetEncoding(1252).GetBytes(headerData.ToString());

            sw.Write(headerDataBytes);
            sw.Write(floors);
            sw.Write(objects);
            sw.Write(walls);
            sw.Write(regions);
            sw.Write(effects);

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
        public void SetEffectAt(int x, int y, int effectNumber)
        {
            int pos = GetPosFrom(x, y);

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
        public void SetFloorAt(int x, int y, int floorNumber)
        {
            int pos = GetPosFrom(x, y);

            floors[pos] = (byte)floorNumber;
        }

        /// <summary> Set the object number at a tile specified by x &amp; y
        /// </summary>
        /// <param name="x">
        /// x coordinate
        /// </param> <param name="y">
        /// Y coordinate
        /// </param>
        /// <param name="objectNumber"></param>
        public void SetObjectAt(int x, int y, int objectNumber)
        {
            int pos = GetPosFrom(x, y);

            objects[pos] = (byte)objectNumber;
        }

        /// <summary> Set the region number at a tile specified by x &amp; y
        /// </summary>
        /// <param name="x">
        /// x coordinate
        /// </param> <param name="y">
        /// Y coordinate
        /// </param>
        ///<param name="regionNumber"></param>
        public void SetRegionAt(int x, int y, int regionNumber)

        {
            int pos = GetPosFrom(x, y);

            regions[pos] = (byte)regionNumber;
        }

        /// <summary> Set the wall number at a tile specified by x &amp; y
        /// </summary>
        /// <param name="x">
        /// x coordinate
        /// </param> <param name="y">
        ///
        /// </param><param name="wallNumber"></param>
        public void SetWallAt(int x, int y, int wallNumber)
        {
            int pos = (height * x + y);

            walls[pos] = (byte)wallNumber;
        }

        #endregion Public Methods

        #region Private Methods

        private int GetPosFrom(int x, int y)
        {
            return ((height * (x / 2) + y) * 2);
        }

        private bool ParseMatrix(byte[] matrix)
        {
            if (matrix.Length != BytesLayerCount * 5)
            {
                Console.WriteLine("Something is wrong here...");
                return false;
            }
            mapMatrix = matrix;

            for (int i = 0; i < BytesLayerCount; i++)
                floors[i] = matrix[i];

            for (int i = 0; i < BytesLayerCount; i++)
            {
                objects[i] = matrix[i + BytesLayerCount];
            }

            for (int i = 0; i < BytesLayerCount; i++)
            {
                walls[i] = matrix[i + (BytesLayerCount * 2)];
            }

            for (int i = 0; i < BytesLayerCount; i++)
            {
                regions[i] = matrix[i + (BytesLayerCount * 3)];
            }

            for (int i = 0; i < BytesLayerCount; i++)
            {
                effects[i] = matrix[i + (BytesLayerCount * 4)];
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
                width = int.Parse(Values["height"]);

            if (Values.ContainsKey("width"))
                width = int.Parse(Values["width"]);

            if (Values.ContainsKey("revision"))
                revision = int.Parse(Values["revision"]);

            if (Values.ContainsKey("patcht"))
                patcht = int.Parse(Values["patcht"]);

            if (Values.ContainsKey("name"))
                name = Values["name"];

            if (Values.ContainsKey("patchs"))
                patchs = Values["patchs"];

            if (Values.ContainsKey("rating"))
                rating = Values["rating"];

            if (Values.ContainsKey("allowjs"))
                allowjs = Values["allowjs"] == "1";

            if (Values.ContainsKey("allowlf"))
                allowlf = Values["allowlf"] == "1";

            if (Values.ContainsKey("allowfurl"))
                allowfurl = Values["allowfurl"] == "1";

            if (Values.ContainsKey("swearfilter"))
                swearfilter = Values["swearfilter"] == "1";

            if (Values.ContainsKey("nowho"))
                nowho = Values["nowho"] == "1";

            if (Values.ContainsKey("forcesittable"))
                forcesittable = Values["forcesittable"] == "1";

            if (Values.ContainsKey("allowlarge"))
                allowlarge = Values["allowlarge"] == "1";

            if (Values.ContainsKey("allowshouts"))
                allowshouts = Values["allowshouts"] == "1";

            if (Values.ContainsKey("notab"))
                notab = Values["notab"] == "1";

            if (Values.ContainsKey("nonovelty"))
                nonovelty = Values["nonovelty"] == "1";

            if (Values.ContainsKey("parentalcontrols"))
                parentalcontrols = Values["parentalcontrols"] == "1";

            if (Values.ContainsKey("encoded"))
                encoded = Values["encoded"] == "1";
        }

        #endregion Private Methods
    }
}