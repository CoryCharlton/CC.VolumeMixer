using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace CC.VolumeMixer
{
    public partial class SettingsWindow
    {
        #region Constructor
        public SettingsWindow()
        {
            InitializeComponent();

            // Setup Fade Speed ComboBoxItems
            ComboBoxOnScreenDisplayFadeSpeed.Items.Add(FadeSpeed.Slow);
            ComboBoxOnScreenDisplayFadeSpeed.Items.Add(FadeSpeed.Normal);
            ComboBoxOnScreenDisplayFadeSpeed.Items.Add(FadeSpeed.Fast);

            // Setup Color Theme ComboBoxItems
            foreach (var onScreenDisplayTheme in OnScreenDisplayThemes.Themes)
            {
                ComboBoxOnScreenDisplayTheme.Items.Add(onScreenDisplayTheme);
            }

            //TODO: Remove after debug....
            ComboBoxOnScreenDisplayTheme.SelectedIndex = 5;

            SettingsToUI();
        }

        public SettingsWindow(OnScreenDisplayWindow onScreenDisplayWindow) : this()
        {
            _onScreenDisplayWindow = onScreenDisplayWindow;
        }
        #endregion

        #region Private Fields
        private readonly OnScreenDisplayWindow _onScreenDisplayWindow;
        #endregion

        #region Private Event Handlers
        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            UIToSettings();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Load();
            Close();
        }

        private void ButtonDefaults_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Reset();
            SettingsToUI();
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            UIToSettings();
            Settings.Default.Save();
            Close();
        }

        private void ColorPickerOnScreenDisplayDropShadowColor_SelectedColorChanged(object sender, ColorChangeEventArgs e)
        {
            ComboBoxOnScreenDisplayTheme.SelectedItem = GetThemeFromSelectedColors();
        }

        private void ColorPickerOnScreenDisplayDropShadowColor_SelectedColorChanging(object sender, ColorChangeEventArgs e)
        {
            if (_onScreenDisplayWindow != null)
            {
                _onScreenDisplayWindow.VolumeBar.DropShadowColor = e.Color;
            }
        }

        private void ColorPickerOnScreenDisplayForegroundColor_SelectedColorChanged(object sender, ColorChangeEventArgs e)
        {
            ComboBoxOnScreenDisplayTheme.SelectedItem = GetThemeFromSelectedColors();
        }

        private void ColorPickerOnScreenDisplayForegroundColor_SelectedColorChanging(object sender, ColorChangeEventArgs e)
        {
            if (_onScreenDisplayWindow != null)
            {
                _onScreenDisplayWindow.VolumeBar.Foreground = new SolidColorBrush(e.Color);
            }
        }

        private void ComboBoxOnScreenDisplayTheme_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var onScreenDisplayTheme = (OnScreenDisplayTheme) ComboBoxOnScreenDisplayTheme.SelectedItem;
            if (onScreenDisplayTheme != OnScreenDisplayTheme.Custom)
            {
                ColorPickerOnScreenDisplayDropShadowColor.SelectedColor = onScreenDisplayTheme.DropShadowColor;
                ColorPickerOnScreenDisplayForegroundColor.SelectedColor = onScreenDisplayTheme.ForegroundColor;
            }
        }
        #endregion

        #region Private Methods
        private OnScreenDisplayTheme GetThemeFromSelectedColors()
        {
            var returnValue = OnScreenDisplayTheme.Custom;

            foreach (var onScreenDisplayTheme in OnScreenDisplayThemes.Themes.Where(onScreenDisplayTheme => onScreenDisplayTheme.DropShadowColor == ColorPickerOnScreenDisplayDropShadowColor.SelectedColor && onScreenDisplayTheme.ForegroundColor == ColorPickerOnScreenDisplayForegroundColor.SelectedColor))
            {
                returnValue = onScreenDisplayTheme;
                break;
            }

            return returnValue;
        }

        private void SettingsToUI()
        {
            ColorPickerOnScreenDisplayDropShadowColor.SelectedColor = Settings.Default.OnScreenDisplayDropShadowColor;
            ColorPickerOnScreenDisplayForegroundColor.SelectedColor = Settings.Default.OnScreenDisplayForegroundColor;
            ComboBoxOnScreenDisplayFadeSpeed.SelectedItem = Settings.Default.OnScreenDisplayFadeSpeed;
            ComboBoxOnScreenDisplayTheme.SelectedItem = Settings.Default.OnScreenDisplayTheme;
        }
        
        private void UIToSettings()
        {
            Settings.Default.OnScreenDisplayDropShadowColor = ColorPickerOnScreenDisplayDropShadowColor.SelectedColor;
            Settings.Default.OnScreenDisplayFadeSpeed = (FadeSpeed) ComboBoxOnScreenDisplayFadeSpeed.SelectedItem;
            Settings.Default.OnScreenDisplayForegroundColor = ColorPickerOnScreenDisplayForegroundColor.SelectedColor;
            Settings.Default.OnScreenDisplayTheme = (OnScreenDisplayTheme) ComboBoxOnScreenDisplayTheme.SelectedItem;
        }
        #endregion
        
        #region Protected Methods
        protected override void OnActivated(EventArgs e)
        {
            if (_onScreenDisplayWindow != null)
            {
                _onScreenDisplayWindow.IsPreviewMode = true;
            }

            base.OnActivated(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_onScreenDisplayWindow != null)
            {
                _onScreenDisplayWindow.IsPreviewMode = false;
            }

            base.OnClosing(e);
        }
        #endregion
    }
}
