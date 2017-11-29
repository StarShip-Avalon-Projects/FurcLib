#region Credit

/*
CREDIT: Kylix http://forums.furcadia.com/index.php?furcadia_session_id=12550-ucpo-pet&showtopic=45869&hl=kylix+fox
*/

#endregion Credit

using Furcadia.IO;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Furcadia.Drawing.Graphics
{
    /// <summary>
    ///
    /// </summary>
    public struct Frame
    {
        #region Public Fields

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Frame.FrameFormat'
        public FrameFormats FrameFormat;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Frame.FrameFormat'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Frame.FramePos'
        public Pos FramePos;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Frame.FramePos'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Frame.FurrePos'
        public Pos FurrePos;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Frame.FurrePos'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Frame.Height'
#pragma warning disable CS3003 // Type of 'Frame.Height' is not CLS-compliant
        public ushort Height;
#pragma warning restore CS3003 // Type of 'Frame.Height' is not CLS-compliant
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Frame.Height'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Frame.ImageData'
        public byte[] ImageData;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Frame.ImageData'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Frame.ImageDataSize'
#pragma warning disable CS3003 // Type of 'Frame.ImageDataSize' is not CLS-compliant
        public uint ImageDataSize;
#pragma warning restore CS3003 // Type of 'Frame.ImageDataSize' is not CLS-compliant
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Frame.ImageDataSize'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Frame.Width'
#pragma warning disable CS3003 // Type of 'Frame.Width' is not CLS-compliant
        public ushort Width;
#pragma warning restore CS3003 // Type of 'Frame.Width' is not CLS-compliant
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Frame.Width'

        #endregion Public Fields

        #region Public Enums

        [Flags]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Frame.FrameFormats'
#pragma warning disable CS3009 // 'Frame.FrameFormats': base type 'ushort' is not CLS-compliant
        public enum FrameFormats : ushort
#pragma warning restore CS3009 // 'Frame.FrameFormats': base type 'ushort' is not CLS-compliant
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Frame.FrameFormats'
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Frame.FrameFormats.Format8Bit'
            Format8Bit = 1,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Frame.FrameFormats.Format8Bit'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Frame.FrameFormats.FormatRGB'
            FormatRGB
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Frame.FrameFormats.FormatRGB'
        }

        #endregion Public Enums

        #region Public Structs

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Frame.Pos'

        public struct Pos
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Frame.Pos'
        {
            #region Public Fields

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Frame.Pos.X'
            public short X;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Frame.Pos.X'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Frame.Pos.Y'
            public short Y;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Frame.Pos.Y'

            #endregion Public Fields
        }

        #endregion Public Structs
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Shape'

    public struct Shape
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Shape'
    {
        #region Public Fields

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Shape.Flags'
        public ShapeFlags Flags;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Shape.Flags'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Shape.Frames'
        public Frame[] Frames;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Shape.Frames'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Shape.KitterSpeak'
        public StepBlock[] KitterSpeak;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Shape.KitterSpeak'

#pragma warning disable CS3003 // Type of 'Shape.NumFrames' is not CLS-compliant
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Shape.NumFrames'
        public ushort NumFrames;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Shape.NumFrames'
#pragma warning restore CS3003 // Type of 'Shape.NumFrames' is not CLS-compliant

#pragma warning disable CS3003 // Type of 'Shape.NumSteps' is not CLS-compliant
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Shape.NumSteps'
        public ushort NumSteps;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Shape.NumSteps'
#pragma warning restore CS3003 // Type of 'Shape.NumSteps' is not CLS-compliant

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Shape.ShapeNum'
        public short ShapeNum;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Shape.ShapeNum'

        #endregion Public Fields

        #region Public Enums

        [Flags]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Shape.ShapeFlags'
#pragma warning disable CS3009 // 'Shape.ShapeFlags': base type 'ushort' is not CLS-compliant
        public enum ShapeFlags : ushort
#pragma warning restore CS3009 // 'Shape.ShapeFlags': base type 'ushort' is not CLS-compliant
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Shape.ShapeFlags'
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Shape.ShapeFlags.Walkable'
            Walkable = 1,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Shape.ShapeFlags.Walkable'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Shape.ShapeFlags.Gettable'
            Gettable = 2,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Shape.ShapeFlags.Gettable'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Shape.ShapeFlags.Sittable'
            Sittable = 4
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Shape.ShapeFlags.Sittable'
        }

        #endregion Public Enums
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock'

    public struct StepBlock
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock'
    {
        #region Public Fields

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.CounterMax'
        public short CounterMax;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.CounterMax'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepType'
        public StepBlockStepTypes StepType;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepType'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.Value'
        public short Value;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.Value'

        #endregion Public Fields

        #region Public Enums

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes'
#pragma warning disable CS3009 // 'StepBlock.StepBlockStepTypes': base type 'ushort' is not CLS-compliant

        public enum StepBlockStepTypes : ushort
#pragma warning restore CS3009 // 'StepBlock.StepBlockStepTypes': base type 'ushort' is not CLS-compliant
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes'
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.Frame'
            Frame = 1,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.Frame'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.Delay'
            Delay,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.Delay'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.Loop'
            Loop,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.Loop'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.Jump'
            Jump,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.Jump'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.PosX'
            PosX,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.PosX'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.PosY'
            PosY,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.PosY'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.FurreX'
            FurreX,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.FurreX'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.FurreY'
            FurreY,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.FurreY'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.DrawFront'
            DrawFront,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.DrawFront'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.DrawBehind'
            DrawBehind,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.DrawBehind'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.AutoFrameDelay'
            AutoFrameDelay,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.AutoFrameDelay'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.Stop'
            Stop,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.Stop'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.CameraState'
            CameraState
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StepBlock.StepBlockStepTypes.CameraState'
        }

        #endregion Public Enums
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaPaletteException'

    public class FurcadiaPaletteException : System.ApplicationException
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaPaletteException'
    {
        #region Public Constructors

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaPaletteException.FurcadiaPaletteException(string)'

        public FurcadiaPaletteException(string msg)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaPaletteException.FurcadiaPaletteException(string)'
            : base(msg)
        {
        }

        #endregion Public Constructors
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes'

    public class FurcadiaShapes
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes'
    {
        #region Public Fields

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.Encryption'
        public int Encryption;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.Encryption'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.Generator'
        public int Generator;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.Generator'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.Header'
        public string Header;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.Header'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.NumShapes'
        public int NumShapes;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.NumShapes'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.Reserved1'
        public int Reserved1;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.Reserved1'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.Reserved2'
        public int Reserved2;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.Reserved2'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.Shapes'
        public Shape[] Shapes;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.Shapes'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.Version'
        public int Version;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.Version'

        #endregion Public Fields

        #region Public Constructors

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.FurcadiaShapes(string)'

        public FurcadiaShapes(string path)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.FurcadiaShapes(string)'
        {
            /* Don't fiddle with extensions */
            System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.StreamReader(path).BaseStream);
            string header = System.Text.Encoding.GetEncoding(1252).GetString(br.ReadBytes(4)).ToLower();
            br.Close();
            switch (header)
            {
                case "fshx":
                    LoadFOX(path);
                    break;

                case "fsh2":
                    LoadFS2(path);
                    break;

                default:
                    LoadFSH(path);
                    break;
            }
        }

        #endregion Public Constructors

        #region Private Enums

        [FlagsAttribute]
        private enum FBJFlags : byte
        {
            Walkable = 1,
            Gettable = 2,
            Sittable = 4,
            FurreX = 8,
            FurreY = 16
        }

        #endregion Private Enums

        #region Public Methods

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.ToBitmap(Frame, Palette)'

        public System.Drawing.Bitmap ToBitmap(Furcadia.Drawing.Graphics.Frame frame, Palette pal)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapes.ToBitmap(Frame, Palette)'
        {
            /* Create a new bitmap */
            try
            {
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(frame.Width, frame.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                /* Set up the bitmap data for use */
                IntPtr ptr = bmpData.Scan0;
                int bytes = bmpData.Stride * bmp.Height;
                byte[] rgbValues = new byte[bytes];

                /* Copy the image data over */
                if (frame.FrameFormat == Frame.FrameFormats.Format8Bit)
                {
                    int bpos, ipos = (int)frame.ImageDataSize - 1;
                    for (int y = 0; y < bmpData.Height; y++)
                    {
                        /* Alignment and flip */
                        bpos = bmpData.Stride * (y + 1) - (bmpData.Stride - bmpData.Width * 4) - 1;

                        /* Pixel assignment */
                        for (int x = 0; x < bmpData.Width; x++)
                        {
                            rgbValues[bpos--] = pal.Colors[frame.ImageData[ipos]].A;
                            rgbValues[bpos--] = pal.Colors[frame.ImageData[ipos]].R;
                            rgbValues[bpos--] = pal.Colors[frame.ImageData[ipos]].G;
                            rgbValues[bpos--] = pal.Colors[frame.ImageData[ipos--]].B;
                        }
                    }
                    /* Save the bitmap */
                    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
                    bmp.UnlockBits(bmpData);
                }
                else if (frame.FrameFormat == Frame.FrameFormats.FormatRGB)
                {
#pragma warning disable CS1030 // #warning: '24-bit is not supported in this version. For updates: http://furcadia.codeplex.com/'
#warning 24-bit is not supported in this version. For updates: http://furcadia.codeplex.com/
                    //24 bit support
                }
#pragma warning restore CS1030 // #warning: '24-bit is not supported in this version. For updates: http://furcadia.codeplex.com/'
                return bmp;
            }
            catch
            {
                /* Because we don't care if it didn't render */
                return null;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private string ChangeExtension(string path, string newExtension)
        {
            char[] delims = { '/', '\\' };
            string[] structure = path.Split(delims);
            string fullfile = structure[structure.GetUpperBound(0)];
            string filename = fullfile.Substring(0, fullfile.LastIndexOf('.'));
            string filepath = path.Substring(0, path.LastIndexOfAny(delims) + 1);
            return (filepath + filename + "." + newExtension);
        }

        private void LoadFBJ(string path)
        {
            string file = ChangeExtension(path, "fbj");
            if (!File.Exists(file)) return;
            System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.StreamReader(file).BaseStream);

            /* Ignore header */
            br.ReadBytes(4);
            int chunkCount = br.ReadInt32();

            /* Read object definitions */
            for (int f = 0; f < chunkCount; f++)
            {
                byte thisFlag = br.ReadByte();
                Shapes[f].Flags = (Shape.ShapeFlags)(thisFlag & 0x07);

                if ((thisFlag & ((byte)FBJFlags.FurreX)) > 0)
                {
                    Shapes[f].Frames[0].FurrePos.X = (short)br.ReadSByte();

                    if ((thisFlag & ((byte)FBJFlags.FurreY)) > 0) Shapes[f].Frames[0].FurrePos.Y = (short)br.ReadSByte();
                }
                else if ((thisFlag & ((byte)FBJFlags.FurreY)) > 0)
                {
                    Shapes[f].Frames[0].FurrePos.Y = (short)br.ReadSByte();
                }
            }

            br.Close();
        }

        private void LoadFOX(string path)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.StreamReader(path).BaseStream);

            /* Retrieve header information */
            Header = System.Text.Encoding.GetEncoding(1252).GetString(br.ReadBytes(4));
            Version = br.ReadInt32();
            NumShapes = br.ReadInt32();
            Generator = br.ReadInt32();
            Encryption = br.ReadInt32();
            if (Encryption > 0)
            {
                br.Close();
                throw new FurcadiaShapesException("The FOX file (" + path + ") is encrypted and will not be opened.");
            }
            Reserved1 = br.ReadInt32();
            Reserved2 = br.ReadInt32();

            /* Loop through the shapes */
            Shapes = new Shape[NumShapes];
            for (int s = 0; s < NumShapes; s++)
            {
                /* Retrieve shape header information */
                Shapes[s].Flags = (Furcadia.Drawing.Graphics.Shape.ShapeFlags)br.ReadUInt16();
                Shapes[s].ShapeNum = br.ReadInt16();
                Shapes[s].NumFrames = br.ReadUInt16();
                Shapes[s].NumSteps = br.ReadUInt16();

                /* Loop through the frames */
                Shapes[s].Frames = new Frame[Shapes[s].NumFrames];
                for (int f = 0; f < Shapes[s].NumFrames; f++)
                {
                    /* Retrieve frame information */
                    Shapes[s].Frames[f].FrameFormat = (Furcadia.Drawing.Graphics.Frame.FrameFormats)br.ReadUInt16();
                    Shapes[s].Frames[f].Width = br.ReadUInt16();
                    Shapes[s].Frames[f].Height = br.ReadUInt16();
                    Shapes[s].Frames[f].FramePos.X = br.ReadInt16();
                    Shapes[s].Frames[f].FramePos.Y = br.ReadInt16();
                    Shapes[s].Frames[f].FurrePos.X = br.ReadInt16();
                    Shapes[s].Frames[f].FurrePos.Y = br.ReadInt16();
                    Shapes[s].Frames[f].ImageDataSize = br.ReadUInt32();
                    Shapes[s].Frames[f].ImageData = br.ReadBytes((int)Shapes[s].Frames[f].ImageDataSize);
                }

                /* Loop through the KitterSpeak */
                Shapes[s].KitterSpeak = new StepBlock[Shapes[s].NumSteps];
                for (int k = 0; k < Shapes[s].NumSteps; k++)
                {
                    /* Retrieve KitterSpeak information */
                    Shapes[s].KitterSpeak[k].StepType = (StepBlock.StepBlockStepTypes)br.ReadUInt16();
                    Shapes[s].KitterSpeak[k].Value = br.ReadInt16();
                    Shapes[s].KitterSpeak[k].CounterMax = br.ReadInt16();
                }
            }

            br.Close();
        }

        private void LoadFS2(string path)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.StreamReader(path).BaseStream);

            /* Assume/retrieve header information */
            Header = System.Text.Encoding.GetEncoding(1252).GetString(br.ReadBytes(4)); br.ReadByte();
            Version = Convert.ToInt32(System.Text.Encoding.GetEncoding(1252).GetString(br.ReadBytes(3)));
            NumShapes = br.ReadInt32();
            Generator = 0;
            Encryption = br.ReadInt32();
            if (Encryption > 0)
            {
                br.Close();
                throw new FurcadiaShapesException("The FS2 file (" + path + ") is encrypted and will not be opened.");
            }
            Reserved1 = 0;
            Reserved2 = 0;

            /* Loop through the shapes */
            Shapes = new Shape[NumShapes];
            for (int s = 0; s < NumShapes; s++)
            {
                Shapes[s].Flags = 0;
                Shapes[s].NumFrames = 1;
                Shapes[s].NumSteps = 0;

                Shapes[s].Frames = new Frame[1];
                Shapes[s].Frames[0].FrameFormat = Frame.FrameFormats.Format8Bit;
                Shapes[s].Frames[0].Width = (ushort)br.ReadByte();
                Shapes[s].Frames[0].Height = (ushort)br.ReadByte();
                Shapes[s].Frames[0].FramePos.X = (short)br.ReadSByte();
                Shapes[s].Frames[0].FramePos.Y = (short)br.ReadSByte();
                Shapes[s].Frames[0].FurrePos.X = (short)0;
                Shapes[s].Frames[0].FurrePos.Y = (short)0;
                Shapes[s].ShapeNum = br.ReadInt16();
                Shapes[s].Frames[0].ImageDataSize = (uint)(Shapes[s].Frames[0].Width * Shapes[s].Frames[0].Height);
                Shapes[s].Frames[0].ImageData = br.ReadBytes((int)Shapes[s].Frames[0].ImageDataSize);
            }

            br.Close();

            try
            {
                LoadFBJ(path);
            }
            catch
            {
                /* If it can't load the FBJ, oh well */
            }
        }

        private void LoadFSH(string path)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.StreamReader(path).BaseStream);

            /* Assume/retrieve header information */
            Header = "FSH1";
            Version = 1;
            NumShapes = (int)br.ReadUInt16();
            Generator = 0;
            Encryption = 0;
            Reserved1 = 0;
            Reserved2 = 0;

            /* Skip over chunk sizes */
            for (int s = 0; s < NumShapes; s++) br.ReadUInt16();

            /* Loop through the shapes */
            Shapes = new Shape[NumShapes];
            for (int s = 0; s < NumShapes; s++)
            {
                Shapes[s].Flags = 0;
                Shapes[s].ShapeNum = 0;
                Shapes[s].NumFrames = 1;
                Shapes[s].NumSteps = 0;

                Shapes[s].Frames = new Frame[1];
                Shapes[s].Frames[0].FrameFormat = Frame.FrameFormats.Format8Bit;
                Shapes[s].Frames[0].Width = (ushort)br.ReadByte();
                Shapes[s].Frames[0].Height = (ushort)br.ReadByte();
                Shapes[s].Frames[0].FramePos.X = (short)br.ReadSByte();
                Shapes[s].Frames[0].FramePos.Y = (short)br.ReadSByte();
                Shapes[s].Frames[0].FurrePos.X = (short)0;
                Shapes[s].Frames[0].FurrePos.Y = (short)0;
                Shapes[s].Frames[0].ImageDataSize = (uint)(Shapes[s].Frames[0].Width * Shapes[s].Frames[0].Height);
                Shapes[s].Frames[0].ImageData = br.ReadBytes((int)Shapes[s].Frames[0].ImageDataSize);
            }

            br.Close();

            LoadFBJ(path);
        }

        #endregion Private Methods
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapesException'

    public class FurcadiaShapesException : System.ApplicationException
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapesException'
    {
        #region Public Constructors

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapesException.FurcadiaShapesException(string)'

        public FurcadiaShapesException(string msg)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaShapesException.FurcadiaShapesException(string)'
            : base(msg)
        {
        }

        #endregion Public Constructors
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Palette'

    public class Palette
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Palette'
    {
        #region Public Fields

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Palette.Colors'
        public readonly Color[] Colors;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Palette.Colors'

        #endregion Public Fields

        #region Private Fields

        private static Paths Furcpath;

        #endregion Private Fields

        #region Public Constructors

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Palette.Palette(string)'

        public Palette(string pcxPath)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Palette.Palette(string)'
        {
            Furcpath = new Paths();

            byte[] colors;

            /* Open the PCX and rip out the palette (with verification!) */
            System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.StreamReader(pcxPath).BaseStream);
            br.BaseStream.Seek(-769, System.IO.SeekOrigin.End);
            if (br.ReadByte() != 12)
            {
                br.Close();
                throw new FurcadiaPaletteException("A 256-color palette was not found in file (" + pcxPath + ").");
            }
            colors = br.ReadBytes(768);
            br.Close();

            /* Reorganize the data into a Color class array */
            Color[] cols = new Color[256];
            cols[0] = System.Drawing.Color.FromArgb(0, 0, 0, 0);
            for (int i = 1; i < 256; i++)
            {
                cols[i] = Color.FromArgb(colors[i * 3], colors[i * 3 + 1], colors[i * 3 + 2]);
            }
            Colors = cols;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Palette.Palette(string, string)'

        public Palette(string pcxPath, string fpath)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Palette.Palette(string, string)'
        {
            Furcpath = new Paths(fpath);

            byte[] colors;

            /* Open the PCX and rip out the palette (with verification!) */
            System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.StreamReader(pcxPath).BaseStream);
            br.BaseStream.Seek(-769, System.IO.SeekOrigin.End);
            if (br.ReadByte() != 12)
            {
                br.Close();
                throw new FurcadiaPaletteException("A 256-color palette was not found in file (" + pcxPath + ").");
            }
            colors = br.ReadBytes(768);
            br.Close();

            /* Reorganize the data into a Color class array */
            Color[] cols = new Color[256];
            cols[0] = System.Drawing.Color.FromArgb(0, 0, 0, 0);
            for (int i = 1; i < 256; i++)
            {
                cols[i] = Color.FromArgb(colors[i * 3], colors[i * 3 + 1], colors[i * 3 + 2]);
            }
            Colors = cols;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// title261.pcx
        /// </summary>
        public static Palette Default
        {
            get { return new Palette(Furcpath.GetDefaultPatchPath() + "/title261.pcx"); }
        }

        #endregion Public Properties
    }

    /// <summary>
    /// Remap colors
    /// </summary>
    public static class Remapper
    {
        #region Remaps

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Remapper.BadgeRemap'

        public static byte[] BadgeRemap = new byte[] {
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Remapper.BadgeRemap'
            14, 12, 10, 240, 135, 39, 37, 35, 27, 21, 55, 51, 49, 241, 56, 70,
            75, 73, 242, 244, 86, 85, 83, 245, 238, 230, 228, 226, 242, 237, 133, 131,
            129, 99, 235, 111, 110, 109, 115, 175
        };

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Remapper.BootRemap'

        public static byte[][] BootRemap = new byte[][] {
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Remapper.BootRemap'
            new byte[] { 10, 10, 11, 12, 13, 14, 15, 15 },
            new byte[] { 16, 17, 18, 19, 20, 21, 22, 23 },
            new byte[] { 24, 25, 26, 27, 28, 29, 30, 31 },
            new byte[] { 32, 33, 34, 35, 36, 37, 38, 39 },
            new byte[] { 40, 41, 42, 43, 44, 45, 46, 47 },
            new byte[] { 48, 49, 50, 51, 52, 53, 54, 55 },
            new byte[] { 56, 57, 58, 59, 60, 61, 62, 63 },
            new byte[] { 64, 65, 66, 67, 68, 69, 70, 71 },
            new byte[] { 72, 73, 74, 75, 76, 77, 78, 79 },
            new byte[] { 80, 81, 82, 83, 84, 85, 86, 87 },
            new byte[] { 88, 89, 90, 91, 92, 93, 94, 95 },
            new byte[] { 96, 97, 98, 99, 100, 101, 102, 103 },
            new byte[] { 104, 105, 106, 107, 108, 109, 110, 111 },
            new byte[] { 112, 113, 114, 115, 116, 117, 118, 119 },
            new byte[] { 120, 121, 122, 123, 124, 125, 126, 127 },
            new byte[] { 128, 129, 130, 131, 132, 133, 134, 135 },
            new byte[] { 136, 137, 138, 139, 140, 141, 142, 143 },
            new byte[] { 144, 145, 146, 147, 148, 149, 150, 151 },
            new byte[] { 152, 153, 154, 155, 156, 157, 158, 159 },
            new byte[] { 160, 161, 162, 163, 164, 165, 166, 167 },
            new byte[] { 168, 169, 170, 171, 172, 173, 174, 175 },
            new byte[] { 176, 177, 178, 179, 180, 181, 182, 183 },
            new byte[] { 184, 185, 186, 187, 188, 189, 190, 191 },
            new byte[] { 192, 193, 194, 195, 196, 197, 198, 199 },
            new byte[] { 200, 201, 202, 203, 204, 205, 206, 207 },
            new byte[] { 120, 209, 210, 211, 212, 213, 214, 215 },
            new byte[] { 216, 217, 218, 219, 220, 221, 222, 223 },
            new byte[] { 224, 225, 226, 227, 228, 229, 230, 231 },
            new byte[] { 234, 234, 235, 236, 237, 238, 239, 239 },
            new byte[] { 88, 88, 89, 90, 90, 91, 91, 92 },
        };

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Remapper.BracersRemap'

        public static byte[][] BracersRemap = new byte[][] {
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Remapper.BracersRemap'
            new byte[] { 10, 10, 11, 12, 13, 14, 15, 15 },
            new byte[] { 16, 17, 18, 19, 20, 21, 22, 23 },
            new byte[] { 24, 25, 26, 27, 28, 29, 30, 31 },
            new byte[] { 32, 33, 34, 35, 36, 37, 38, 39 },
            new byte[] { 40, 41, 42, 43, 44, 45, 46, 47 },
            new byte[] { 48, 49, 50, 51, 52, 53, 54, 55 },
            new byte[] { 56, 57, 58, 59, 60, 61, 62, 63 },
            new byte[] { 64, 65, 66, 67, 68, 69, 70, 71 },
            new byte[] { 72, 73, 74, 75, 76, 77, 78, 79 },
            new byte[] { 80, 81, 82, 83, 84, 85, 86, 87 },
            new byte[] { 88, 89, 90, 91, 92, 93, 94, 95 },
            new byte[] { 96, 97, 98, 99, 100, 101, 102, 103 },
            new byte[] { 104, 105, 106, 107, 108, 109, 110, 111 },
            new byte[] { 112, 113, 114, 115, 116, 117, 118, 119 },
            new byte[] { 120, 121, 122, 123, 124, 125, 126, 127 },
            new byte[] { 128, 129, 130, 131, 132, 133, 134, 135 },
            new byte[] { 136, 137, 138, 139, 140, 141, 142, 143 },
            new byte[] { 144, 145, 146, 147, 148, 149, 150, 151 },
            new byte[] { 152, 153, 154, 155, 156, 157, 158, 159 },
            new byte[] { 160, 161, 162, 163, 164, 165, 166, 167 },
            new byte[] { 168, 169, 170, 171, 172, 173, 174, 175 },
            new byte[] { 176, 177, 178, 179, 180, 181, 182, 183 },
            new byte[] { 184, 185, 186, 187, 188, 189, 190, 191 },
            new byte[] { 192, 193, 194, 195, 196, 197, 198, 199 },
            new byte[] { 200, 201, 202, 203, 204, 205, 206, 207 },
            new byte[] { 120, 209, 210, 211, 212, 213, 214, 215 },
            new byte[] { 216, 217, 218, 219, 220, 221, 222, 223 },
            new byte[] { 224, 225, 226, 227, 228, 229, 230, 231 },
            new byte[] { 234, 234, 235, 236, 237, 238, 239, 239 },
            new byte[] { 88, 88, 89, 90, 90, 91, 91, 92 },
        };

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Remapper.CapeRemap'

        public static byte[][] CapeRemap = new byte[][] {
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Remapper.CapeRemap'
            new byte[] { 10, 10, 11, 12, 13, 14, 15, 15 },
            new byte[] { 16, 17, 18, 19, 20, 21, 22, 23 },
            new byte[] { 24, 25, 26, 27, 28, 29, 30, 31 },
            new byte[] { 32, 33, 34, 35, 36, 37, 38, 39 },
            new byte[] { 40, 41, 42, 43, 44, 45, 46, 47 },
            new byte[] { 48, 49, 50, 51, 52, 53, 54, 55 },
            new byte[] { 56, 57, 58, 59, 60, 61, 62, 63 },
            new byte[] { 64, 65, 66, 67, 68, 69, 70, 71 },
            new byte[] { 72, 73, 74, 75, 76, 77, 78, 79 },
            new byte[] { 80, 81, 82, 83, 84, 85, 86, 87 },
            new byte[] { 88, 89, 90, 91, 92, 93, 94, 95 },
            new byte[] { 96, 97, 98, 99, 100, 101, 102, 103 },
            new byte[] { 104, 105, 106, 107, 108, 109, 110, 111 },
            new byte[] { 112, 113, 114, 115, 116, 117, 118, 119 },
            new byte[] { 120, 121, 122, 123, 124, 125, 126, 127 },
            new byte[] { 128, 129, 130, 131, 132, 133, 134, 135 },
            new byte[] { 136, 137, 138, 139, 140, 141, 142, 143 },
            new byte[] { 144, 145, 146, 147, 148, 149, 150, 151 },
            new byte[] { 152, 153, 154, 155, 156, 157, 158, 159 },
            new byte[] { 160, 161, 162, 163, 164, 165, 166, 167 },
            new byte[] { 168, 169, 170, 171, 172, 173, 174, 175 },
            new byte[] { 176, 177, 178, 179, 180, 181, 182, 183 },
            new byte[] { 184, 185, 186, 187, 188, 189, 190, 191 },
            new byte[] { 192, 193, 194, 195, 196, 197, 198, 199 },
            new byte[] { 200, 201, 202, 203, 204, 205, 206, 207 },
            new byte[] { 120, 209, 210, 211, 212, 213, 214, 215 },
            new byte[] { 216, 217, 218, 219, 220, 221, 222, 223 },
            new byte[] { 224, 225, 226, 227, 228, 229, 230, 231 },
            new byte[] { 234, 234, 235, 236, 237, 238, 239, 239 },
            new byte[] { 88, 88, 89, 90, 90, 91, 91, 92 },
        };

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Remapper.EyeRemap'

        public static byte[] EyeRemap = new byte[] {
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Remapper.EyeRemap'
            23, 37, 50, 52, 67, 83, 86, 110, 185, 234, 236, 238, 243, 244, 245, 150,
            153, 156, 207, 129, 10, 159, 90, 230, 240, 241, 242, 202, 127, 172
        };

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Remapper.FurRemap'

        public static byte[][] FurRemap = new byte[][] {
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Remapper.FurRemap'
            new byte[] { 199, 200, 201, 202, 203, 204, 205, 206 },
            new byte[] { 89, 90, 91, 92, 93, 94, 95, 174 },
            new byte[] { 204, 204, 205, 205, 206, 206, 207, 207 },
            new byte[] { 216, 217, 218, 219, 220, 221, 222, 223 },
            new byte[] { 184, 185, 186, 187, 188, 189, 190, 191 },
            new byte[] { 152, 153, 154, 155, 156, 157, 158, 159 },
            new byte[] { 104, 105, 106, 107, 108, 109, 110, 111 },
            new byte[] { 96, 97, 98, 99, 100, 101, 102, 103 },
            new byte[] { 32, 33, 34, 35, 36, 37, 38, 39 },
            new byte[] { 16, 17, 18, 19, 20, 21, 22, 23 },
            new byte[] { 40, 42, 44, 47, 25, 28, 153, 152 },
            new byte[] { 146, 147, 149, 150, 151, 152, 153, 155 },
            new byte[] { 33, 34, 35, 36, 37, 12, 38, 20 },
            new byte[] { 177, 178, 179, 180, 181, 182, 148, 150 },
            new byte[] { 48, 49, 50, 51, 52, 53, 54, 55 },
            new byte[] { 72, 73, 74, 75, 76, 77, 78, 79 },
            new byte[] { 128, 129, 130, 131, 132, 133, 134, 135 },
            new byte[] { 144, 145, 146, 148, 150, 152, 153, 154 },
            new byte[] { 80, 81, 82, 83, 84, 85, 86, 87 },
            new byte[] { 224, 225, 226, 227, 228, 229, 230, 231 },
            new byte[] { 10, 10, 11, 11, 12, 13, 14, 15 },
            new byte[] { 120, 192, 209, 193, 194, 213, 216, 219 },
            new byte[] { 162, 164, 166, 168, 170, 171, 173, 175 },
            new byte[] { 195, 196, 197, 198, 199, 200, 201, 202 },
            new byte[] { 232, 233, 234, 235, 236, 237, 238, 239 },
        };

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Remapper.HairRemap'

        public static byte[][] HairRemap = new byte[][] {
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Remapper.HairRemap'
            new byte[] { 12, 13, 14, 15, 159, 159, 159, 159 },
            new byte[] { 10, 11, 12, 13, 14, 14, 14, 14 },
            new byte[] { 10, 10, 10, 11, 12, 12, 12, 12 },
            new byte[] { 34, 35, 240, 37, 38, 38, 38, 38 },
            new byte[] { 133, 134, 135, 158, 159, 159, 159, 159 },
            new byte[] { 37, 38, 39, 155, 156, 156, 156, 156 },
            new byte[] { 35, 36, 37, 38, 39, 39, 39, 39 },
            new byte[] { 33, 34, 35, 36, 37, 37, 37, 37 },
            new byte[] { 24, 26, 27, 154, 156, 156, 156, 156 },
            new byte[] { 19, 20, 21, 22, 23, 23, 23, 23 },
            new byte[] { 53, 54, 55, 61, 62, 62, 62, 62 },
            new byte[] { 49, 50, 51, 52, 53, 53, 53, 53 },
            new byte[] { 48, 49, 49, 50, 52, 52, 52, 52 },
            new byte[] { 48, 49, 241, 52, 53, 53, 53, 53 },
            new byte[] { 141, 56, 56, 57, 58, 58, 58, 58 },
            new byte[] { 68, 69, 70, 71, 79, 79, 79, 79 },
            new byte[] { 73, 74, 75, 76, 77, 77, 77, 77 },
            new byte[] { 64, 72, 73, 74, 75, 75, 75, 75 },
            new byte[] { 64, 64, 242, 66, 67, 67, 67, 67 },
            new byte[] { 244, 244, 244, 84, 85, 85, 85, 85 },
            new byte[] { 84, 85, 86, 87, 175, 175, 175, 175 },
            new byte[] { 83, 84, 85, 86, 87, 87, 87, 87 },
            new byte[] { 81, 82, 83, 84, 85, 85, 85, 85 },
            new byte[] { 234, 245, 245, 236, 237, 237, 237, 237 },
            new byte[] { 236, 237, 238, 239, 239, 239, 239, 239 },
            new byte[] { 228, 229, 230, 205, 206, 206, 206, 206 },
            new byte[] { 226, 227, 228, 229, 230, 230, 230, 230 },
            new byte[] { 224, 225, 226, 227, 228, 228, 228, 228 },
            new byte[] { 137, 242, 242, 67, 69, 69, 69, 69 },
            new byte[] { 235, 236, 237, 238, 239, 239, 239, 239 },
            new byte[] { 131, 132, 133, 134, 135, 135, 135, 135 },
            new byte[] { 129, 130, 131, 132, 133, 133, 133, 133 },
            new byte[] { 32, 128, 129, 130, 131, 131, 131, 131 },
            new byte[] { 97, 98, 99, 100, 101, 101, 101, 101 },
            new byte[] { 234, 234, 235, 236, 236, 236, 236, 236 },
            new byte[] { 109, 110, 111, 204, 205, 205, 205, 205 },
            new byte[] { 108, 109, 110, 111, 204, 204, 204, 204 },
            new byte[] { 107, 108, 109, 110, 111, 111, 111, 111 },
            new byte[] { 113, 114, 115, 116, 118, 118, 118, 118 },
            new byte[] { 173, 174, 175, 175, 207, 207, 207, 207 },
            new byte[] { 200, 202, 204, 205, 207, 207, 207, 207 },
            new byte[] { 195, 197, 198, 200, 202, 202, 202, 202 },
            new byte[] { 120, 209, 194, 216, 219, 219, 219, 219 },
            new byte[] { 144, 146, 150, 153, 154, 154, 154, 154 },
            new byte[] { 120, 122, 124, 126, 127, 127, 127, 127 },
        };

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Remapper.MarkingsRemap'

        public static byte[][] MarkingsRemap = new byte[][] {
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Remapper.MarkingsRemap'
            new byte[] { 199, 200, 201, 202, 203, 204, 205, 206 },
            new byte[] { 89, 90, 91, 92, 93, 94, 95, 174 },
            new byte[] { 204, 204, 205, 205, 206, 206, 207, 207 },
            new byte[] { 216, 217, 218, 219, 220, 221, 222, 223 },
            new byte[] { 184, 185, 186, 187, 188, 189, 190, 191 },
            new byte[] { 152, 153, 154, 155, 156, 157, 158, 159 },
            new byte[] { 104, 105, 106, 107, 108, 109, 110, 111 },
            new byte[] { 96, 97, 98, 99, 100, 101, 102, 103 },
            new byte[] { 32, 33, 34, 35, 36, 37, 38, 39 },
            new byte[] { 16, 17, 18, 19, 20, 21, 22, 23 },
            new byte[] { 40, 42, 44, 47, 25, 28, 153, 152 },
            new byte[] { 146, 147, 149, 150, 151, 152, 153, 155 },
            new byte[] { 33, 34, 35, 36, 37, 12, 38, 20 },
            new byte[] { 177, 178, 179, 180, 181, 182, 148, 150 },
            new byte[] { 48, 49, 50, 51, 52, 53, 54, 55 },
            new byte[] { 72, 73, 74, 75, 76, 77, 78, 79 },
            new byte[] { 128, 129, 130, 131, 132, 133, 134, 135 },
            new byte[] { 144, 145, 146, 148, 150, 152, 153, 154 },
            new byte[] { 80, 81, 82, 83, 84, 85, 86, 87 },
            new byte[] { 224, 225, 226, 227, 228, 229, 230, 231 },
            new byte[] { 10, 10, 11, 11, 12, 13, 14, 15 },
            new byte[] { 120, 192, 209, 193, 194, 213, 216, 219 },
            new byte[] { 162, 164, 166, 168, 170, 171, 173, 175 },
            new byte[] { 195, 196, 197, 198, 199, 200, 201, 202 },
            new byte[] { 232, 233, 234, 235, 236, 237, 238, 239 },
        };

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Remapper.TrousersRemap'

        public static byte[][] TrousersRemap = new byte[][] {
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Remapper.TrousersRemap'
            new byte[] { 10, 10, 11, 12, 13, 14, 15, 15 },
            new byte[] { 16, 17, 18, 19, 20, 21, 22, 23 },
            new byte[] { 24, 25, 26, 27, 28, 29, 30, 31 },
            new byte[] { 32, 33, 34, 35, 36, 37, 38, 39 },
            new byte[] { 40, 41, 42, 43, 44, 45, 46, 47 },
            new byte[] { 48, 49, 50, 51, 52, 53, 54, 55 },
            new byte[] { 56, 57, 58, 59, 60, 61, 62, 63 },
            new byte[] { 64, 65, 66, 67, 68, 69, 70, 71 },
            new byte[] { 72, 73, 74, 75, 76, 77, 78, 79 },
            new byte[] { 80, 81, 82, 83, 84, 85, 86, 87 },
            new byte[] { 88, 89, 90, 91, 92, 93, 94, 95 },
            new byte[] { 96, 97, 98, 99, 100, 101, 102, 103 },
            new byte[] { 104, 105, 106, 107, 108, 109, 110, 111 },
            new byte[] { 112, 113, 114, 115, 116, 117, 118, 119 },
            new byte[] { 120, 121, 122, 123, 124, 125, 126, 127 },
            new byte[] { 128, 129, 130, 131, 132, 133, 134, 135 },
            new byte[] { 136, 137, 138, 139, 140, 141, 142, 143 },
            new byte[] { 144, 145, 146, 147, 148, 149, 150, 151 },
            new byte[] { 152, 153, 154, 155, 156, 157, 158, 159 },
            new byte[] { 160, 161, 162, 163, 164, 165, 166, 167 },
            new byte[] { 168, 169, 170, 171, 172, 173, 174, 175 },
            new byte[] { 176, 177, 178, 179, 180, 181, 182, 183 },
            new byte[] { 184, 185, 186, 187, 188, 189, 190, 191 },
            new byte[] { 192, 193, 194, 195, 196, 197, 198, 199 },
            new byte[] { 200, 201, 202, 203, 204, 205, 206, 207 },
            new byte[] { 120, 209, 210, 211, 212, 213, 214, 215 },
            new byte[] { 216, 217, 218, 219, 220, 221, 222, 223 },
            new byte[] { 224, 225, 226, 227, 228, 229, 230, 231 },
            new byte[] { 234, 234, 235, 236, 237, 238, 239, 239 },
            new byte[] { 88, 88, 89, 90, 90, 91, 91, 92 },
        };

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Remapper.VestRemap'

        public static byte[][] VestRemap = new byte[][] {
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Remapper.VestRemap'
            new byte[] { 10, 10, 11, 12, 13, 14, 15, 15 },
            new byte[] { 16, 17, 18, 19, 20, 21, 22, 23 },
            new byte[] { 24, 25, 26, 27, 28, 29, 30, 31 },
            new byte[] { 32, 33, 34, 35, 36, 37, 38, 39 },
            new byte[] { 40, 41, 42, 43, 44, 45, 46, 47 },
            new byte[] { 48, 49, 50, 51, 52, 53, 54, 55 },
            new byte[] { 56, 57, 58, 59, 60, 61, 62, 63 },
            new byte[] { 64, 65, 66, 67, 68, 69, 70, 71 },
            new byte[] { 72, 73, 74, 75, 76, 77, 78, 79 },
            new byte[] { 80, 81, 82, 83, 84, 85, 86, 87 },
            new byte[] { 88, 89, 90, 91, 92, 93, 94, 95 },
            new byte[] { 96, 97, 98, 99, 100, 101, 102, 103 },
            new byte[] { 104, 105, 106, 107, 108, 109, 110, 111 },
            new byte[] { 112, 113, 114, 115, 116, 117, 118, 119 },
            new byte[] { 120, 121, 122, 123, 124, 125, 126, 127 },
            new byte[] { 128, 129, 130, 131, 132, 133, 134, 135 },
            new byte[] { 136, 137, 138, 139, 140, 141, 142, 143 },
            new byte[] { 144, 145, 146, 147, 148, 149, 150, 151 },
            new byte[] { 152, 153, 154, 155, 156, 157, 158, 159 },
            new byte[] { 160, 161, 162, 163, 164, 165, 166, 167 },
            new byte[] { 168, 169, 170, 171, 172, 173, 174, 175 },
            new byte[] { 176, 177, 178, 179, 180, 181, 182, 183 },
            new byte[] { 184, 185, 186, 187, 188, 189, 190, 191 },
            new byte[] { 192, 193, 194, 195, 196, 197, 198, 199 },
            new byte[] { 200, 201, 202, 203, 204, 205, 206, 207 },
            new byte[] { 120, 209, 210, 211, 212, 213, 214, 215 },
            new byte[] { 216, 217, 218, 219, 220, 221, 222, 223 },
            new byte[] { 224, 225, 226, 227, 228, 229, 230, 231 },
            new byte[] { 234, 234, 235, 236, 237, 238, 239, 239 },
            new byte[] { 88, 88, 89, 90, 90, 91, 91, 92 },
        };

        #endregion Remaps

        #region Public Fields

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Remapper.Palette'
        public static Color[] Palette;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Remapper.Palette'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Remapper.PalLoaded'
        public static bool PalLoaded = false;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Remapper.PalLoaded'

        #endregion Public Fields

        #region Public Methods

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Remapper.Remap(byte[], int, int, string, int)'

        public static Bitmap Remap(byte[] source, int width, int height, string colourcode, int highlight)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Remapper.Remap(byte[], int, int, string, int)'
        {
            byte[] RawPal = Properties.Resources.furc;
            Color[] ConvPal = new Color[256];
            int FilePos = 0;

            for (int i = 0; i < 256; i++)
            {
                ConvPal[i] = Color.FromArgb(RawPal[FilePos], RawPal[FilePos + 1], RawPal[FilePos + 2]);
                FilePos += 3;
            }

            ConvPal[0] = Color.Transparent;
            Palette = ConvPal;

            byte[] RemappedPal = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                RemappedPal[i] = (byte)i;
            }

            // try/catch block will allow us to die peacefully if an invalid
            // colour is found
            try
            {
                // first off parse the colour code into its components
                int FurColour = colourcode[1] - 35;
                int MarkingsColour = colourcode[2] - 35;
                int HairColour = colourcode[3] - 35;
                int EyeColour = colourcode[4] - 35;
                int BadgeColour = colourcode[5] - 35;
                int VestColour = colourcode[6] - 35;
                int BracerColour = colourcode[7] - 35;
                int CapeColour = colourcode[8] - 35;
                int BootColour = colourcode[9] - 35;
                int TrousersColour = colourcode[10] - 35;

                // go through the palette and remap specific bits
                for (int ColIdx = 199; ColIdx <= 206; ColIdx++)
                {
                    RemappedPal[ColIdx] = FurRemap[FurColour][ColIdx - 199];
                }

                for (int ColIdx = 136; ColIdx <= 143; ColIdx++)
                {
                    RemappedPal[ColIdx] = MarkingsRemap[MarkingsColour][ColIdx - 136];
                }

                for (int ColIdx = 128; ColIdx <= 135; ColIdx++)
                {
                    RemappedPal[ColIdx] = HairRemap[HairColour][ColIdx - 128];
                }

                RemappedPal[50] = EyeRemap[EyeColour];
                RemappedPal[11] = BadgeRemap[BadgeColour];

                for (int ColIdx = 72; ColIdx <= 79; ColIdx++)
                {
                    RemappedPal[ColIdx] = VestRemap[VestColour][ColIdx - 72];
                }

                for (int ColIdx = 80; ColIdx <= 87; ColIdx++)
                {
                    RemappedPal[ColIdx] = BracersRemap[BracerColour][ColIdx - 80];
                }

                for (int ColIdx = 32; ColIdx <= 39; ColIdx++)
                {
                    RemappedPal[ColIdx] = CapeRemap[CapeColour][ColIdx - 32];
                }

                for (int ColIdx = 16; ColIdx <= 23; ColIdx++)
                {
                    RemappedPal[ColIdx] = BootRemap[BootColour][ColIdx - 16];
                }

                for (int ColIdx = 224; ColIdx <= 231; ColIdx++)
                {
                    RemappedPal[ColIdx] = TrousersRemap[TrousersColour][ColIdx - 224];
                }

                RemappedPal[10] = 206; // shadows
            }
            catch
            {
            }

            // now convert it to a new bitmap
            //Bitmap Source = source.Clone(new Rectangle(0, 0, source.Width, source.Height), PixelFormat.Format8bppIndexed);
            Bitmap Output = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            //BitmapData SourceBitmapData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData DestBitmapData = Output.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            //byte[] SourceData = new byte[SourceBitmapData.Stride * source.Height];
            //System.Runtime.InteropServices.Marshal.Copy(SourceBitmapData.Scan0, SourceData, 0, SourceData.Length);

            byte[] DestData = new byte[DestBitmapData.Stride * height];
            int Pos = 0;

            for (int Y = height - 1; Y >= 0; Y--)
            {
                for (int X = 0; X < width; X++)
                {
                    DestData[(DestBitmapData.Stride * Y) + X] = RemappedPal[source[Pos]];
                    Pos++;
                }
            }

            //source.UnlockBits(SourceBitmapData);

            System.Runtime.InteropServices.Marshal.Copy(DestData, 0, DestBitmapData.Scan0, DestData.Length);
            Output.UnlockBits(DestBitmapData);

            ColorPalette ThisPal = Output.Palette;
            Palette.CopyTo(ThisPal.Entries, 0);
            Output.Palette = ThisPal;

            if (highlight > -1)
            {
                Bitmap Orig = Output.Clone(new Rectangle(0, 0, width, height), PixelFormat.Format32bppArgb);
                Output = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(Output);
                ImageAttributes attr = new ImageAttributes();
                ColorMatrix mtx = new ColorMatrix
                {
                    Matrix33 = 0.25F
                };
                attr.SetColorMatrix(mtx, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.DrawImage(Orig,
                    new Rectangle(0, 0, width, height),
                    0, 0, width, height,
                    GraphicsUnit.Pixel, attr);
                g.Dispose();

                BitmapData modifyBD = Output.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                Int32[] data = new Int32[modifyBD.Stride * modifyBD.Height / 4];
                System.Runtime.InteropServices.Marshal.Copy(modifyBD.Scan0, data, 0, data.Length);
                int pos = 0;

                int bound1 = 0, bound2 = 0;

                if (highlight == 0)
                {
                    bound1 = 199; bound2 = 206;
                }
                else if (highlight == 1)
                {
                    bound1 = 136; bound2 = 143;
                }
                else if (highlight == 2)
                {
                    bound1 = 128; bound2 = 135;
                }
                else if (highlight == 3)
                {
                    bound1 = 50; bound2 = 50;
                }
                else if (highlight == 4)
                {
                    bound1 = 11; bound2 = 11;
                }
                else if (highlight == 5)
                {
                    bound1 = 72; bound2 = 79;
                }
                else if (highlight == 6)
                {
                    bound1 = 80; bound2 = 87;
                }
                else if (highlight == 7)
                {
                    bound1 = 32; bound2 = 39;
                }
                else if (highlight == 8)
                {
                    bound1 = 16; bound2 = 23;
                }
                else if (highlight == 9)
                {
                    bound1 = 224; bound2 = 231;
                }

                for (int y = height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte piece = source[pos];
                        if (piece >= bound1 && piece <= bound2)
                        {
                            int index = (modifyBD.Stride * y / 4) + x;
                            int bit = (bound2 - piece) * 20;
                            data[index] = unchecked((int)0xFF0000FF | (bit << 8) | (bit << 16));
                        }
                        else if (piece == 207)
                        {
                            int index = (modifyBD.Stride * y / 4) + x;
                            data[index] = (int)(((uint)data[index]) | 0x80000000);
                        }

                        pos++;
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(data, 0, modifyBD.Scan0, data.Length);
                Output.UnlockBits(modifyBD);
            }

            return Output;
        }

        #endregion Public Methods
    }
}