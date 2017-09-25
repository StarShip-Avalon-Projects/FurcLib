using Furcadia.Drawing.Graphics;
using System.Collections.Generic;
using System.Drawing;

namespace Furcadia.Drawing
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Helper'

    public static class Helper
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Helper'
    {
        #region Public Methods

        /// <summary>
        /// Converts a Char to Desc Tag
        /// </summary>
        /// <param name="c">
        /// </param>
        /// <returns>
        /// </returns>
        public static int CharToDescTag(char c)
        {
            return (int)(c - 33);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Helper.ToBitmapArray(FurcadiaShapes)'

        public static Bitmap[] ToBitmapArray(FurcadiaShapes toConvert)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Helper.ToBitmapArray(FurcadiaShapes)'
        {
            List<Bitmap> bitmaps = new List<Bitmap>();
            Palette pal = Palette.Default;
            Shape[] shapes = toConvert.Shapes;
            foreach (Shape shape in shapes)
            {
                foreach (Frame frame in shape.Frames)
                {
                    if (frame.ImageDataSize > 0)
                        bitmaps.Add(toConvert.ToBitmap(frame, pal));
                }
            }
            return bitmaps.ToArray();
        }

        #endregion Public Methods
    }
}