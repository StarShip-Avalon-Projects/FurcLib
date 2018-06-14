using System;
using Furcadia.Net.DreamInfo;
using Furcadia.Text;
using static Extentions.MathExtensions;

namespace Furcadia.Drawing
{
    //Dim tilesWide As UInt32 = extraTilesLeft + 7 + 1 + 7 + extraTilesRight
    //Dim tilesHigh As UInt32 = extraTilesTop + 8 + 1 + 8 + extraTilesBottom
    // NB: these lines *look* similar, but the numbers are for *completely* different reasons!
    //tilesWide = tilesWide * 2 ' * 2 as all X values are even: we count 0, 2, 4...
    //tilesHigh = tilesHigh * 2 ' * 2 as zig-zaggy vertical cols can fit twice as many tiles to a column

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
            FurreLocation = new FurrePosition();
        }

        /// <summary>
        /// Initialize from center point usually a furre position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPosition(Base220 x, Base220 y)
        {
            furreLocation = new FurrePosition(x, y);
            int XoddOffset = 2;
            int YoddOffset = 0;
            if (IsOdd(furreLocation.Y))
            {
                XoddOffset = 0;
                YoddOffset = 1;
            }
            StartLocation = new FurrePosition()
            {
                X = furreLocation.X - 8 + XoddOffset,
                Y = furreLocation.Y - 8 - 1
            };

            EndLocation = new FurrePosition()
            {
                X = FurreLocation.X + 14,
                Y = FurreLocation.Y + 17 + YoddOffset
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewArea"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        public ViewArea(FurrePosition location)
        {
            SetPosition(location.X, location.Y);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ViewArea(Base220 x, Base220 y)
        {
            SetPosition(x, y);
        }

        #region Public Fields

        /// <summary>
        /// Gets or sets the furre location in the center of the View rectangle.
        /// </summary>
        /// <value>
        /// The furre location.
        /// </value>
        public FurrePosition FurreLocation
        {
            get { return furreLocation; }

            set
            {
                SetPosition(value.X, value.Y);
                furreLocation = value;
            }
        }

        private FurrePosition furreLocation;

        /// <summary>
        /// Upper Left corner where map drawing begins.
        /// </summary>
        /// <value>
        /// The start location.
        /// </value>
        public FurrePosition StartLocation { get; private set; } = new FurrePosition();

        /// <summary>
        /// Lower right corner where the map drawing of the rectangle ends
        /// </summary>
        public FurrePosition EndLocation { get; private set; } = new FurrePosition();

        /// <summary>
        /// Target location is in our viewable rectangle?
        /// </summary>
        /// <param name="TargetLocation">The location.</param>
        /// <returns></returns>
        public bool InRange(FurrePosition TargetLocation)
        {
            return StartLocation.X <= TargetLocation.X
                    && StartLocation.Y <= TargetLocation.Y
                    && EndLocation.Y >= TargetLocation.Y
                    && EndLocation.X >= TargetLocation.X;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"({StartLocation} - {EndLocation})";
        }

        #endregion Public Fields
    }
}