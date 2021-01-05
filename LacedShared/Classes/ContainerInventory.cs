using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedShared.Classes
{
    public class ContainerInventory
    {
        [JsonProperty]
        public int OwnerID = -1;
        [JsonProperty]
        public int ContainerID;
        [JsonProperty]
        public string ContainerName;
        [JsonProperty]
        public int ContainerWeightLimit;
        [JsonProperty]
        public int ContainerWeight;
        [JsonProperty]
        public Dictionary<int, InventoryItem> Inventory = new Dictionary<int, InventoryItem>();

        public ContainerInventory(int _containerSize)
        {
            for (int i=0;i < _containerSize;i++)
            {
                Inventory.Add(i, new InventoryItem(i));
            }
        }
    }
}
