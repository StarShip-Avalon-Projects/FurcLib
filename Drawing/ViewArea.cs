namespace Furcadia.Drawing
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'VisibleArea'

    public static class VisibleArea
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'VisibleArea'
    {
        #region Public Methods

        //Reference http://pastebin.com/QbnjwjNc
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'VisibleArea.getTargetRectFromCenterCoord(int, int)'

        public static ViewArea getTargetRectFromCenterCoord(int X, int Y)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'VisibleArea.getTargetRectFromCenterCoord(int, int)'
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'VisibleArea.IsOdd(int)'

        public static bool IsOdd(int value)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'VisibleArea.IsOdd(int)'
        {
            return value % 2 != 0;
        }

        #endregion Public Methods
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ViewArea'

    public class ViewArea
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ViewArea'
    {
        #region Public Fields

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ViewArea.height'
        public int height;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ViewArea.height'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ViewArea.length'
        public int length;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ViewArea.length'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ViewArea.X'
        public int X;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ViewArea.X'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ViewArea.Y'
        public int Y;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ViewArea.Y'

        #endregion Public Fields
    }
}