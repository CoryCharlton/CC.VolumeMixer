using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using CoreAudioApi;

namespace CC.VolumeMixer
{
    public class CoreAudioDevice: DependencyObject
    {
        #region Constructor
        static CoreAudioDevice()
        {
            Default = new CoreAudioDevice();
        }

        public CoreAudioDevice()
        {
            //TODO: Support multiple devices
            var deviceEnumerator = new MMDeviceEnumerator();
            
            _device = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            _device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;

            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(150), DispatcherPriority.Normal, Timer_Tick, Dispatcher) {IsEnabled = false};

            IsMuted = _device.AudioEndpointVolume.Mute;
            Volume = (int) (_device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
        }
        #endregion

        #region Dependency Properties
        public static DependencyProperty IsMutedProperty = DependencyProperty.Register("IsMuted", typeof(bool), typeof(CoreAudioDevice), new PropertyMetadata(false, VolumePropertyChanged));
        public static DependencyProperty PeakLeftProperty = DependencyProperty.Register("PeakLeft", typeof(int), typeof(CoreAudioDevice), new PropertyMetadata(0));
        public static DependencyProperty PeakProperty = DependencyProperty.Register("Peak", typeof(int), typeof(CoreAudioDevice), new PropertyMetadata(0));
        public static DependencyProperty PeakRightProperty = DependencyProperty.Register("PeakRight", typeof(int), typeof(CoreAudioDevice), new PropertyMetadata(0));
        public static DependencyProperty VolumeProperty = DependencyProperty.Register("Volume", typeof(int), typeof(CoreAudioDevice), new PropertyMetadata(0, VolumePropertyChanged));
        #endregion

        #region Public Events
        public event EventHandler VolumeChanged;
        #endregion

        #region Private Fields
        private readonly MMDevice _device;
        private readonly DispatcherTimer _timer;
        #endregion

        #region Public Properties
        public static CoreAudioDevice Default { get; protected set; }

        public bool IsPeakMonitorEnabled
        {
            get { return _timer.IsEnabled; }
            set { _timer.IsEnabled = value; }
        }

        public bool IsMuted
        {
            get { return (bool)GetValue(IsMutedProperty); }
            set { SetValue(IsMutedProperty, value); }
        }

        public int Peak
        {
            get { return (int)GetValue(PeakProperty); }
            set { SetValue(PeakProperty, value); }
        }


        public int PeakLeft
        {
            get { return (int)GetValue(PeakLeftProperty); }
            set { SetValue(PeakLeftProperty, value); }
        }

        public int PeakRight
        {
            get { return (int)GetValue(PeakRightProperty); }
            set { SetValue(PeakRightProperty, value); }
        }
        
        public int Volume
        {
            get { return (int) GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }
        #endregion

        #region Private Event Handlers
        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData notificationData)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(() => AudioEndpointVolume_OnVolumeNotification(notificationData)));
            }
            else
            {
                IsMuted = notificationData.Muted;
                Volume = (int) (notificationData.MasterVolume*100);
                
                //OnVolumeChanged(notificationData);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Peak = (int) (_device.AudioMeterInformation.MasterPeakValue*100);
            PeakLeft = (int) (_device.AudioMeterInformation.PeakValues[0]*100);
            PeakRight = (int) (_device.AudioMeterInformation.PeakValues[1]*100);
        }

        private static void VolumePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var coreAudioDevice = d as CoreAudioDevice;
            if (coreAudioDevice != null)
            {
                coreAudioDevice.OnVolumeChanged();
            }
        }
        #endregion

        #region Protected Methods
        protected void OnVolumeChanged()
        {
            var volumeChanged = VolumeChanged;
            if (volumeChanged != null)
            {
                volumeChanged.BeginInvoke(this, EventArgs.Empty, null, null);
            }
        }
        #endregion
    }
}
