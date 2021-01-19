namespace LacedClient.Classes.Managers
{
    using CitizenFX.Core;
    using CitizenFX.Core.Native;
    using CitizenFX.Core.UI;
    using LacedClient.Models;
    using LacedShared.Classes;
    using LacedShared.Libs;
    using LacedShared.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Threading.Tasks;

    public class SessionManager
    {
        public static bool NUIReady = false;

        public static Session PlayerSession;
        private static CommandManager CommandManager;
        private static InventoryManager InventoryManager;
        public SessionManager()
        {
            RegisterEventHandlers();
            

            MainClient.GetInstance().RegisterNUICallback("laced_interface_ready", SetNUIReady);
            MainClient.GetInstance().RegisterNUICallback("laced_interface_close", CloseNUI);

            CommandManager = new CommandManager();

            try
            {
                RegisterCharacterNUICallbacks();
                RegisterCardealerNUICallbacks();
                RegisterGarageNUICallbacks();
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex);
            }

            
            Utils.DebugLine("SessionManager Loaded!", "CSessionManager");
        }
        ~SessionManager()
        {
            Utils.WriteLine("SessionManager was destroyed!");
        }
        private void SetNUIReady(dynamic _data, CallbackDelegate _callback)
        {
            Utils.DebugLine("Interface is ready", "CSessionManager");
            NUIReady = true;

            _ = _callback(new { ok = true });
        }
        private void CloseNUI(dynamic _data, CallbackDelegate _callback)
        {
            MainClient.GetInstance().SetNUIFocus(false, false);

            _ = _callback(new { ok = true });
        }
        public void RegisterEventHandlers()
        {
            MainClient.GetInstance().RegisterEventHandler("onClientResourceStart", new Action<string>(OnClientResourceStart));
            MainClient.GetInstance().RegisterEventHandler("onClientResourceStop", new Action<string>(OnClientResourceStop));
            MainClient.GetInstance().RegisterEventHandler("Laced:EnableCharacterSelection", new Action<string>(EnableCharacterSelection));

            MainClient.GetInstance().RegisterEventHandler("Laced:UpdateCharacterSelection", new Action<string>(UpdateCharacterSelection));

            Utils.DebugLine("Registered Event Handlers!", "CSessionManager");
        }
        public static void RegisterCardealerNUICallbacks()
        {
            MainClient.GetInstance().RegisterNUICallback("laced_cardealer_buy", BuyCardealerVehicle);
        }
        public static void RegisterGarageNUICallbacks()
        {
            MainClient.GetInstance().RegisterNUICallback("laced_garage_retrieve", RetrieveGarageVehicle);
        }
        public static void RegisterCharacterNUICallbacks()
        {
            MainClient.GetInstance().RegisterNUICallback("laced_character_create", CreateCharacter);
            MainClient.GetInstance().RegisterNUICallback("laced_character_select", SelectCharacter);
            MainClient.GetInstance().RegisterNUICallback("laced_character_delete", DeleteCharacter);

            MainClient.GetInstance().RegisterNUICallback("laced_disconnect", NUIDisconnect);

            Utils.DebugLine("Registered character NUI callbacks!", "CSessionManager");
        }
        public static void UnregisterCharacterNUICallbacks()
        {
            MainClient.GetInstance().UnregisterNUICallback("laced_character_create");
            MainClient.GetInstance().UnregisterNUICallback("laced_character_select");
            MainClient.GetInstance().UnregisterNUICallback("laced_character_delete");

            MainClient.GetInstance().UnregisterNUICallback("laced_disconnect");

            Utils.DebugLine("Unregistered character NUI callbacks!", "CSessionManager");
        }
        private async void OnClientResourceStart(string _resourceName)
        {
            if (_resourceName == MainClient.ResourceName())
            {
                Utils.DebugLine("Resource Started!", "CSessionManager");
                
                MainClient.GetInstance().SetNUIFocus(false, false);
                int maxThreshhold = 2000;
                int timeStamp = Game.GameTime;

                Screen.LoadingPrompt.Show("Loading Interface", LoadingSpinnerType.Clockwise3);

                Game.Player.CanControlCharacter = false;
                Game.Player.IsInvincible = true;
                do { await BaseScript.Delay(1000); } while ((timeStamp + maxThreshhold) > Game.GameTime);
                do { await BaseScript.Delay(1000); } while (!NUIReady);
                Screen.LoadingPrompt.Hide();

                API.ShutdownLoadingScreen();

                Game.Player.CanControlCharacter = true;
                Game.Player.IsInvincible = false;

                MainClient.TriggerServerEvent("Laced:CreateSession");
            }
        }
        private async void OnClientResourceStop(string _resourceName)
        {
            if (_resourceName != MainClient.ResourceName()) return;
            MainClient.TriggerServerEvent("Laced:Disconnect");
            await MainClient.Delay(500);
            PlayerSession = null;
        }
        private async void EnableCharacterSelection(string _characters)
        {
            Utils.DebugLine($"Enabling Character Selection {_characters}", "CSessionManager");
            World.RenderingCamera = World.CreateCamera(new Vector3(0f, 1500f, 500f), Vector3.Zero, 50);
            if (API.IsScreenFadedOut())
            {
                API.DoScreenFadeIn(500);

                while (!API.IsScreenFadedIn())
                {
                    await BaseScript.Delay(0);
                }
            }
            MainClient.GetInstance().SetNUIFocus(true, true);
            MainClient.GetInstance().SendNUIData("laced_character", "OpenMenu", _characters);
        }
        private void UpdateCharacterSelection(string _characters)
        {
            Utils.DebugLine("Updaing Character selection!", "CSessionManager");

            MainClient.GetInstance().SendNUIData("laced_character", "UpdateChars", _characters);
        }
        private static async void CreateCharacter(dynamic _data, CallbackDelegate _callback)
        {
            await MainClient.Delay(50);

            MainClient.TriggerServerEvent("Laced:CreateCharacter", _data.firstName, _data.lastName, _data.gender);

            _ = _callback(new { ok = true });
        }
        private static async void SelectCharacter(dynamic _data, CallbackDelegate _callback)
        {
            await MainClient.Delay(500);

            MainClient.GetInstance().SetNUIFocus(false, false);
            MainClient.TriggerServerEvent("Laced:SelectCharacter", _data.id);

            UnregisterCharacterNUICallbacks();
            _ = _callback(new { ok = true });
        }
        private static async void DeleteCharacter(dynamic _data, CallbackDelegate _callback)
        {
            await MainClient.Delay(500);

            MainClient.TriggerServerEvent("Laced:DeleteCharacter", _data.id);

            _ = _callback(new { ok = true });
        }
        private static async void BuyCardealerVehicle(dynamic _data, CallbackDelegate _callback)
        {
            Utils.WriteLine("Buying cardealer!");

            MainClient.TriggerServerEvent("Laced:BuyCardealerVehicle", PlayerSession.getSessionKey(), (string)_data.CarModel, new Action<bool, string>(async (_bought, _garageItem) => {
                if (_bought)
                {
                    Character playerCharacter = PlayerSession.getSelectedCharacter();
                    GarageItem garageItem = JsonConvert.DeserializeObject<GarageItem>(_garageItem);
                    Utils.WriteLine("Bought item!");
                    //Loop through all the garage items to get how many of these cars the character already has
                    int vehGarageId = 0;
                    foreach (GarageItem gI in playerCharacter.Garage.garageItems)
                    {
                        if (gI.vehicleModel == garageItem.vehicleModel)
                        {
                            vehGarageId++;
                        }
                    }
                    string vehGarageIdString = "";
                    if (vehGarageId > 0)
                    {
                        vehGarageIdString = vehGarageId.ToString();
                    }
                    //After the car is added into the garage, spawn it
                    uint vehicleHash = Helpers.GetVehicleHashFromString(garageItem.vehicleModel);

                    int vehicleID = await Helpers.SpawnVehicle(vehicleHash, garageItem.vehicleNumberPlate);
                    Utils.WriteLine($"Vehicle created ID:[{vehicleID}], Vehicle networkID: [{API.NetworkGetNetworkIdFromEntity(vehicleID)}]");
                    //Create the new garage item and add it into the character garage
                    garageItem.setNetworkID(API.NetworkGetNetworkIdFromEntity(vehicleID));
                    playerCharacter.Garage.garageItems.Add(garageItem);

                    if (vehicleID != -1)
                    {
                        _ = _callback(new { ok = true });
                    }
                    else
                    {
                        _ = _callback(new { ok = false });
                    }
                }
            }));

            MainClient.GetInstance().SetNUIFocus(false, false);

            await Task.FromResult(0);
        }
        private static void RetrieveGarageVehicle(dynamic _data, CallbackDelegate _callback)
        {
            int garageID = _data.garageID;
            foreach (GarageItem gI in PlayerSession.getSelectedCharacter().Garage.garageItems)
            {
                if (gI.garageID == garageID)
                {
                    if (!gI.stored || gI.impounded) { Utils.WriteLine("Vehicle not stored or is impounded!"); _ = _callback(new { ok = false }); return; }

                    MainClient.TriggerServerEvent("Laced:RetrieveGarageVehicle", PlayerSession.getSessionKey(), gI.garageID, new Action<bool, string>(async (_retrieved, _garageItem) => { 
                        if (_retrieved)
                        {
                            GarageItem garageItem = JsonConvert.DeserializeObject<GarageItem>(_garageItem);

                            gI.setImpounded(garageItem.impounded);
                            gI.setStored(garageItem.stored);
                            uint vehicleHash = Helpers.GetVehicleHashFromString(garageItem.vehicleModel);
                            int vehicleID = await Helpers.SpawnVehicle(vehicleHash, garageItem.vehicleNumberPlate);

                            gI.setNetworkID(API.NetworkGetNetworkIdFromEntity(vehicleID));
                            
                            if (vehicleID != -1)
                            {
                                _ = _callback(new { ok = true });
                            }
                            else
                            {
                                _ = _callback(new { ok = false});
                            }
                        }
                    }));
                }
            }
        }
        private static async void NUIDisconnect(dynamic _data, CallbackDelegate _callback)
        {
            await MainClient.Delay(500);
            UnregisterCharacterNUICallbacks();
            _ = _callback(new { ok = true });
        }
    }
}
