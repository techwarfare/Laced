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
            //Make sure that the marker and the marker data (i.e. cardealer vehicles) are not null
            if (_marker != null && _marker.MarkerData != null)
            {
                if (!Game.PlayerPed.IsInVehicle() || !Game.Player.IsAlive) { Utils.WriteLine("Not in a vehicle or dead!"); return; }

                MainClient.GetInstance().SetNUIFocus(true, true);
                Utils.WriteLine(_marker.MarkerData.ToString());
                MainClient.GetInstance().SendNUIData("laced_cardealer", "OpenMenu", JsonConvert.SerializeObject(_marker.MarkerData));
                await Task.FromResult(0);
            }
        }
        public async static void EnableGarageMenu(Marker _marker)
        {
            if (_marker != null)
            {
                if (Game.PlayerPed.IsInVehicle() || !Game.Player.IsAlive) { Utils.WriteLine("In a vehicle or dead!"); return; }

                MainClient.GetInstance().SetNUIFocus(true, true);
                MainClient.GetInstance().SendNUIData("laced_garage", "OpenMenu", JsonConvert.SerializeObject(SessionManager.PlayerSession.getSelectedCharacter().Garage.garageItems));
                await Task.FromResult(0);
            }
        }
        public async static void EnableMechanicMenu(Marker _marker)
        {
            if (_marker == null || _marker.MarkerData == null)
            {
                
            }
        }
        public async static void GarageStoring(Marker _marker)
        {
            if (_marker == null) { return; }

            if (!Game.PlayerPed.IsInVehicle() || !Game.Player.IsAlive) { Utils.WriteLine("Not in a vehicle or dead!"); return; }
            await Task.FromResult(0);
            //Get the current vehicle of the character
            Vehicle plyVeh = Game.Player.Character.CurrentVehicle;
            Utils.WriteLine($"Vehicle handle:[{plyVeh.Handle}], Vehicle ID:[{plyVeh.NetworkId}]");
            //Loop through all of the characters garage items to check if the current vehicle is inside their garage
            foreach (GarageItem gI in SessionManager.PlayerSession.getSelectedCharacter().Garage.garageItems)
            {
                Utils.WriteLine($"Garage network id:[{gI.vehicleNetworkID}]");
                //If the network ID is the same between both items then we know the character has their own car
                if (gI.vehicleNetworkID == plyVeh.NetworkId)
                {
                    //Send the 
                    MainClient.TriggerServerEvent("Laced:GarageStoreVehicle", SessionManager.PlayerSession.getSessionKey(), JsonConvert.SerializeObject(gI), new Action<bool>((_stored) => { 
                        //Networkcallback, if the server returned with true then we can set if the car is stored and impounded
                        if (_stored)
                        {
                            GarageItem updatedItem = SessionManager.PlayerSession.getSelectedCharacter().Garage.garageItems.Find(GI => GI.garageID == gI.garageID);
                            updatedItem.setImpounded(false);
                            updatedItem.setStored(true);
                            updatedItem.setNetworkID(0);
                            plyVeh.Delete();
                        }
                        else
                        {
                            Utils.WriteLine("Something went wrong when storing the vehicle!");
                        }
                    }));
                }
            }
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
                        MainClient.TriggerServerEvent("Laced:SellCardealerVehicle", SessionManager.PlayerSession.getSessionKey(), JsonConvert.SerializeObject(gI), new Action<bool, string, int>((_sold, _garageItem, _price) =>
                        {
                            if (_sold)
                            {
                                Utils.WriteLine("Selling vehicle!");
                                GarageItem garageItem = JsonConvert.DeserializeObject<GarageItem>(_garageItem);
                                SessionManager.PlayerSession.getSelectedCharacter().SellVehicle(garageItem);
                                Utils.WriteLine("Removed vehicle!");
                                SessionManager.PlayerSession.getSelectedCharacter().SellItem(_price);
                                Utils.WriteLine("Sold vehicle!");
                                API.DeleteEntity(ref vehHandle);
                            }
                        }));
                    }
                }
            }
        }
    }
}
