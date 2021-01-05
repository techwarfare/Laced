using LacedShared.Libs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedShared.Classes
{
    public class InventoryItem
    {
        [JsonProperty]
        public int SlotID { get; protected set; }
        [JsonProperty]
        public string ItemName { get; protected set; }
        [JsonProperty]
        public int ItemWeight { get; protected set; }
        [JsonProperty]
        public int ItemAmount { get; protected set; }
        public InventoryItem(int _slotID)
        {
            SlotID = _slotID;
            ItemName = "";
            ItemWeight = -1;
            ItemAmount = -1;
        }
        public bool UseItem()
        {
            try
            {
                ItemAmount -= 1;

                return true;
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex);
                return false;
            }
        }
    }
}
