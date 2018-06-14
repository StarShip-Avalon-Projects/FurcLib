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
    /// Furcadia Furre View area (Isometric Map View)
    /// </summary>
    public class ViewArea
    {
        #region Private Fields

        private FurrePosition furreLocation;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewArea"/> class.
        /// </summary>
        public ViewArea()
        {
            FurreLocation = new FurrePosition();
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

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Lower right corner where the map drawing of the rectangle ends
        /// </summary>
        public FurrePosition EndLocation { get; private set; } = new FurrePosition();

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

        /// <summary>
        /// Upper Left corner where map drawing begins.
        /// </summary>
        /// <value>
        /// The start location.
        /// </value>
        public FurrePosition StartLocation { get; private set; } = new FurrePosition();

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="obj1">The obj1.</param>
        /// <param name="obj2">The obj2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(ViewArea obj1, ViewArea obj2)
        {
            return !obj1.Equals(obj2);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="obj1">The obj1.</param>
        /// <param name="obj2">The obj2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ViewArea obj1, ViewArea obj2)
        {
            return obj1.Equals(obj2);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is ViewArea view)
                return view.EndLocation == EndLocation && view.StartLocation == StartLocation;

            return base.Equals(obj);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return StartLocation.GetHashCode() ^ EndLocation.GetHashCode();
        }

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
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"(Top Left: {StartLocation} - Bottom Right: {EndLocation})";
        }

        #endregion Public Methods
    }
}