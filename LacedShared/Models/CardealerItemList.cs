using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedShared.Models
{
    public class CardealerItemList
    {
        [JsonProperty]
        public string CarName { get; protected set; }
        [JsonProperty]
        public Dictionary<string, dynamic> CarInfo { get; protected set; }
        public CardealerItemList()
        {

        }
    }
}
