using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedShared.Models.Configs
{
    public class WeatherConfig
    {
        [JsonProperty]
        public int WeatherSwitchTime { get; protected set; }
    }
}
