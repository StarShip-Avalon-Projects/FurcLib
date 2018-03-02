using System;
using Furcadia.Net.DreamInfo;

namespace Furcadia.Drawing
{
    /// <summary>
    /// Furre Visible area
    /// </summary>
    public class VisibleArea
    {
        #region Public Methods

        private ViewArea rec = new ViewArea();

        /// <summary>
        /// Gets the target View area from the center coordinates, This is useful for finding the View area of the Connected Furre
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public VisibleArea(int X, int Y) : this(new FurrePosition(X, Y))
        {
        }

        /// <summary>
        /// Gets the target rect from center coord.
        /// </summary>
        /// <param name="CenterLocation">The center location.</param>
        /// <returns></returns>
        public VisibleArea(FurrePosition CenterLocation)
        {
            // Set the size of the rectangle drawn around the player. The +1
            // is the tile the player is on.

            //Dim tilesWide As UInt32 = extraTilesLeft + 7 + 1 + 7 + extraTilesRight
            //Dim tilesHigh As UInt32 = extraTilesTop + 8 + 1 + 8 + extraTilesBottom
            // NB: these lines *look* similar, but the numbers are for *completely* different reasons!
            //tilesWide = tilesWide * 2 ' * 2 as all X values are even: we count 0, 2, 4...
            //tilesHigh = tilesHigh * 2 ' * 2 as zig-zaggy vertical cols can fit twice as many tiles to a column

            // Set where in the map our visible (0,0) will be.
            int XoddOffset = 2;
            int YoddOffset = 0;
            if (IsOdd(CenterLocation.Y))
            {
                XoddOffset = 0;
                YoddOffset = 1;
            }
            rec.FurreLocation.X = CenterLocation.X - 8 + XoddOffset;
            rec.FurreLocation.Y = CenterLocation.Y - 8 - 1;
            // 1 for the tile the user is in.
            rec.Length = rec.FurreLocation.X + 14;
            rec.Height = rec.FurreLocation.Y + 17 + YoddOffset;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewArea"/> class.
        /// </summary>
        public ViewArea()
        {
            location = new FurrePosition();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewArea"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public ViewArea(int x, int y)
        {
            location = new FurrePosition(x, y);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewArea"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        public ViewArea(FurrePosition location)
        {
            this.location = location;
        }

        #region Public Fields

        private FurrePosition location;

        /// <summary>
        /// height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// length
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the furre location.
        /// </summary>
        /// <value>
        /// The furre location.
        /// </value>
        public FurrePosition FurreLocation
        {
            get => location;
            set => location = value;
        }

        #endregion Public Fields
    }
}