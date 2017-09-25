namespace Furcadia.Drawing
{

    /// <summary>
    /// Furre Visible area
    /// </summary>
    public static class VisibleArea
    {
        #region Public Methods

            /// <summary>
            /// Gets the target View area from the center coordinates, This is useful for finding the View area of the Connected Furre
            /// </summary>
            /// <param name="X"></param>
            /// <param name="Y"></param>
            /// <returns></returns>
        public static ViewArea GetTargetRectFromCenterCoord(int X, int Y)

        {
            // Set the size of the rectangle drawn around the player. The +1
            // is the tile the player is on.
            var rec = new ViewArea();
            //Dim tilesWide As UInt32 = extraTilesLeft + 7 + 1 + 7 + extraTilesRight
            //Dim tilesHigh As UInt32 = extraTilesTop + 8 + 1 + 8 + extraTilesBottom
            // NB: these lines *look* similar, but the numbers are for *completely* different reasons!
            //tilesWide = tilesWide * 2 ' * 2 as all X values are even: we count 0, 2, 4...
            //tilesHigh = tilesHigh * 2 ' * 2 as zig-zaggy vertical cols can fit twice as many tiles to a column

            // Set where in the map our visible (0,0) will be.
            int XoddOffset = 2;
            int YoddOffset = 0;
            if (IsOdd(Y))
            {
                XoddOffset = 0;
                YoddOffset = 1;
            }
            rec.X = X - 8 + XoddOffset;
            rec.Y = Y - 8 - 1;
            // 1 for the tile the user is in.
            rec.length = rec.X + 14;
            rec.height = rec.Y + 17 + YoddOffset;
            return rec;
        }

        /// <summary>
        /// Integer is Odd
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Visible are a Furre can see
    /// </summary>
    public class ViewArea
    {
        #region Public Fields

        /// <summary>
        /// height
        /// </summary>
        public int height;

        /// <summary>
        /// length
        /// </summary>
        public int length;

        /// <summary>
        /// X Coordinate
        /// </summary>
        public int X;

        /// <summary>
        /// Y Coordinate
        /// </summary>
        public int Y;

        #endregion Public Fields
    }
}