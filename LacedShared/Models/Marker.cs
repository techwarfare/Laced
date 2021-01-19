using CitizenFX.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedShared.Models
{
    public class Blip
    {
        [JsonProperty]
        public string BlipName { get; protected set; }
        [JsonProperty]
        public string BlipColor { get; protected set; }
    }
    public class Marker
    {
        public string MarkerKey { get; protected set; }
        public string MarkerName { get; protected set; }

        public Vector3 MarkerPos { get; protected set; }
        public string MarkerType { get; protected set; }
        public string MarkerAction { get; protected set; }
        public object MarkerData { get; protected set; }
        public Marker(string _markerKey, string _markerName, Vector3 _markerPos, string _markerType, string _markerAction = null, object _markerData = null)
        {
            MarkerKey = _markerKey;
            MarkerName = _markerName;
            MarkerPos = _markerPos;
            MarkerType = _markerType;
            MarkerData = _markerData;
            if (_markerAction != null)
            {
                MarkerAction = _markerAction;
            }
        }
    }
}
