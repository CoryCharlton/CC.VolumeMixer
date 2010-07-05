using System;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using System.Xml.Serialization;
using CC.Utilities;

namespace CC.VolumeMixer
{
    public class Settings: INotifyPropertyChanged
    {
        #region Constructor
        static Settings()
        {
            Default = new Settings(true);
        }

        public Settings() : this(false)
        {
            // Empty Constructor
        }

        public Settings(bool loadSettings)
        {
            if (loadSettings)
            {
                Load();
            }
        }
        #endregion
        
        #region Public Constants
        public static readonly Color DefaultOnScreenDisplayDropShadowColor = Colors.Black;
        public static readonly FadeSpeed DefaultOnScreenDisplayFadeSpeed = FadeSpeed.Normal;
        public readonly static Color DefaultOnScreenDisplayForegroundColor = (Color)ColorConverter.ConvertFromString("#FF19A5C1");

        public const string FileName = "CC.VolumeMixer.Settings";

        public const string OnScreenDisplayDropShadowColorPropertyName = "OnScreenDisplayDropShadowColor";
        public const string OnScreenDisplayFadeSpeedPropertyName = "OnScreenDisplayFadeSpeed";
        public const string OnScreenDisplayForegroundBrushPropertyName = "OnScreenDisplayForegroundBrush";
        public const string OnScreenDisplayForegroundColorPropertyName = "OnScreenDisplayForegroundColor";
        public const string OnScreenDisplayThemePropertyName = "OnScreenDisplayTheme";
        #endregion
        
        #region Public Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Private Fields
        private Color _onScreenDisplayDropShadowColor;
        private FadeSpeed _onScreenDisplayFadeSpeed;
        private SolidColorBrush _onScreenDisplayForegroundBrush;
        private Color _onScreenDisplayForegroundColor;
        private OnScreenDisplayTheme _onScreenDisplayTheme;
        #endregion

        #region Public Properties
        public static Settings Default { get; private set; }

        public Color OnScreenDisplayDropShadowColor
        {
            get { return _onScreenDisplayDropShadowColor; }
            set
            {
                if (_onScreenDisplayDropShadowColor == value)
                {
                    return;
                }

                _onScreenDisplayDropShadowColor = value;

                OnPropertyChanged(OnScreenDisplayDropShadowColorPropertyName);
            }
        }

        public FadeSpeed OnScreenDisplayFadeSpeed
        {
            get { return _onScreenDisplayFadeSpeed; }
            set
            {
                if (_onScreenDisplayFadeSpeed == value)
                {
                    return;
                }

                _onScreenDisplayFadeSpeed = value;

                OnPropertyChanged(OnScreenDisplayFadeSpeedPropertyName);
            }
        }

        [XmlIgnore]
        public SolidColorBrush OnScreenDisplayForegroundBrush
        {
            get { return _onScreenDisplayForegroundBrush; }
            private set
            {
                _onScreenDisplayForegroundBrush = value;

                OnPropertyChanged(OnScreenDisplayForegroundBrushPropertyName);
            }
        }

        public Color OnScreenDisplayForegroundColor
        {
            get { return _onScreenDisplayForegroundColor; }
            set
            {
                if (_onScreenDisplayForegroundColor == value)
                {
                    return;
                }

                _onScreenDisplayForegroundColor = value;

                OnScreenDisplayForegroundBrush = new SolidColorBrush(_onScreenDisplayForegroundColor);
                OnPropertyChanged(OnScreenDisplayForegroundColorPropertyName);
            }
        }

        public OnScreenDisplayTheme OnScreenDisplayTheme
        {
            get { return _onScreenDisplayTheme; }
            set
            {
                if (_onScreenDisplayTheme == value)
                {
                    return;
                }

                _onScreenDisplayTheme = value;

                OnPropertyChanged(OnScreenDisplayThemePropertyName);
            }
        }
        #endregion

        #region Protected Methods
        protected void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Public Methods
        public void Load()
        {
            Reset();

            using (var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForDomain())
            {
                if (isolatedStorageFile.FileExists(FileName))
                {
                    using (var isolatedStorageFileStream =  isolatedStorageFile.OpenFile(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(Settings));
                        try
                        {
                            var loadedSettings = xmlSerializer.Deserialize(isolatedStorageFileStream) as Settings;

                            if (loadedSettings != null)
                            {
                                OnScreenDisplayDropShadowColor = loadedSettings.OnScreenDisplayDropShadowColor;
                                OnScreenDisplayFadeSpeed = loadedSettings.OnScreenDisplayFadeSpeed;
                                OnScreenDisplayForegroundColor = loadedSettings.OnScreenDisplayForegroundColor;
                                OnScreenDisplayTheme = loadedSettings.OnScreenDisplayTheme;
                            }
                        }
                        catch (Exception exception)
                        {
                            Logging.LogException(exception);
                        }
                    }
                }
            }
        }

        public void Save()
        {
            using (var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForDomain())
            {
                using (var isolatedStorageFileStream = isolatedStorageFile.OpenFile(FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    using (var streamWriter = new StreamWriter(isolatedStorageFileStream))
                    {
                        streamWriter.Write(this.ToXml());

                        streamWriter.Flush();
                    }
                }
            }
        }

        public void Reset()
        {
            OnScreenDisplayDropShadowColor = DefaultOnScreenDisplayDropShadowColor;
            OnScreenDisplayFadeSpeed = DefaultOnScreenDisplayFadeSpeed;
            OnScreenDisplayForegroundColor = DefaultOnScreenDisplayForegroundColor;
            OnScreenDisplayTheme = OnScreenDisplayThemes.DefaultTheme;
        }
        #endregion
    }
}
