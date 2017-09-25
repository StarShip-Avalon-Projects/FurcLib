using System;

namespace Furcadia.FurcMap
{
    [CLSCompliant(true)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MapPosition'
    public class MapPosition
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapPosition'
    {
        #region Public Fields

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.objectNumber'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.effectNumber'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.floorNumber'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.regionNumber'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.wallNumber'
        public int floorNumber, objectNumber, wallNumber, regionNumber, effectNumber;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.wallNumber'
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.regionNumber'
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.floorNumber'
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.effectNumber'
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.objectNumber'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.y'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.x'
        public int x, y;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.x'
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.y'

        #endregion Public Fields

        #region Public Constructors

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.MapPosition(int, int)'

        public MapPosition(int x, int y)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.MapPosition(int, int)'
        {
            this.x = x;
            this.y = y;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.MapPosition(int, int, Map)'

        public MapPosition(int x, int y, Map map)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MapPosition.MapPosition(int, int, Map)'
        {
            this.x = x;
            this.y = y;
            this.floorNumber = map.getFloorAt(x, y);
            this.objectNumber = map.getObjectAt(x, y);
            this.wallNumber = map.getWallAt(x, y);
            this.regionNumber = map.getRegionAt(x, y);
            this.effectNumber = map.getEffectAt(x, y);
        }

        #endregion Public Constructors
    }
}