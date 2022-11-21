using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace WolfPad.MVVM.Models
{
    public class AppConfig
    {
        public string ReOpenPath { get; set; } = string.Empty;

        public void Read()
        {
            try
            {
                var readed = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText("appConfig.json"));
                ReOpenPath = readed.ReOpenPath;
            }
            catch (Exception) { }

        }

        public void Write()
        {
            try
            {
                File.WriteAllText("appConfig.json", JsonSerializer.Serialize<AppConfig>(this));
            }
            catch (Exception) { }
        }

    }
}
