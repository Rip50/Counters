using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Serialization;
using mshtml;
using Newtonsoft.Json;

namespace Browser
{

    public class Config
    {
        public class Session
        {
            public DateTime SessionTime;
            public WaterCounterData Water;
            public GazCounterData Gaz;
        }

        public List<Session> Sessions;

        public int Day;
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    public partial class App : Application
    {
        public static Config Config { get; set; }

        public Config LoadConfig(string name = null)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(),"configs", $"{name}.cfg");
            return File.Exists(path) ? 
                   JsonConvert.DeserializeObject<Config>(File.ReadAllText(path)) : 
                   new Config() {Day = 24, Sessions = new List<Config.Session>()
                   {
                       new Config.Session() {Gaz = new GazCounterData(), SessionTime = DateTime.Now, Water = new WaterCounterData()}
                   } };
        }

        public Config LoadConfigOrCreateNew(string name = null)
        {
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "configs");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, $"{name}.cfg");
            if (!File.Exists(path))
            {
                using (var stream = File.Create(path))
                {
                    var serialized = JsonConvert.SerializeObject(new Config(), Formatting.Indented);
                    using (var writer = new StreamWriter(stream))
                        writer.Write(serialized);
                }
            }

            return LoadConfig(name);
        }

        public void SaveConfig(string name = null)
        {
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "configs");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, $"{name}.cfg");
            if (!File.Exists(path))
                File.Create(path);
            File.WriteAllText(path, JsonConvert.SerializeObject(Config, Formatting.Indented));
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            Config = LoadConfigOrCreateNew("Config");
            var now = DateTime.Now;
            var lastSession = Config.Sessions?.LastOrDefault();
            if (lastSession != null && (now.Day < Config.Day || now.Subtract(lastSession.SessionTime) < TimeSpan.FromDays(DateTime.DaysInMonth(now.Year, now.Month) - Config.Day)))
            {
                Current.Shutdown(0);
            }
            Config.Sessions?.Add(new Config.Session() {
                Gaz = new GazCounterData(),
                Water = new WaterCounterData()
                {
                    CW1 = new WaterCounterData.Impl(), CW2 = new WaterCounterData.Impl(), HW1 = new WaterCounterData.Impl(), HW2 = new WaterCounterData.Impl()
                },
                SessionTime = now });

            Current.MainWindow = new MainWindow();
            var mw = (Browser.MainWindow) Current.MainWindow;
            mw.SessionSucceeded += OnSessionSucceeded;
            mw.SessionFailed += OnSessionFailed;
            Current.MainWindow.Show();
        }

        private void OnSessionFailed()
        {
            Current.Shutdown(0);
        }

        private void OnSessionSucceeded()
        {
            SaveConfig("Config");
            Current.Shutdown(0);
        }
        
    }

}
