using System.Windows.Data;

namespace BossConsoleHost
{
    public class SettingBindingExtension : Binding
    {
        public SettingBindingExtension()
        {
            Initialize();
        }

        public SettingBindingExtension(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Source = BossConsoleHost.Properties.Settings.Default;
            this.Mode = BindingMode.TwoWay;
        }
    }
}