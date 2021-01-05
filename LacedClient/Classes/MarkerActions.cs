using CitizenFX.Core;
using CitizenFX.Core.Native;
using LacedClient.Classes.Managers;
using LacedShared.Classes;
using LacedShared.Libs;
using LacedShared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedClient.Classes
{
    public class MarkerActions
    {
        public MarkerActions()
        {

        }
        public static void WarpPlayer(Marker _marker)
        {
            if (_marker.MarkerData == null) { return; }

            Vector3 telePos = (Vector3)_marker.MarkerData;

            try
            {
                Game.PlayerPed.Position = telePos;
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex);
            }
        }
        public static void WarpToMarker(Marker _marker)
        {
            if (_marker.MarkerData == null) { return; }
            //Loop through all markers configured and find which one to teleport too
            foreach (var v in ConfigManager.MarkerConfig)
            {
                if (v.Value.MarkerName.Replace(" ", "").ToLower() == _marker.MarkerData.ToString().Replace(" ", "").ToLower())
                {
                    Vector3 telePos = new Vector3(v.Value.MarkerPosX, v.Value.MarkerPosY, v.Value.MarkerPosZ);
                    try
                    {
                        Game.PlayerPed.Position = telePos;
                    }
                    catch (Exception _ex)
                    {
                        Utils.Throw(_ex);
                    }
                }
            }
        }
        public async static void EnableCardealerMenu(Marker _marker)
        {
            if (_marker == null || _marker.MarkerData == null)
            {
                MainClient.GetInstance().SetNUIFocus(true, true);
                MainClient.GetInstance().SendNUIData("laced_cardealer", "OpenMenu", JsonConvert.SerializeObject(_marker.MarkerData));
                await Task.FromResult(0);
            }
        }
        public async static void EnableGarageMenu(Marker _marker)
        {
            if (_marker == null || _marker.MarkerData == null)
            {
                MainClient.GetInstance().SetNUIFocus(true, true);
                MainClient.GetInstance().SendNUIData("laced_garage", "OpenMenu", JsonConvert.SerializeObject(_marker.MarkerData));
                await Task.FromResult(0);
            }
        }
        /// <summary>
        /// TO-DO
        /// Make mechanic menu a NUI menu
        /// </summary>
        /// <param name="_marker"></param>
        public async static void EnableMechanicMenu(Marker _marker)
        {
            if (_marker == null || _marker.MarkerData == null)
            {
                MainClient.GetInstance().SetNUIFocus(true, true);
                MainClient.GetInstance().SendNUIData("laced_mechanic", "OpenMenu", JsonConvert.SerializeObject(Game.PlayerPed.CurrentVehicle));
            }
        }
        public async static void GarageStoring(Marker _marker)
        {
            if (_marker == null || _marker.MarkerData == null) { return; }
        }
        public async static void SellVehicle(Marker _marker)
        {
            if (_marker == null) { return; }

            if (Game.Player.IsAlive && Game.Player.Character.IsInVehicle())
            {
                Vehicle plyVeh = Game.Player.Character.CurrentVehicle;
                int vehHandle = plyVeh.Handle;

                foreach (GarageItem gI in SessionManager.PlayerSession.getSelectedCharacter().Garage.garageItems)
                {
                    if (gI.vehicleNetworkID == plyVeh.NetworkId)
                    {
                        MainClient.TriggerServerEvent("Laced:SellVehicle", plyVeh.Model.Hash, gI.garageID, new Action<string>((seshKey) =>
                        {

                        }));

                        await MainClient.Delay(500);
                        API.DeleteEntity(ref vehHandle);

                        
                    }
                }
            }
        }
    }
}
