using MacroTalk.DataStruct;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace MacroTalk
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ApplicationSettings settings = Utils.SettingData;
            string json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "Settings.json", json);

            //TODO:加载预制头像

        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Settings.json"))
                return;
            string json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Settings.json");
            ApplicationSettings settings = JsonConvert.DeserializeObject<ApplicationSettings>(json)!;
            Utils.SettingData = settings;
        }
    }
}
