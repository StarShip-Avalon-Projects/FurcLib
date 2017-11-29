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
            this.floorNumber = map.GetFloorAt(x, y);
            this.objectNumber = map.GetObjectAt(x, y);
            this.regionNumber = map.GetRegionAt(x, y);
            this.effectNumber = map.GetEffectAt(x, y);

            if (x % 2 == 0)
            {
                this.wallNENumber = map.GetWallAt(x + 1, y);
                this.wallNWNumber = map.GetWallAt(x, y);
            }
            else
            {
                this.wallNENumber = map.GetWallAt(x, y);
                this.wallNWNumber = map.GetWallAt(x - 1, y);
            }
        }

        #endregion Public Constructors
    }
}