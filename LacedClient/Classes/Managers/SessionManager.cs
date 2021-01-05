namespace LacedClient.Classes.Managers
{
    using CitizenFX.Core;
    using CitizenFX.Core.Native;
    using CitizenFX.Core.UI;
    using LacedClient.Models;
    using LacedShared.Classes;
    using LacedShared.Libs;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Threading.Tasks;

    public class SessionManager
    {
        public bool NUIReady = false;

        public static Session PlayerSession;
        private static CommandManager CommandManager;
        private static InventoryManager InventoryManager;
        public SessionManager()
        {
            RegisterEventHandlers();
            

            MainClient.GetInstance().RegisterNUICallback("laced_interface_ready", SetNUIReady);

            CommandManager = new CommandManager();

            try
            {
                RegisterCharacterNUICallbacks();
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
        public void RegisterEventHandlers()
        {
            MainClient.GetInstance().RegisterEventHandler("onClientResourceStart", new Action<string>(OnClientResourceStart));
            MainClient.GetInstance().RegisterEventHandler("onClientResourceStop", new Action<string>(OnClientResourceStop));
            MainClient.GetInstance().RegisterEventHandler("Laced:EnableCharacterSelection", new Action<string>(EnableCharacterSelection));

            MainClient.GetInstance().RegisterEventHandler("Laced:UpdateCharacterSelection", new Action<string>(UpdateCharacterSelection));

            Utils.DebugLine("Registered Event Handlers!", "CSessionManager");
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
        private static async void NUIDisconnect(dynamic _data, CallbackDelegate _callback)
        {
            await MainClient.Delay(500);

            UnregisterCharacterNUICallbacks();
            _ = _callback(new { ok = true });
        }
    }
}
