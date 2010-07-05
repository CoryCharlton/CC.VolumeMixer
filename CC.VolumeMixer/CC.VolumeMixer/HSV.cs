namespace CC.VolumeMixer
{
    internal struct HSV
    {
        #region Constructor
        public HSV(double hue, double saturation, double value)
        {
            _hue = hue;
            _saturation = saturation;
            _value = value;
        }
        #endregion
        
        #region Private Fields
        private readonly double _hue;
        private readonly double _saturation;
        private readonly double _value;
        #endregion

        #region Public Properties
        public double Hue
        {
            get { return _hue; }
        }

        public double Saturation
        {
            get { return _saturation; }
        }

        public double Value
        {
            get { return _value; }
        }
        #endregion
    }
}
