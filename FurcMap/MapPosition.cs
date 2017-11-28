using System;

namespace Furcadia.FurcMap
{
    /// <summary>
    ///tile data
    /// </summary>
    [CLSCompliant(true)]
    public class MapPosition
    {
        #region Public Fields

        /// <summary>
        ///objects
        /// </summary>
        public int floorNumber, objectNumber, wallNumber, regionNumber, effectNumber;

        /// <summary>
        ///Coordinates
        /// </summary>
        public int x, y;

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public MapPosition(int x, int y)

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
        public MapPosition(int x, int y, Map map)
        {
            this.x = x;
            this.y = y;
            floorNumber = map.GetFloorAt(x, y);
            objectNumber = map.GetObjectAt(x, y);
            wallNumber = map.GetWallAt(x, y);
            regionNumber = map.GetRegionAt(x, y);
            effectNumber = map.GetEffectAt(x, y);
        }

        #endregion Public Constructors
    }
}