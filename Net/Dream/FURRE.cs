﻿using Furcadia.Drawing;
using Furcadia.Movement;
using System;
using static Furcadia.Net.Dream.Avatar;
using static Furcadia.Util;

namespace Furcadia.Net.Dream
{
    /// <summary>
    /// Class for Proxies and bots to use Furrre Data provided by the game server.
    /// </summary>
    public class Furre
    {
        #region Private Fields

        private int _AFK;
        private string _badge;

        /// <summary>
        /// v31c Colorcodes
        /// </summary>
        private ColorString _Color;

        private string _Desc;

        private uint _FloorObjectCurrent;
        private uint _FloorObjectOld;

        private int _group;
        private int _ID;
        private int _LastStat;
        private int _level;
        private string _Message;
        private string name;

        private uint _PawObjectCurrent;
        private uint _PawObjectOld;

        private string _tag;
        private bool _Visible;
        private bool _WasVisible;

        private FurrePosition Location;
        private FurrePosition SourceLocation;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        public Furre()
        {
            _Color = new ColorString();
            Location = new FurrePosition();
            LastPosition = new FurrePosition();
            _LastStat = -1;
            name = "Unknown";
        }

        /// <summary>
        /// </summary>
        /// <param name="FurreID">
        /// </param>
        public Furre(int FurreID) : this()
        {
            _ID = FurreID;

        }

        /// <summary>
        /// Furre Constructor with Name
        /// </summary>
        /// <param name="Name">
        /// </param>
        public Furre(string Name) : this()
        {
            name = Name;

        }

        /// <summary>
        /// </summary>
        /// <param name="FurreID">
        /// </param>
        /// <param name="Name">
        /// </param>
        public Furre(int FurreID, string Name) : this()
        {
            _ID = FurreID;
            name = Name;

        }

        #endregion Public Constructors

        #region Public Properties

        private av_DIR direction;

        private FurrePose pose;

        /// <summary>
        /// Away from keyboard time
        /// </summary>
        public int AfkTime
        {
            get { return _AFK; }
            set { _AFK = value; }
        }

        /// <summary>
        /// </summary>
        public string BeekinBadge
        {
            get { return _badge; }
            set
            {
                _badge = value;
                _tag = Badges.GetTag(_badge);
                _group = Badges.GetGroup(_badge);
                _level = Badges.GetLevel(_badge);
            }
        }

        /// <summary>
        /// Furcadia Color Code (v31c)
        /// </summary>
        public ColorString FurreColors
        {
            //TODO: Move section to a Costume Sub Class
            // Furcadia now supports Costumes through Online FurEd
            get { return _Color; }
            set
            {
                _Color = value;
            }
        }

        /// <summary>
        /// Furcadia Description
        /// </summary>
        public string FurreDescription
        {
            get { return _Desc; }
            set { _Desc = value; }
        }

        /// <summary>
        /// </summary>
        public av_DIR Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        [Obsolete]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Furre.DSSpecies'
        public int DSSpecies
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Furre.DSSpecies'
        {
            get { return FurreColors.Species; }
        }

        /// <summary>
        /// </summary>
        [CLSCompliant(false)]
        public uint FloorObjectCurrent
        {
            get { return _FloorObjectCurrent; }
            set
            {
                _FloorObjectOld = _FloorObjectCurrent;
                _FloorObjectCurrent = value;
            }
        }

        /// <summary>
        /// </summary>
        [CLSCompliant(false)]
        public uint FloorObjectOld
        {
            get { return _FloorObjectOld; }
            set { _FloorObjectOld = value; }
        }

        /// <summary>
        /// </summary>
        [Obsolete]
        public int Gender
        {
            get { return FurreColors.Gender; }
        }

        ///// <summary>
        ///// </summary>
        //public Avatar.Frame FrameInfo
        //{
        //    get
        //    {
        //        return Avatar.SpecNum(shape);
        //    }
        //}
        /// <summary>
        /// </summary>
        public int Group
        {
            get { return _group; }
        }

        /// <summary>
        /// Furre ID
        /// </summary>
        public int FurreID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        /// <summary>
        /// The Position the Furre Moved from
        /// </summary>
        public FurrePosition LastPosition
        {
            get { return SourceLocation; }
            set { SourceLocation = value; }
        }

