using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace CC.VolumeMixer
{
    public static class OnScreenDisplayThemes
    {
        #region Constructor
        static OnScreenDisplayThemes()
        {
            // Keep alphabetized
            _themes.Add(new OnScreenDisplayTheme("Blue", (Color)ColorConverter.ConvertFromString("#FF19A5C1")));
            _themes.Add(new OnScreenDisplayTheme("Green", (Color)ColorConverter.ConvertFromString("#FF5AC119")));
            _themes.Add(new OnScreenDisplayTheme("Orange", (Color)ColorConverter.ConvertFromString("#FFC15719")));
            _themes.Add(new OnScreenDisplayTheme("Purple", (Color)ColorConverter.ConvertFromString("#FFB619C1")));
            _themes.Add(new OnScreenDisplayTheme("Red", (Color)ColorConverter.ConvertFromString("#FFC11919")));

            // Default theme should be "Blue"
            DefaultTheme = _themes[0];

            // Always add Custom last
            _themes.Add(OnScreenDisplayTheme.Custom);
        }
        #endregion

        #region Private Fields
        private static OnScreenDisplayTheme _defaultTheme = null;
        private static readonly object _lockObject = new object();
        private static readonly List<OnScreenDisplayTheme> _themes = new List<OnScreenDisplayTheme>();
        #endregion

        #region Public Properties
        public static OnScreenDisplayTheme DefaultTheme
        {
            get
            {
                if (_defaultTheme == null)
                {
                    lock (_lockObject)
                    {
                        _defaultTheme = new OnScreenDisplayTheme("Blue", (Color) ColorConverter.ConvertFromString("#FF19A5C1"));
                    }
                }

                return _defaultTheme;
            }
            private set { _defaultTheme = value; }
        }

        public static ReadOnlyCollection<OnScreenDisplayTheme> Themes
        {
            get { return _themes.AsReadOnly(); }
        }
        #endregion
    }
}
