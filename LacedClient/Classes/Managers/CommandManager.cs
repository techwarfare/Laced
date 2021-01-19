using CitizenFX.Core;
using CitizenFX.Core.Native;
using LacedShared.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedClient.Classes.Managers
{
    public class CommandManager
    {
        public CommandManager()
        {
            MainClient.GetInstance().RegisterEventHandler("Laced:AdminSpawnVehicle", new Action<string, string>(AdminSpawnVehicle));
            MainClient.GetInstance().RegisterEventHandler("Laced:ChangeCharacter", new Action<string, string>(ChangeCharacter));
            MainClient.GetInstance().RegisterEventHandler("Laced:GetCharacterPosition", new Action<string>(GetCharacterPosition));
            MainClient.GetInstance().RegisterEventHandler("Laced:AdminGiveWeapon", new Action<string, string>(AdminGiveWeapon));
        }
        private async void GetCharacterPosition(string _seshKey)
        {
            await Task.FromResult(0);
            if (!Helpers.CheckSessionKey(_seshKey)) { MainClient.TriggerServerEvent("Laced:SessionKeyError"); return; }

            Utils.WriteLine($"X: {Game.PlayerPed.Position.X}, Y:{Game.PlayerPed.Position.Y}, Z:{Game.PlayerPed.Position.Z}");
        }
        private async void AdminGiveWeapon(string _seshKey, string _vehicleModel)
        {
            if (!Helpers.CheckSessionKey(_seshKey)) { MainClient.TriggerServerEvent("Laced:SessionKeyError"); return; }

            await MainClient.Delay(0);
        }
        private async void AdminSpawnVehicle(string _seshKey, string _vehicleModel)
        {
            if (!Helpers.CheckSessionKey(_seshKey)) { MainClient.TriggerServerEvent("Laced:SessionKeyError"); return; }
            uint vehicleHash = Helpers.GetVehicleHashFromString(_vehicleModel);
            Utils.WriteLine(vehicleHash.ToString());
            if (!API.IsModelInCdimage(vehicleHash) || !API.IsModelAVehicle(vehicleHash))
            {
                Utils.WriteLine("Model doesn't exist!");
                return;
            }

            API.RequestModel(vehicleHash);

            while (!API.HasModelLoaded(vehicleHash))
            {
                Utils.DebugLine("Waiting for model to load!", "CCommandManager");
                await MainClient.Delay(500);
            }

            int playerPedID = API.PlayerPedId();
            Vector3 plyPos = Game.PlayerPed.Position;

            int vehicle = API.CreateVehicle(vehicleHash, plyPos.X, plyPos.Y, plyPos.Z, API.GetEntityHeading(playerPedID), true, false);
            await MainClient.Delay(50);
            API.SetPedIntoVehicle(playerPedID, vehicle, -1);

            API.SetEntityAsNoLongerNeeded(ref vehicle);

            API.SetModelAsNoLongerNeeded(vehicleHash);
        }
        private void ChangeCharacter(string _seshKey, string _characters)
        {
            if (!Helpers.CheckSessionKey(_seshKey)) { MainClient.TriggerServerEvent("Laced:SessionKeyError"); return; }

            if (SessionManager.PlayerSession.getSelectedCharacter() != null)
            {
                SessionManager.PlayerSession.ChangeCharacters();

                SessionManager.RegisterCharacterNUICallbacks();

                MainClient.TriggerEvent("Laced:EnableCharacterSelection", _characters);
            }
            else
            {
                Utils.WriteLine("You haven't selected a character!");
                return;
            }
        }
    }
}
