using LacedShared.Libs;
using LacedShared.Models.Configs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedClient.Classes.Managers
{
    public class ConfigManager
    {
        public static List<SpawningConfig> SpawningConfig = null;
        public static CharacterConfig CharacterConfig = null;
        public static Dictionary<string, int> ControlsConfig = null;
        public static Dictionary<string, MarkerConfig> MarkerConfig = null;
        public ConfigManager()
        {
            MainClient.GetInstance().RegisterEventHandler("Laced:SendClientConfigs", new Action<string, string>(SetConfigs));

            Utils.WriteLine("ConfigManager Loaded!");
        }
        ~ConfigManager()
        {
            MainClient.GetInstance().UnregisterEventHandler("Laced:SendClientConfigs");

            Utils.WriteLine("ConfigManager was destroyed!");
        }
        private void SetConfigs(string _type, string _config)
        {
            switch (_type)
            {
                case "spawns":
                    SpawningConfig = JsonConvert.DeserializeObject<List<SpawningConfig>>(_config);
                    break;
                case "controls":
                    ControlsConfig = JsonConvert.DeserializeObject<Dictionary<string, int>>(_config);
                    break;
                case "character":
                    CharacterConfig = JsonConvert.DeserializeObject<CharacterConfig>(_config);
                    break;
                case "markers":
                    MarkerConfig = JsonConvert.DeserializeObject<Dictionary<string, MarkerConfig>>(_config);
                    break;
                default:
                    Utils.WriteLine($"Couldn't Load Config {_type}");
                    break;
            }
        }
    }
}
