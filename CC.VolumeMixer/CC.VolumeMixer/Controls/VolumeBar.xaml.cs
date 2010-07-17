using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using CC.Utilities;
using CoreAudioApi;

namespace CC.VolumeMixer.Controls
{
    public partial class VolumeBar
    {
        #region Constructor
        public VolumeBar()
        {
            InitializeComponent();
            //CoreAudioDevice.Default.VolumeChanged += CoreAudioDevice_VolumeChanged;

            var dropShadowColorBinding = BindingHelper.CreateOneWayBinding(DropShadowColorProperty, this);
            var volumeBinding = BindingHelper.CreateOneWayBinding(CoreAudioDevice.VolumeProperty, CoreAudioDevice.Default);
            
            ProgressBarVolume.SetBinding(ForegroundProperty, BindingHelper.CreateOneWayBinding(ForegroundProperty, this));
            ProgressBarVolume.SetBinding(RangeBase.ValueProperty, volumeBinding);
            TextBlockVolume.SetBinding(TextBlock.TextProperty, volumeBinding);
            VolumeIcon.SetBinding(VolumeIcon.DropShadowColorProperty, dropShadowColorBinding);
            VolumeIcon.SetBinding(VolumeIcon.IsMutedProperty, BindingHelper.CreateOneWayBinding(CoreAudioDevice.IsMutedProperty, CoreAudioDevice.Default));

            BindingOperations.SetBinding(DropShadowProgressBar, DropShadowEffect.ColorProperty, dropShadowColorBinding);
            BindingOperations.SetBinding(DropShadowTextBlock, DropShadowEffect.ColorProperty, dropShadowColorBinding);
            
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                SetBinding(DropShadowColorProperty, BindingHelper.CreateOneWayBinding(Settings.OnScreenDisplayDropShadowColorPropertyName, Settings.Default));
                SetBinding(ForegroundProperty, BindingHelper.CreateOneWayBinding(Settings.OnScreenDisplayForegroundBrushPropertyName, Settings.Default));
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
        private void CoreAudioDevice_VolumeChanged()
        {
            //VolumeIcon.IsMuted = volumeNotificationData.Muted;
        }
        #endregion
    }
}
