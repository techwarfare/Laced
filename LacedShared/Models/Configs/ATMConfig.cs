using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedShared.Models.Configs
{
    public class ATMConfig
    {
        [JsonProperty]
        public float ATMPosX { get; protected set; }
        [JsonProperty]
        public float ATMPosY { get; protected set; }
        [JsonProperty]
        public float ATMPosZ { get; protected set; }
        [JsonProperty]
        public float ATMHeading { get; protected set; }
    }
}
