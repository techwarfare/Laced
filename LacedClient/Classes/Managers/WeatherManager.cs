using CitizenFX.Core;
using LacedShared.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedClient.Classes.Managers
{
    public class WeatherManager
    {
        public WeatherManager()
        {
            MainClient.GetInstance().RegisterEventHandler("Laced:SendClientWeather", new Action<int>(SendClientWeather));

            Utils.DebugLine("WeatherManager Loaded!", "CWeatherManager");
        }

        public void SendClientWeather(int _current)
        {
            World.TransitionToWeather((Weather)_current, 45f);
            World.NextWeather = (Weather)_current;

            Utils.DebugLine("Recieved client weather!", "CWeatherManager");
        }
    }
}
