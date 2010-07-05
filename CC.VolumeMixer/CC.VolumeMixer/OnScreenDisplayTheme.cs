using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace CC.VolumeMixer
{
    public class OnScreenDisplayTheme: IEquatable<OnScreenDisplayTheme>
    {
        #region Constructor
        public OnScreenDisplayTheme()
        {
            
        }

        public OnScreenDisplayTheme(string name, Color foregroundColor) : this(name, foregroundColor, Colors.Black)
        {
            // Empty Constructor
        }
        
        public OnScreenDisplayTheme(string name, Color foregroundColor, Color dropShadowColor)
        {
            DropShadowColor = dropShadowColor;
            ForegroundColor = foregroundColor;
            Name = name;
        }
        #endregion

        #region Private Fields
        private static OnScreenDisplayTheme _custom;
        private static readonly object _lockObject = new object();
        #endregion

        #region Public Properties
        [XmlIgnore]
        public static OnScreenDisplayTheme Custom
        {
            get { return _custom; }
            private set
            {
                if (_custom == null)
                {
                    lock (_lockObject)
                    {
                        _custom = new OnScreenDisplayTheme("<Custom>", Colors.White) {IsCustomTheme = true};
                    }
                }
                _custom = value;
            }
        }

        [XmlIgnore]
        public Color DropShadowColor { get; private set; }

        [XmlIgnore]
        public Color ForegroundColor { get; private set; }

        [XmlIgnore]
        public bool IsCustomTheme { get; private set; }

        public string Name { get; set; }
        #endregion

        #region Public Methods
        public static bool operator ==(OnScreenDisplayTheme x, OnScreenDisplayTheme y)
        {
            if (null == x as object && null == y as object)
            {
                return true;
            }

            if (null == x as object || null == y as object)
            {
                return false;
            }

            return x.Equals(y);
        }

        public static bool operator !=(OnScreenDisplayTheme x, OnScreenDisplayTheme y)
        {
            if (null == x as object && null == y as object)
            {
                return false;
            }

            if (null == x as object || null == y as object)
            {
                return true;
            }

            return !x.Equals(y);
        }

        public bool Equals(OnScreenDisplayTheme other)
        {
            return other != null && Name.Equals(other.Name);
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}
