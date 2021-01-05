namespace LacedServer.Classes.Managers
{
    using System.Collections.Generic;
    using LacedShared.Libs;
    using LacedShared.Models.Configs;
    using Newtonsoft.Json;

    public class ConfigManager
    {
        public static ServerConfig ServerConfig = null;
        public static CharacterConfig CharacterConfig = null;
        public static List<SpawningConfig> SpawningConfig = null;
        public static WeatherConfig WeatherConfig = null;
        public static Dictionary<string, int> ControlsConfig = null;
        public static Dictionary<string, MarkerConfig> MarkerConfig = null;
        public ConfigManager()
        {
            Utils.DebugLine("Config Manager Loading", "SConfigManager");

            string resourceName = MainServer.ResouceName();
            ServerConfig = JsonConvert.DeserializeObject<ServerConfig>(MainServer.LoadResourceFile(resourceName, "/configs/server_config.json"));
            CharacterConfig = JsonConvert.DeserializeObject<CharacterConfig>(MainServer.LoadResourceFile(resourceName, "/configs/character_config.json"));
            SpawningConfig = JsonConvert.DeserializeObject<List<SpawningConfig>>(MainServer.LoadResourceFile(resourceName, "/configs/playerspawns.json"));
            WeatherConfig = JsonConvert.DeserializeObject<WeatherConfig>(MainServer.LoadResourceFile(resourceName, "/configs/weather_config.json"));
            ControlsConfig = JsonConvert.DeserializeObject<Dictionary<string, int>>(MainServer.LoadResourceFile(resourceName, "/configs/controls_config.json"));
            MarkerConfig = JsonConvert.DeserializeObject<Dictionary<string, MarkerConfig>>(MainServer.LoadResourceFile(resourceName, "/configs/markers_config.json"));

            Utils.DebugLine("Config Manager Loaded", "SConfigManager");
        }
    }
}
