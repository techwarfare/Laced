using CitizenFX.Core;
using LacedServer.Models;
using LacedShared.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedServer.Classes.Managers
{
    public enum Weather
    {
        Unknown = -1,
        ExtraSunny = 0,
        Clear = 1,
        Clouds = 2,
        Smog = 3,
        Foggy = 4,
        Overcast = 5,
        Raining = 6,
        ThunderStorm = 7,
        Clearing = 8,
        Neutral = 9,
        Snowing = 10,
        Blizzard = 11,
        Snowlight = 12,
        Christmas = 13,
        Halloween = 14
    }

    public class WeatherManager
    {
        public static Weather[] weatherList = new Weather[] {
            Weather.ExtraSunny,
            Weather.Clear,
            Weather.Clouds,
            Weather.Smog,
            Weather.Foggy,
            Weather.Overcast,
            Weather.Raining,
            Weather.ThunderStorm,
            Weather.Clearing,
            Weather.Neutral,
            Weather.Snowing,
            Weather.Blizzard,
            Weather.Snowlight,
            Weather.Christmas,
            Weather.Halloween
        };

        public bool FirstWeatherChange = true;
        public static Weather CurrentWeather;
        public static Weather LastWeather;

        public WeatherManager()
        {
            GenerateWeather();

            Utils.DebugLine("WeatherManager Loaded!", "SWeatherManager");
        }

        private async void GenerateWeather()
        {
            DateTime TimeStamp = DateTime.Now.Add(TimeSpan.FromMilliseconds(ConfigManager.WeatherConfig.WeatherSwitchTime * 60000));
            Weather randomWeather = (Weather)new Random().Next(0, 14);

            if (FirstWeatherChange)
            {
                CurrentWeather = randomWeather;
                FirstWeatherChange = false;
            }
            else
            {
                LastWeather = CurrentWeather;
                CurrentWeather = randomWeather;
            }

            foreach (Session s in SessionManager.Sessions)
            {
                s.Player.TriggerEvent("Laced:SendClientWeather", Convert.ToInt32(CurrentWeather), Convert.ToInt32(LastWeather));
            }

            while (DateTime.Now < TimeStamp)
            {
                await BaseScript.Delay(1000);
            }

            GenerateWeather();
        }
    }
}
