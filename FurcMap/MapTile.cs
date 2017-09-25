namespace Furcadia.FurcMap
{
    /// <summary>
    /// Map objects position information
    /// </summary>
    public class MapTile
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapTile'
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
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public MapTile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="map"></param>
        public MapTile(int x, int y, Map map)
        {
            this.x = x;
            this.y = y;
            this.floorNumber = map.getFloorAt(x, y);
            this.objectNumber = map.getObjectAt(x, y);
            this.regionNumber = map.getRegionAt(x, y);
            this.effectNumber = map.getEffectAt(x, y);

            if (x % 2 == 0)
            {
                this.wallNENumber = map.getWallAt(x + 1, y);
                this.wallNWNumber = map.getWallAt(x, y);
            }
            else
            {
                this.wallNENumber = map.getWallAt(x, y);
                this.wallNWNumber = map.getWallAt(x - 1, y);
            }
        }

        #endregion Public Constructors
    }
}