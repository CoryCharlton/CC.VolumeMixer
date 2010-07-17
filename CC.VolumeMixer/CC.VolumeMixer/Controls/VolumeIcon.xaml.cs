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
using CC.Utilities;

namespace CC.VolumeMixer.Controls
{
	public partial class VolumeIcon
    {
        #region Constructor
        public VolumeIcon()
		{
			InitializeComponent();
            IsMuted = false;

            var dropShadowColorBinding = BindingHelper.CreateOneWayBinding(DropShadowColorProperty, this);

            BindingOperations.SetBinding(DropShadowVolume, DropShadowEffect.ColorProperty, dropShadowColorBinding);
            BindingOperations.SetBinding(DropShadowX, DropShadowEffect.ColorProperty, dropShadowColorBinding);
            
            var foregroundBinding = BindingHelper.CreateOneWayBinding(ForegroundProperty, this);

            PathVolume.SetBinding(Shape.FillProperty, foregroundBinding);
            PathX.SetBinding(VisibilityProperty, BindingHelper.CreateOneWayBinding(IsMutedProperty, this, new BooleanToVisibilityConverter()));
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