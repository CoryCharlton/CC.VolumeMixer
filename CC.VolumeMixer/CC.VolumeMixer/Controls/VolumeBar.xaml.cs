using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using CoreAudioApi;

namespace CC.VolumeMixer.Controls
{
    public partial class VolumeBar
    {
        #region Constructor
        public VolumeBar()
        {
            InitializeComponent();
            CoreAudioDevice.Default.VolumeChanged += CoreAudioDevice_VolumeChanged;

            var dropShadowColorBinding = new Binding
                                             {
                                                 Mode = BindingMode.OneWay,
                                                 Path = new PropertyPath(DropShadowColorProperty),
                                                 Source = this
                                             };

            var volumeBinding = new Binding
                                    {
                                        Mode = BindingMode.OneWay,
                                        Path = new PropertyPath(CoreAudioDevice.VolumeProperty),
                                        Source = CoreAudioDevice.Default
                                    };

            
            ProgressBarVolume.SetBinding(ForegroundProperty, new Binding {Mode = BindingMode.OneWay, Path = new PropertyPath(ForegroundProperty), Source = this});
            ProgressBarVolume.SetBinding(RangeBase.ValueProperty, volumeBinding);
            TextBlockVolume.SetBinding(TextBlock.TextProperty, volumeBinding);
            VolumeIcon.SetBinding(VolumeIcon.DropShadowColorProperty, dropShadowColorBinding);

            BindingOperations.SetBinding(DropShadowProgressBar, DropShadowEffect.ColorProperty, dropShadowColorBinding);
            BindingOperations.SetBinding(DropShadowTextBlock, DropShadowEffect.ColorProperty, dropShadowColorBinding);
            
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                SetBinding(DropShadowColorProperty, new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(Settings.OnScreenDisplayDropShadowColorPropertyName), Source = Settings.Default });
                SetBinding(ForegroundProperty, new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(Settings.OnScreenDisplayForegroundBrushPropertyName), Source = Settings.Default });
            }
        }
        #endregion

        #region Dependency Properties
        public static DependencyProperty DropShadowColorProperty = DependencyProperty.Register("DropShadowColor", typeof(Color), typeof(VolumeBar), new PropertyMetadata(Colors.Black));
        #endregion

        #region Public Properties
        public Color DropShadowColor
        {
            get { return (Color)GetValue(DropShadowColorProperty); }
            set { SetValue(DropShadowColorProperty, value); }
        }
        #endregion

        #region Private Event Handlers
        private void CoreAudioDevice_VolumeChanged(AudioVolumeNotificationData volumeNotificationData)
        {
            VolumeIcon.IsMuted = volumeNotificationData.Muted;
        }
        #endregion
    }
}
