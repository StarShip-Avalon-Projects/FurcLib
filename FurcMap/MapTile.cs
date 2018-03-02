namespace Furcadia.FurcMap
{
    /// <summary>
    /// Map objects position information
    /// </summary>
    public class MapTile
    {
        #region Public Fields

        /// <summary>
        ///
        /// </summary>
        public int floorNumber, objectNumber, wallNENumber, wallNWNumber, regionNumber, effectNumber;

        /// <summary>
        ///
        /// </summary>
        public int x, y;

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTile"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public MapTile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTile"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="map">The map.</param>
        public MapTile(int x, int y, Map map)
        {
            this.x = x;
            this.y = y;
            floorNumber = map.GetFloorAt(x, y);
            objectNumber = map.GetObjectAt(x, y);
            regionNumber = map.GetRegionAt(x, y);
            effectNumber = map.GetEffectAt(x, y);

            if (x % 2 == 0)
            {
                wallNENumber = map.GetWallAt(x + 1, y);
                wallNWNumber = map.GetWallAt(x, y);
            }
            else
            {
                wallNENumber = map.GetWallAt(x, y);
                wallNWNumber = map.GetWallAt(x - 1, y);
            }
        }

        #endregion Public Constructors
    }
}