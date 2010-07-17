using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using CoreAudioApi;

namespace CC.VolumeMixer
{
    public partial class OnScreenDisplayWindow
    {
        #region Constructor
        public OnScreenDisplayWindow()
        {
            InitializeComponent();
            VolumeBar.Opacity = 0;
            CoreAudioDevice.Default.VolumeChanged += CoreAudioDevice_VolumeChanged;
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
            SetNotifyIconIcon();

            _contextMenu.MenuItems.Add(new MenuItem("Settings", Settings_OnClick));
            _contextMenu.MenuItems.Add("-");
            _contextMenu.MenuItems.Add(new MenuItem("Exit", Exit_OnClick));

            _notifyIcon.ContextMenu = _contextMenu;
            //_notifyIcon.MouseDown += NotifyIcon_MouseDown;
            _notifyIcon.Visible = true;

            _onTopTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, OnTopTimer_Tick, Dispatcher) {IsEnabled = false};

            _volumeBarFadeAnimation.Completed += VolumeBarFadeAnimation_Completed;

            Visibility = Visibility.Hidden;
        }
        #endregion

        #region Dependency Properties
        public static DependencyProperty IsPreviewModeProperty = DependencyProperty.Register("IsPreviewMode", typeof(bool), typeof(OnScreenDisplayWindow), new PropertyMetadata(false, IsPreviewModePropertyChanged));
        #endregion

        #region Private Fields
        private readonly ContextMenu _contextMenu = new ContextMenu();
        //private readonly MasterVolumeWindow _masterVolumeWindow = new MasterVolumeWindow();
        private readonly NotifyIcon _notifyIcon = new NotifyIcon();
        private readonly DispatcherTimer _onTopTimer;
        private volatile bool _settingsOpen;
        private volatile SettingsWindow _settingsWindow;
        private readonly Icon _sound000 = Properties.Resources.Sound000;
        private readonly Icon _sound050 = Properties.Resources.Sound050;
        private readonly Icon _sound100 = Properties.Resources.Sound100;
        private readonly Icon _soundMuted = Properties.Resources.SoundMuted;
        private readonly DoubleAnimation _volumeBarFadeAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(5))) {AutoReverse = false, AccelerationRatio = .75};
        #endregion

        #region Public Properties
        public bool IsPreviewMode
        {
            get { return (bool) GetValue(IsPreviewModeProperty); }
            set { SetValue(IsPreviewModeProperty, value); }
        }
        #endregion

        #region Private Event Handlers
        private void CoreAudioDevice_VolumeChanged(object sender, EventArgs e)
        {
            SetNotifyIconIcon();
            ShowVolumeIcon();
        }

        private void Exit_OnClick(object sender, EventArgs e)
        {
            Close();
        }

        private static void IsPreviewModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var onScreenDisplayWindow = d as OnScreenDisplayWindow;
            if (onScreenDisplayWindow != null)
            {
                if ((bool)e.NewValue)
                {
                    if (onScreenDisplayWindow.Visibility != Visibility.Visible)
                    {
                        onScreenDisplayWindow.Topmost = true;
                        onScreenDisplayWindow.Visibility = Visibility.Visible;

                        onScreenDisplayWindow._onTopTimer.IsEnabled = true;
                    }

                    onScreenDisplayWindow.VolumeBar.BeginAnimation(OpacityProperty, null);
                    onScreenDisplayWindow.VolumeBar.Opacity = 1;
                }
                else
                {
                    onScreenDisplayWindow.VolumeBar.DropShadowColor = Settings.Default.OnScreenDisplayDropShadowColor;
                    onScreenDisplayWindow.VolumeBar.Foreground = Settings.Default.OnScreenDisplayForegroundBrush;
                    onScreenDisplayWindow.VolumeBar.Opacity = 0;
                    onScreenDisplayWindow.VolumeBar.BeginAnimation(OpacityProperty, onScreenDisplayWindow._volumeBarFadeAnimation);
                }
            }
        }

        private void OnTopTimer_Tick(object sender, EventArgs e)
        {
            Topmost = false;
            Topmost = true;
        }

        private void Settings_OnClick(object sender, EventArgs e)
        {
            if (_settingsOpen && _settingsWindow != null)
            {
                _settingsWindow.Activate();
            }
            else
            {
                _settingsOpen = true;
                _settingsWindow = new SettingsWindow(this);
                _settingsWindow.ShowDialog();
                _settingsOpen = false;
            }
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case Settings.OnScreenDisplayFadeSpeedPropertyName:
                    {
                        switch (Settings.Default.OnScreenDisplayFadeSpeed)
                        {
                            case FadeSpeed.Fast:
                                {
                                    _volumeBarFadeAnimation.Duration = TimeSpan.FromSeconds(2.5);
                                    break;
                                }
                            case FadeSpeed.Normal:
                                {
                                    _volumeBarFadeAnimation.Duration = TimeSpan.FromSeconds(5);
                                    break;
                                }
                            case FadeSpeed.Slow:
                                {
                                    _volumeBarFadeAnimation.Duration = TimeSpan.FromSeconds(7.5);
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        //TODO: Implement this..
        //private void NotifyIcon_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        _masterVolumeWindow.Show();
        //    }
        //}

        private void VolumeBarFadeAnimation_Completed(object sender, EventArgs e)
        {
            if (!IsPreviewMode && VolumeBar.Opacity == 0)
            {
                _onTopTimer.IsEnabled = false;

                Visibility = Visibility.Hidden;
            }
        }
        #endregion

        #region Private 
        private void SetNotifyIconIcon()
        {
            if (!CoreAudioDevice.Default.Dispatcher.CheckAccess())
            {
                CoreAudioDevice.Default.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(SetNotifyIconIcon));
            }
            else
            {
                if (CoreAudioDevice.Default.IsMuted)
                {
                    _notifyIcon.Icon = _soundMuted;
                }
                else
                {
                    if (CoreAudioDevice.Default.Volume > 60)
                    {
                        _notifyIcon.Icon = _sound100;
                    }
                    else if (CoreAudioDevice.Default.Volume > 40)
                    {
                        _notifyIcon.Icon = _sound050;
                    }
                    else
                    {
                        _notifyIcon.Icon = _sound000;
                    }
                }
            }
        }

        private void ShowVolumeIcon()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(ShowVolumeIcon));
            }
            else
            {
                VolumeBar.BeginAnimation(OpacityProperty, null);

                if (Visibility != Visibility.Visible)
                {
                    Topmost = true;
                    Visibility = Visibility.Visible;

                    _onTopTimer.IsEnabled = true;
                }

                if (!IsPreviewMode)
                {
                    VolumeBar.BeginAnimation(OpacityProperty, _volumeBarFadeAnimation);
                }
            }
        }
        #endregion

        #region Protected Methods
        protected override void OnClosing(CancelEventArgs e)
        {
            _notifyIcon.Visible = false;

            base.OnClosing(e);
        }
        #endregion
    }
}
