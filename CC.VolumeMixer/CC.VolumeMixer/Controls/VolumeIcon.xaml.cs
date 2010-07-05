using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CC.VolumeMixer.Controls
{
	public partial class VolumeIcon
    {
        #region Constructor
        public VolumeIcon()
		{
			InitializeComponent();
            IsMuted = false;

            var dropShadowColorBinding = new Binding
                                             {
                                                 Mode = BindingMode.OneWay,
                                                 Path = new PropertyPath(DropShadowColorProperty),
                                                 Source = this
                                             };

            BindingOperations.SetBinding(DropShadowVolume, DropShadowEffect.ColorProperty, dropShadowColorBinding);
            BindingOperations.SetBinding(DropShadowX, DropShadowEffect.ColorProperty, dropShadowColorBinding);
            
            var foregroundBinding = new Binding
		                                {
		                                    Mode = BindingMode.OneWay,
		                                    Path = new PropertyPath(ForegroundProperty),
		                                    Source = this
		                                };

            PathVolume.SetBinding(Shape.FillProperty, foregroundBinding);
            PathX.SetBinding(VisibilityProperty, new Binding { Converter = new BooleanToVisibilityConverter(), Mode = BindingMode.OneWay, Path = new PropertyPath(IsMutedProperty), Source = this });
		}
        #endregion

        #region Dependency Properties
        public static DependencyProperty DropShadowColorProperty = DependencyProperty.Register("DropShadowColor", typeof(Color), typeof(VolumeIcon), new PropertyMetadata(Colors.Black));
        public static DependencyProperty IsMutedProperty = DependencyProperty.Register("IsMuted", typeof(bool), typeof(VolumeIcon), new PropertyMetadata(false));
        #endregion

        #region Public Properties
        public Color DropShadowColor
        {
            get { return (Color)GetValue(DropShadowColorProperty); }
            set { SetValue(DropShadowColorProperty, value); }
        }
        
        public bool IsMuted
        {
            get { return (bool)GetValue(IsMutedProperty); }
            set { SetValue(IsMutedProperty, value); }
        }
        #endregion
    }
}