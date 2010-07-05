using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CC.VolumeMixer.Controls
{
    public partial class ColorPicker
    {
        #region Constructor
        public ColorPicker()
        {
            InitializeComponent();
        }
        #endregion

        #region Dependency Properties
        public static DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof (Color), typeof (ColorPicker), new PropertyMetadata(Colors.White, SelectedColorPropertyChanged));
        #endregion

        #region Public Events
        public event EventHandler<ColorChangeEventArgs> SelectedColorChanged;
        public event EventHandler<ColorChangeEventArgs> SelectedColorChanging;
        #endregion

        #region Private Fields
        private bool _colorSampleMouseCaptured;
        private double _colorSampleX;
        private double _colorSampleY;

        private bool _hueMonitorMouseCaptured;
        private double _huePos;
        #endregion

        #region Public Properties
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }
        #endregion

        #region Private Event Handlers
        private void ColorSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _colorSampleMouseCaptured = ColorSample.CaptureMouse();
            var position = e.GetPosition((UIElement)sender);
            DragSliders(position.X, position.Y);
        }

        private void ColorSample_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ColorSample.ReleaseMouseCapture();
            _colorSampleMouseCaptured = false;
            SetValue(SelectedColorProperty, GetColor());
        }

        private void ColorSample_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_colorSampleMouseCaptured)
            {
                return;
            }

            var position = e.GetPosition((UIElement)sender);
            DragSliders(position.X, position.Y);
        }

        private void Header_MouseLeave(object sender, MouseEventArgs e)
        {
            PopupMain.StaysOpen = false;
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PopupMain.StaysOpen = true;
            PopupMain.IsOpen = !PopupMain.IsOpen;
        }

        private void HueMonitor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _hueMonitorMouseCaptured = HueMonitor.CaptureMouse();
            DragSliders(0, e.GetPosition((UIElement)sender).Y);
        }

        private void HueMonitor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HueMonitor.ReleaseMouseCapture();
            _hueMonitorMouseCaptured = false;
            SetValue(SelectedColorProperty, GetColor());
        }

        private void HueMonitor_MouseMove(object sender, MouseEventArgs e)
        {
            DragSliders(0, e.GetPosition((UIElement)sender).Y);
        }

        private static void SelectedColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorPicker = d as ColorPicker;
            if (colorPicker != null)
            {
                colorPicker.UpdateVisuals();
                colorPicker.OnSelectedColorChanged(new ColorChangeEventArgs((Color)e.NewValue));
            }
        }
        #endregion

        #region Private Methods
        private static Color ConvertHsvToRgb(double h, double s, double v)
        {
            h = h / 360;

            if (s > 0)
            {
                if (h >= 1)
                {
                    h = 0;
                }

                h = 6 * h;
                
                var hueFloor = (int)Math.Floor(h);
                var a = (byte)Math.Round(byte.MaxValue * v * (1.0 - s));
                var b = (byte)Math.Round(byte.MaxValue * v * (1.0 - (s * (h - hueFloor))));
                var c = (byte)Math.Round(byte.MaxValue * v * (1.0 - (s * (1.0 - (h - hueFloor)))));
                var d = (byte)Math.Round(byte.MaxValue * v);

                switch (hueFloor)
                {
                    case 0: return Color.FromArgb(byte.MaxValue, d, c, a);
                    case 1: return Color.FromArgb(byte.MaxValue, b, d, a);
                    case 2: return Color.FromArgb(byte.MaxValue, a, d, c);
                    case 3: return Color.FromArgb(byte.MaxValue, a, b, d);
                    case 4: return Color.FromArgb(byte.MaxValue, c, a, d);
                    case 5: return Color.FromArgb(byte.MaxValue, d, a, b);
                    default: return Color.FromArgb(0, 0, 0, 0);
                }
            }
            else
            {
                var d = (byte)(v * byte.MaxValue);
                return Color.FromArgb(255, d, d, d);
            }
        }

        private static HSV ConvertRgbToHsv(Color c)
        {
            var r = (c.R / 255.0);
            var g = (c.G / 255.0);
            var b = (c.B / 255.0);

            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));

            if (max == min)
            {
                return new HSV(0, 0, 0);
            }

            var h = 0.0;

            if (max == r && g >= b)
            {
                h = 60 * (g - b) / (max - min);
            }
            else if (max == r && g < b)
            {
                h = 60 * (g - b) / (max - min) + 360;
            }
            else if (max == g)
            {
                h = 60 * (b - r) / (max - min) + 120;
            }
            else if (max == b)
            {
                h = 60 * (r - g) / (max - min) + 240;
            }

            var s = (max == 0) ? 0.0 : (1.0 - (min / max));

            return new HSV(h, s, max);
        }

        private void DragSliders(double x, double y)
        {
            if (_hueMonitorMouseCaptured)
            {
                if (y < 0)
                {
                    _huePos = 0;
                }
                else if (y > HueMonitor.Height)
                {
                    _huePos = HueMonitor.Height;
                }
                else
                {
                    _huePos = y;
                }

                UpdateHueSelection();
            }
            else if (_colorSampleMouseCaptured)
            {
                if (x < 0)
                {
                    _colorSampleX = 0;
                }
                else if (x > ColorSample.Width)
                {
                    _colorSampleX = ColorSample.Width;
                }
                else
                {
                    _colorSampleX = x;
                }

                if (y < 0)
                {
                    _colorSampleY = 0;
                }
                else if (y > ColorSample.Height)
                {
                    _colorSampleY = ColorSample.Height;
                }
                else
                {
                    _colorSampleY = y;
                }

                UpdateSatValSelection();
            }
        }

        private Color GetColor()
        {
            double yComponent = 1 - (_colorSampleY / ColorSample.Height);
            double xComponent = _colorSampleX / ColorSample.Width;
            double hueComponent = (_huePos / HueMonitor.Height) * 360;

            return ConvertHsvToRgb(hueComponent, xComponent, yComponent);
        }
        
        private static Color GetColorFromPosition(double position)
        {
            const int gradientStops = 6;
            position *= gradientStops;
            var mod = (byte) (position % byte.MaxValue);
            var diff = (byte) (byte.MaxValue - mod);

            switch ((int)(position / byte.MaxValue))
            {
                case 0: return Color.FromArgb(byte.MaxValue, byte.MaxValue, mod, byte.MinValue);
                case 1: return Color.FromArgb(byte.MaxValue, diff, byte.MaxValue, byte.MinValue);
                case 2: return Color.FromArgb(byte.MaxValue, byte.MinValue, byte.MaxValue, mod);
                case 3: return Color.FromArgb(byte.MaxValue, byte.MinValue, diff, byte.MaxValue);
                case 4: return Color.FromArgb(byte.MaxValue, mod, byte.MinValue, byte.MaxValue);
                case 5: return Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MinValue, diff);
                case 6: return Color.FromArgb(byte.MaxValue, byte.MaxValue, mod, byte.MinValue);
                default: return Colors.Black;
            }
        }

        private static string GetHtmlColor(Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        private void UpdateCurrentColor()
        {
            var currentColor = GetColor();

            TextBlockHtmlColor.Text = GetHtmlColor(currentColor);
            RectangleSelectedColor.Fill = new SolidColorBrush(currentColor);

            OnSelectedColorChanging(new ColorChangeEventArgs(currentColor));
        }

        private void UpdateHueSelection()
        {
            var huePos = _huePos / HueMonitor.Height * 255;
            
            ColorSample.Fill = new SolidColorBrush(GetColorFromPosition(huePos));
            HueSelector.SetValue(Canvas.TopProperty, _huePos - (HueSelector.Height / 2));
            
            UpdateCurrentColor();
        }

        private void UpdateSatValSelection()
        {
            SampleSelector.SetValue(Canvas.LeftProperty, _colorSampleX - (SampleSelector.Height / 2));
            SampleSelector.SetValue(Canvas.TopProperty, _colorSampleY - (SampleSelector.Height / 2));

            UpdateCurrentColor();
        }

        private void UpdateVisuals()
        {
            var hsv = ConvertRgbToHsv(SelectedColor);

            _huePos = (hsv.Hue / 360 * HueMonitor.Height);
            _colorSampleY = -1 * (hsv.Value - 1) * ColorSample.Height;
            _colorSampleX = hsv.Saturation * ColorSample.Width;
            
            if (!double.IsNaN(_huePos))
            {
                UpdateHueSelection();
            }

            UpdateSatValSelection();
        }

        #endregion

        #region Protected Methods
        protected void OnSelectedColorChanged(ColorChangeEventArgs eventArgs)
        {
            var selectedColorChanged = SelectedColorChanged;
            if (selectedColorChanged != null)
            {
                selectedColorChanged.Invoke(this, eventArgs);
            }
        }

        protected void OnSelectedColorChanging(ColorChangeEventArgs eventArgs)
        {
            var selectedColorChanging = SelectedColorChanging;
            if (selectedColorChanging != null)
            {
                selectedColorChanging.Invoke(this, eventArgs);
            }
        }
        #endregion
    }
}
