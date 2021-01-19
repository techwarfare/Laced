using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedShared.Models
{
    public class CardealerItem
    {
        [JsonProperty]
        public string CarName { get; protected set; }
        [JsonProperty]
        public string CarModel { get; protected set; }
        [JsonProperty]
        public int Price { get; protected set; }

        public CardealerItem()
        {

        }
        public override string ToString()
        {
            return $"CarName:{CarName};CarModel:{CarModel};CarPrice:{Price}";
        }
    }
}
