using Furcadia.Drawing;
using Furcadia.Movement;
using Furcadia.Text;
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
        Base220 FurreID { get; set; }

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
    /// Class for Proxies and bots to use Furrre Data
    /// provided by the game server.
    /// </summary>
    /// <remarks>
    /// Furre ID is provided by the Furcadia game
    /// </remarks>
    [Serializable]
    public class Furre : IFurre
    {
        #region Private Fields

        private int afk;
        private string badge;
        private ColorString colorString;
        private string desc;
        private uint _FloorObjectCurrent;
        private uint _FloorObjectOld;
        private int _group;
        private Base220 iD;
        private int lastStat;
        private int level;
        private uint _PawObjectCurrent;
        private uint _PawObjectOld;
        private string _tag;
        private bool visible;
        private bool wasVisible;
        private AvatarDirection direction;
        private FurrePosition location;
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
            colorString = new ColorString();
            location = new FurrePosition();
            LastPosition = new FurrePosition();
            lastStat = -1;
            name = "Unknown";
            iD = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Furre"/> class.
        /// </summary>
        /// <param name="FurreID">The furre identifier.</param>
        public Furre(Base220 FurreID) : this()
        {
            iD = FurreID;
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
        public Furre(Base220 FurreID, string Name) : this()
        {
            iD = FurreID;
            name = Name;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Away from keyboard time
        /// </summary>
        public int AfkTime
        {
            get => afk;
            set => afk = value;
        }

        /// <summary>
        /// Gets or sets the beekin badge.
        /// </summary>
        /// <value>
        /// The beekin badge.
        /// </value>
        public string BeekinBadge
        {
            get => badge;
            set
            {
                badge = value;
                _tag = Badges.GetTag(badge);
                _group = Badges.GetGroup(badge);
                level = Badges.GetLevel(badge);
            }
        }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        public AvatarDirection Direction
        {
            get => direction;
            set => direction = value;
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
            get => _FloorObjectCurrent;
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
            get => _FloorObjectOld;
            set => _FloorObjectOld = value;
        }

        /// <summary>
        /// Furcadia Color Code (v31c)
        /// </summary>
        public ColorString FurreColors
        {
            //TODO: Move section to a Costume Sub Class
            // Furcadia now supports Costumes through Online FurEd
            get => colorString;
            set => colorString = value;
        }

        /// <summary>
        /// Furcadia Description
        /// </summary>
        public string FurreDescription
        {
            get => desc;
            set => desc = value;
        }

        /// <summary>
        /// Furre ID
        /// </summary>
        public Base220 FurreID
        {
            get => iD;
            set => iD = value;
        }

        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public int Group => _group;

        /// <summary>
        /// The Position the Furre Moved from
        /// </summary>
        public FurrePosition LastPosition
        {
            get => SourceLocation;
            set => SourceLocation = value;
        }

        /// <summary>
        /// Gets the last stat.
        /// </summary>
        /// <value>
        /// The last stat.
        /// </value>
        public int LastStat => lastStat;

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public int Level => level;

        /// <summary>
        /// Last Message Furre had
        /// </summary>
        public string Message
        {
            get => message;
            set => message = value;
        }

        /// <summary>
        /// Furcadia Name
        /// </summary>
        public string Name
        {
            get => name;
            set => name = value;
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
            get => _PawObjectCurrent;
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
            get => _PawObjectOld;
            set => _PawObjectOld = value;
        }

        /// <summary>
        /// Furre Pose
        /// </summary>
        public FurrePose Pose
        {
            get => pose;
            set => pose = value;
        }

        /// <summary>
        /// Current position where the Furre is standing
        /// </summary>
        public FurrePosition Location
        {
            get => location;
            set => location = value;
        }

        /// <summary>
        /// Furcadia Shortname format for Furre Name
        /// </summary>
        public string ShortName => name.ToFurcadiaShortName();

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Furre"/> is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        public bool Visible
        {
            get => visible;
            set
            {
                wasVisible = visible;
                visible = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [was visible].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [was visible]; otherwise, <c>false</c>.
        /// </value>
        public bool WasVisible => wasVisible;

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
            // If left hand side is null...
            if (a is null)
            {
                return b is null;
            }

            // Return true if the fields match:
            return !a.Equals(b);
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
                return b is null;
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
                return fur.ShortName == ShortName
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
            return ShortName.GetHashCode() ^ FurreID;
        }

        /// <summary>
        /// To the furcadia identifier.
        /// </summary>
        /// <returns></returns>
        public int ToFurcadiaID()
        {
            return iD;
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
            return $"{FurreID} - {Name}";
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