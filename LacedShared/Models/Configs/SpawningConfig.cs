using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedShared.Models.Configs
{
    public class SpawningConfig
    {
        [JsonProperty]
        public string SpawnLabel { get; protected set; }
        [JsonProperty]
        public float X { get; protected set; }
        [JsonProperty]
        public float Y { get; protected set; }
        [JsonProperty]
        public float Z { get; protected set; }
        [JsonProperty]
        public float H { get; protected set; }
    }
}
