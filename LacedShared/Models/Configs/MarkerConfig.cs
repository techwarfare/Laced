using CitizenFX.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedShared.Models.Configs
{
    public class MarkerConfig
    {
        [JsonProperty]
        public string MarkerName { get; protected set; }
        [JsonProperty]
        public string MarkerActionType { get; protected set; }
        [JsonProperty]
        public string MarkerType { get; protected set; }
        [JsonProperty]
        public string MarkerIcon { get; protected set; }
        [JsonProperty]
        public string MarkerColor { get; protected set; }
        [JsonProperty]
        public string MarkerWaypointColor { get; protected set; }
        [JsonProperty]
        public float MarkerPosX { get; protected set; }
        [JsonProperty]
        public float MarkerPosY { get; protected set; }
        [JsonProperty]
        public float MarkerPosZ { get; protected set; }
        [JsonProperty]
        public Dictionary<string, object> MarkerData { get; protected set; }

        public MarkerConfig(string _markerName, string _markerActionType, string _markerIcon, Vector3 _markerPos, Dictionary<string, object> _markerData, string _markerColor, string _markerWaypointColor)
        {
            MarkerName = _markerName;
            MarkerActionType = _markerActionType;
            MarkerType = "VerticalCylinder";
            MarkerColor = _markerColor;
            MarkerWaypointColor = _markerWaypointColor;
            MarkerIcon = _markerIcon;
            MarkerPosX = (int)_markerPos.X;
            MarkerPosY = (int)_markerPos.Y;
            MarkerPosZ = (int)_markerPos.Z;
            MarkerData = _markerData;
        }
    }
}
