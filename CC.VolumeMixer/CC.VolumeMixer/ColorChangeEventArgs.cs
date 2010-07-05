using System;
using System.Windows.Media;

namespace CC.VolumeMixer
{
    public class ColorChangeEventArgs: EventArgs
    {
        public ColorChangeEventArgs(Color color)
        {
            Color = color;
        }

        public Color Color { get; private set; }
    }
}