        /// <summary>
        /// </summary>
        public int LastStat
        {
            get { return _LastStat; }
        }

        /// <summary>
        /// </summary>
        public int Level
        {
            get { return _level; }
        }

        /// <summary>
        /// Last Message Furre had
        /// </summary>
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        /// <summary>
        /// Furcadia Name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// </summary>
        [CLSCompliant(false)]
        public uint PawObjectCurrent
        {
            get { return _PawObjectCurrent; }
            set
            {
                _PawObjectOld = _PawObjectCurrent;
                _PawObjectCurrent = value;
            }
        }

        /// <summary>
        /// </summary>
        [CLSCompliant(false)]
        public uint PawObjectOld
        {
            get { return _PawObjectOld; }
            set { _PawObjectOld = value; }
        }

        /// <summary>
        /// Furre Pose
        /// </summary>
        public FurrePose Pose
        {
            get { return pose; }
            set { pose = value; }
        }

        /// <summary>
        /// Current position where the Furre is standing
        /// </summary>
        public FurrePosition Position
        {
            get
            {
                return Location;
            }
            set
            {
                Location = value;
            }
        }

        /// <summary>
        /// Furcadia Shortname format for Furre Name
        /// </summary>
        public string ShortName
        {
            get
            {
                return FurcadiaShortName(name);
            }
        }

        /// <summary>
        /// the X Position the Furre moved from
        /// <para>
        /// Obsolete. Use LasPosition as FurrePosition
        /// </para>
        /// </summary>
        [Obsolete("use LasPosition as FurrePosition", false)]
        public int SourceX
        {
            get { return LastPosition.X; }
            set { LastPosition.X = value; }
        }

        /// <summary>
        /// the Y Position the Furre moved from
        /// <para>
        /// Obsolete. Use LasPosition as FurrePosition
        /// </para>
        /// </summary>
        [Obsolete("use LasPosition as FurrePosition", false)]
        public int SourceY
        {
            get { return LastPosition.Y; }
            set { LastPosition.Y = value; }
        }

        /// <summary>
        /// </summary>
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                _WasVisible = _Visible;
                _Visible = value;
            }
        }

        /// <summary>
        /// </summary>
        public bool WasVisible
        {
            get { return _WasVisible; }
        }

        /// <summary>
        /// the X Position the Furre is currently standing at
        /// </summary>
        [Obsolete("use Position as FurrePosition", false)]
        public int X
        {
            get { return Position.X; }
            set { Position.X = value; }
        }

        /// <summary>
        /// the Y Position the Furre Standing At
        /// </summary>
        [Obsolete("use Position as FurrePosition", false)]
        public int Y
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="a">
        /// </param>
        /// <param name="b">
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator !=(Furre a, Furre b)
        {
            return !(a == b);
        }

        /// <summary>
        /// </summary>
        /// <param name="a">
        /// </param>
        /// <param name="b">
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator ==(Furre a, Furre b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null || (object)b == null)
                return false;
            if (string.IsNullOrEmpty(a.ShortName) || string.IsNullOrEmpty(a.ShortName))
                return a.FurreID == b.FurreID;
            return a.ShortName == b.ShortName;
        }

        /// <summary>
        /// </summary>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() == typeof(Furre))
            {
                Furre ob = (Furre)obj;
                if (!string.IsNullOrEmpty(ShortName))
                    return ob.ShortName == ShortName;
                return ob.FurreID == FurreID;
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public int ToFurcadiaID()
        {
            return _ID;
        }

        /// <summary>
        /// </summary>
        /// <param name="format">
        /// </param>
        /// <returns>
        /// </returns>
        public int ToFurcadiaID(Func<Furre, int> format)
        {
            if (format != null)
                return format(this);
            return ToFurcadiaID();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", FurreID, Name);
        }

        /// <summary>
        /// </summary>
        /// <param name="format">
        /// </param>
        /// <returns>
        /// </returns>
        public string ToString(Func<Furre, string> format)
        {
            if (format != null)
                return format(this);
            return this.ToString();
        }

        #endregion Public Methods
    }
}