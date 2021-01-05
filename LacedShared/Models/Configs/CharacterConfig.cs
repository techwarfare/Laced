using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedShared.Models.Configs
{
    public class CharacterConfig
    {
        [JsonProperty]
        public int MaxCharacters { get; protected set; }
        [JsonProperty]
        public object StarterInventory { get; protected set; }
        [JsonProperty]
        public float[] PrimaryWeaponOnePosition { get; protected set; }
        [JsonProperty]
        public float[] PrimaryWeaponTwoPosition { get; protected set; }
        [JsonProperty]
        public int StarterInventorySize { get; protected set; }
        [JsonProperty]
        public int StarterWallet { get; protected set; }
        [JsonProperty]
        public int StarterBank { get; protected set; }
        [JsonProperty]
        public bool UseBankFunds { get; protected set; }
    }
}
