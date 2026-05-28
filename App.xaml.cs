using System.Windows;
using URUManager.Services;

namespace URUManager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LanguageManager.Initialize();
        }
    }
}
