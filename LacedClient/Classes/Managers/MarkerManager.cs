using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.NaturalMotion;
using LacedShared.Libs;
using LacedShared.Models;
using LacedShared.Models.Configs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedClient.Classes.Managers
{
    public class MarkerManager
    {
        static List<Marker> MarkerList = new List<Marker>();

        private static Dictionary<string, Delegate> MarkerTypes = new Dictionary<string, Delegate>()
        {
            ["WarpMarker"] = new Action<Marker>((_marker) => MarkerActions.WarpPlayer(_marker)),
            ["CardealerMarker"] = new Action<Marker>((_marker) => MarkerActions.EnableCardealerMenu(_marker)),
            ["GarageMarker"] = new Action<Marker>((_marker) => MarkerActions.EnableGarageMenu(_marker)),
            ["MechanicMarker"] = new Action<Marker>((_marker) => MarkerActions.EnableMechanicMenu(_marker)),
            ["WarpToMarker"] = new Action<Marker>((_marker) => MarkerActions.WarpToMarker(_marker)),
            ["GarageStoringMarker"] = new Action<Marker>((_marker) => MarkerActions.GarageStoring(_marker)),
            ["SellVehicleMarker"] = new Action<Marker>((_marker) => MarkerActions.SellVehicle(_marker))
        };
        public MarkerManager()
        {
            Utils.DebugLine("Marker Manager was created!", "CMarkerManager");
            MainClient.GetInstance().RegisterTickHandler(Draw3DMarkers);
        }
        ~MarkerManager()
        {
            MainClient.GetInstance().UnregisterTickHandler(Draw3DMarkers);
            Utils.DebugLine("Marker Manager was destroyed!", "CMarkerManager");
        }
        private async Task Draw3DMarkers()
        {
            if (Game.PlayerPed.IsAlive && Game.PlayerPed.IsVisible)
            {
                foreach (Marker m in MarkerList)
                {
                    if (World.GetDistance(Game.PlayerPed.Position, m.MarkerPos) < 25.0f)
                    {
                        Helpers.Draw3DText(m.MarkerPos, Color.FromArgb(255, 255, 255), m.MarkerName, CitizenFX.Core.UI.Font.ChaletComprimeCologne, 1);

                        World.DrawMarker(MarkerType.VerticalCylinder, m.MarkerPos, Vector3.Zero, Vector3.Zero, new Vector3(1.5f, 1.5f, 1.5f), Color.FromArgb(255, 200, 200, 200), false, true);
                    }

                    if (World.GetDistance(Game.PlayerPed.Position, m.MarkerPos) < 2.0f)
                    {
                        if (API.IsControlJustReleased(0, 46))
                        {
                            Action<Marker> markerAction = GetMarkerAction(m.MarkerAction);

                            markerAction(m);

                            await MainClient.Delay(500);
                        }
                    }
                }
            }
        }
        public static void TurnConfigToMarkers(Dictionary<string, MarkerConfig> _markerConfig)
        {
            foreach (var v in _markerConfig)
            {
                AddMarker(v.Key, v.Value.MarkerName, v.Value.MarkerActionType, new Vector3(v.Value.MarkerPosX, v.Value.MarkerPosY, v.Value.MarkerPosZ), v.Value.MarkerType, v.Value.MarkerData, v.Value.MarkerColor, v.Value.MarkerWaypointColor);
            }
        }
        private static void AddMarker(string _markerKey, string _markerName, string _markerActionType, Vector3 _markerPos, string _markerType, Dictionary<string, object> _markerData, string _markerColor, string _markerWaypointColor)
        {
            Utils.WriteLine($"Adding Marker[{_markerKey}]");
            MarkerType markerType = MarkerType.VerticalCylinder;
            BlipColor blipColor = BlipColor.Green;

            switch (_markerColor)
            {
                case "Green":
                    blipColor = BlipColor.Green;
                    break;
                default:
                    break;
            }

            switch (_markerWaypointColor)
            {
                case "Green":
                    blipColor = BlipColor.Green;
                    break;
                default:
                    break;
            }

            CitizenFX.Core.Blip MarkerBlip = World.CreateBlip(_markerPos);
            MarkerBlip.Color = blipColor;
            MarkerBlip.IsShortRange = true;
            MarkerBlip.Name = _markerName;

            /*//Get the 3D marker type of the marker (ie. VerticalCylinder)
            foreach (MarkerType mt in Enum.GetValues(typeof(MarkerType)))
            {
                if (mt.ToString() == _markerType)
                {
                    markerType = mt;
                }
            }*/

            object markerData = null;
            string stringPos = "";
            if (_markerData != null)
            {
                foreach (var v in _markerData)
                {
                    if (v.Key.ToLower().Contains("position"))
                    {
                        stringPos += v.Value + ",";
                    } else if (v.Key.ToLower().Contains("vehicles"))
                    {
                        markerData = v.Value;
                    } else if (v.Key.ToLower().Contains("markerteleport"))
                    {
                        markerData = v.Value;
                    }
                }
            }

            if (_markerActionType.ToLower() == "cardealermarker")
            {
                
                if (stringPos != "")
                {
                    string[] stringArr = stringPos.Split(',');
                    Vector3 dataPosition = new Vector3(int.Parse(stringArr[0]), int.Parse(stringArr[1]), int.Parse(stringArr[2]));
                    AddMarker(_markerKey+"_sell", "Sell Vehicle", "SellVehicleMarker", dataPosition, _markerType, null, _markerColor, _markerWaypointColor);
                }
            } else
            {
                if (stringPos != "")
                {
                    string[] stringArr = stringPos.Split(',');
                    Vector3 dataPosition = new Vector3(int.Parse(stringArr[0]), int.Parse(stringArr[1]), int.Parse(stringArr[2]));
                    markerData = dataPosition;
                }
            }

            Marker newMarker = new Marker(_markerKey, _markerName, _markerPos, _markerType, _markerActionType, markerData);

            MarkerList.Add(newMarker);
        }
        public Action<Marker> GetMarkerAction(string _markerActionType)
        {
            string searchTerm = _markerActionType.Replace(" ", "").ToLower();

            //Loop through the marker types to find which action the marker should use
            foreach (var v in MarkerTypes)
            {
                if (searchTerm.Contains(v.Key.ToLower()))
                {
                    return (Action<Marker>)MarkerTypes[v.Key];
                }
            }

            return null;
        }
    }
}
