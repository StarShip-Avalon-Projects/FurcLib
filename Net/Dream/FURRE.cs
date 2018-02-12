using Furcadia.Drawing;
using Furcadia.Movement;
using System;
using static Furcadia.Net.DreamInfo.Avatar;

namespace Furcadia.Net.DreamInfo
{
    /// <summary>
    /// Furre Class Interface
    /// </summary>
    public interface IFurre
    {
        #region Public Properties

        /// <summary>
        /// Implements the FurreID or unique furre identifyer
        /// </summary>
        int FurreID { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string Message { get; set; }

        /// <summary>
        /// implements the Furre;s Name Property
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// implements the Furre;s Name Property
        /// </summary>
        string ShortName { get; }

        #endregion Public Properties
    }

    /// <summary>
    /// Class for Proxies and bots to use Furrre Data provided by the game server.
    /// </summary>
    [Serializable]
    public class Furre : IFurre
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
        private uint _PawObjectCurrent;
        private uint _PawObjectOld;
        private string _tag;
        private bool _Visible;
        private bool _WasVisible;
        private av_DIR direction;
        private FurrePosition Location;
        private string message;
        private string name;
        private FurrePose pose;
        private FurrePosition SourceLocation;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Furre"/> class.
        /// </summary>
        public Furre()
        {
            _Color = new ColorString();
            Location = new FurrePosition();
            LastPosition = new FurrePosition();
            _LastStat = -1;
            name = "Unknown";
            _ID = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Furre"/> class.
        /// </summary>
        /// <param name="FurreID">The furre identifier.</param>
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
        /// Initializes a new instance of the <see cref="Furre"/> class.
        /// </summary>
        /// <param name="FurreID">The furre identifier.</param>
        /// <param name="Name">The name.</param>
        public Furre(int FurreID, string Name) : this()
        {
            _ID = FurreID;
            name = Name;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Away from keyboard time
        /// </summary>
        public int AfkTime
        {
            get { return _AFK; }
            set { _AFK = value; }
        }

        /// <summary>
        /// Gets or sets the beekin badge.
        /// </summary>
        /// <value>
        /// The beekin badge.
        /// </value>
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
        /// Gets or sets the direction.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        public av_DIR Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        /// <summary>
        /// Gets or sets the floor object current.
        /// </summary>
        /// <value>
        /// The floor object current.
        /// </value>
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
        /// Gets or sets the floor object old.
        /// </summary>
        /// <value>
        /// The floor object old.
        /// </value>
        [CLSCompliant(false)]
        public uint FloorObjectOld
        {
            get { return _FloorObjectOld; }
            set { _FloorObjectOld = value; }
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
        /// Furre ID
        /// </summary>
        public int FurreID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        /// <summary>
        /// Gets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        [Obsolete]
        public int Gender
        {
            get { return FurreColors.Gender; }
        }

        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public int Group
        {
            get { return _group; }
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
        /// Gets the last stat.
        /// </summary>
        /// <value>
        /// The last stat.
        /// </value>
        public int LastStat
        {
            get { return _LastStat; }
        }

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public int Level
        {
            get { return _level; }
        }

        /// <summary>
        /// Last Message Furre had
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
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
        /// Gets or sets the paw object current.
        /// </summary>
        /// <value>
        /// The paw object current.
        /// </value>
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
        /// Gets or sets the paw object old.
        /// </summary>
        /// <value>
        /// The paw object old.
        /// </value>
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
                return name.ToFurcadiaShortName();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Furre"/> is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
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
        /// Gets a value indicating whether [was visible].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [was visible]; otherwise, <c>false</c>.
        /// </value>
        public bool WasVisible
        {
            get { return _WasVisible; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Furre a, IFurre b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Furre a, IFurre b)
        {
            // If left hand side is null...
            if (a is null)
            {
                // ...and right hand side is null...
                if (b is null)
                {
                    //...both are null and are Equal.
                    return true;
                }

                // ...right hand side is not null, therefore not Equal.
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is IFurre fur)
            {
                return ShortName == fur.ShortName
                    || fur.FurreID == FurreID;
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ FurreID;
        }

        /// <summary>
        /// To the furcadia identifier.
        /// </summary>
        /// <returns></returns>
        public int ToFurcadiaID()
        {
            return _ID;
        }

        /// <summary>
        /// To the furcadia identifier.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public int ToFurcadiaID(Func<IFurre, int> format)
        {
            if (format != null)
                return format(this);
            return ToFurcadiaID();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format($"{FurreID} - {Name}");
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(Func<IFurre, string> format)
        {
            if (format != null)
                return format(this);
            return ToString();
        }

        #endregion Public Methods
    }
}