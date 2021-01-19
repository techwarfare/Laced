using CitizenFX.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedShared.Classes
{
    public class GarageItem
    {
        [JsonProperty]
        public int garageID { get; protected set; }
        [JsonProperty]
        public int characterID { get; protected set; }
        [JsonProperty]
        public string vehicleName { get; protected set; }
        [JsonProperty]
        public string vehicleModel { get; protected set; }
        [JsonProperty]
        public string vehicleNumberPlate { get; protected set; }
        [JsonProperty]
        public int vehicleNetworkID { get; protected set; }
        [JsonProperty]
        public bool stored { get; protected set; }
        [JsonProperty]
        public bool impounded { get; protected set; }
        [JsonProperty]
        public Dictionary<string, int> vehicleMods { get; protected set; }

        public GarageItem()
        {

        }
        public GarageItem(int _garageID, int _characterID, string _vehicleName, string _vehicleModel, string _vehicleNumberPlate, bool _stored, bool _impounded, Dictionary<string, int> _vehicleMods)
        {
            garageID = _garageID;
            characterID = _characterID;
            vehicleName = _vehicleName;
            vehicleModel = _vehicleModel;
            vehicleNumberPlate = _vehicleNumberPlate;
            stored = _stored;
            impounded = _impounded;
            vehicleMods = _vehicleMods;
        }
        public void setImpounded(bool _val)
        {
            impounded = _val;
        }
        public void setStored(bool _val)
        {
            stored = _val;
        }
        public void setNetworkID(int _val)
        {
            vehicleNetworkID = _val;
        }
    }
    public class Garage
    {
        [JsonProperty]
        public int characterID { get; protected set; }
        [JsonProperty]
        public List<GarageItem> garageItems { get; protected set; }
        public Garage()
        {

        }
        public Garage(int _charID, List<GarageItem> _garageItems)
        {
            characterID = _charID;
            garageItems = _garageItems ?? new List<GarageItem>();
        }
    }
}
