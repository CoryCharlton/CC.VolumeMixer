using System.Windows;

namespace CC.VolumeMixer
{
    public partial class App
    {
        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.Save();

            base.OnExit(e);
        }
    }
}
