using CitizenFX.Core;
using CitizenFX.Core.Native;
using LacedClient.Menus;
using LacedClient.Models;
using LacedShared.Libs;
using LacedShared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedClient.Classes.Managers
{
    public class SpawnManager
    {
        public SpawnManager()
        {
            MainClient.GetInstance().RegisterEventHandler("Laced:SpawnPlayerCharacter", new Action<string, string>(SpawnPlayerCharacter));
            DisableAutoSpawn();
        }

        private void DisableAutoSpawn()
        {
            MainClient.GetInstance().CallExport()["spawnmanager"].setAutoSpawn(false);
        }

        private async void SpawnPlayerCharacter(string _sessionKey, string _character)
        {
            if (_sessionKey == null)
            {
                MainClient.TriggerServerEvent("Laced:SessionKeyMissing", "Spawning");
                return;
            }

            Character _spawnedCharacter = JsonConvert.DeserializeObject<Character>(_character);

            var model = new Model(_spawnedCharacter.Model);
            API.RequestModel((uint)model.Hash);
            while (!API.HasModelLoaded((uint)model.Hash))
            {
                Utils.DebugLine("Player Model Loading!", "CSpawnManager");
                await Game.Player.ChangeModel(model);
                await MainClient.Delay(1);
            }
            Utils.DebugLine($"Changed Player Model!", "CSpawnManager");

            //Get's the first spawn pos, aka the fresh spawn
            Vector3 SpawnPos = new Vector3(ConfigManager.SpawningConfig[0].X, ConfigManager.SpawningConfig[0].Y, ConfigManager.SpawningConfig[0].Z);
            float SpawnHeading = ConfigManager.SpawningConfig[0].H;

            //Setting players ped to look like the character
            Game.Player.Character.Style[PedComponents.Hair].SetVariation(_spawnedCharacter.Appearance.HairStyle, 0);
            Game.Player.Character.Style[PedComponents.Torso].SetVariation(_spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Torso)][0], _spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Torso)][1]);
            Game.Player.Character.Style[PedComponents.Legs].SetVariation(_spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Legs)][0], _spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Legs)][1]);
            Game.Player.Character.Style[PedComponents.Hands].SetVariation(_spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Hands)][0], _spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Hands)][1]);
            Game.Player.Character.Style[PedComponents.Shoes].SetVariation(_spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Shoes)][0], _spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Shoes)][1]);
            Game.Player.Character.Style[PedComponents.Special1].SetVariation(_spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Special1)][0], _spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Special1)][1]);
            Game.Player.Character.Style[PedComponents.Special2].SetVariation(_spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Special2)][0], _spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Special2)][1]);
            Game.Player.Character.Style[PedComponents.Special3].SetVariation(_spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Special3)][0], _spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Special3)][1]);
            Game.Player.Character.Style[PedComponents.Textures].SetVariation(_spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Textures)][0], _spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Textures)][1]);
            Game.Player.Character.Style[PedComponents.Torso2].SetVariation(_spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Torso2)][0], _spawnedCharacter.Clothing[Convert.ToInt32(PedComponents.Torso2)][1]);

            API.SetPedHeadBlendData(Game.Player.Character.Handle, _spawnedCharacter.Parents.Father, _spawnedCharacter.Parents.Mother, 0, _spawnedCharacter.Parents.Father, _spawnedCharacter.Parents.Mother, 0, _spawnedCharacter.Parents.Mix, _spawnedCharacter.Parents.Mix, -1, false);

            for (int i = 0; i < _spawnedCharacter.FaceFeatures.Count - 1; i++)
            {
                API.SetPedFaceFeature(Game.Player.Character.Handle, i, _spawnedCharacter.FaceFeatures[i]);
            }

            for (int i = 0; i < _spawnedCharacter.Appearance.Overlays.Count - 1; i++)
            {
                API.SetPedHeadOverlay(Game.Player.Character.Handle, i, _spawnedCharacter.Appearance.Overlays[i].Index, _spawnedCharacter.Appearance.Overlays[i].Opacity);

                if (_spawnedCharacter.Appearance.Overlays[i].IsHair)
                {
                    API.SetPedHeadOverlayColor(Game.Player.Character.Handle, i, 1, _spawnedCharacter.Appearance.HairColor, _spawnedCharacter.Appearance.HairHighlightColor);
                }
            }

            API.SetPedHairColor(Game.Player.Character.Handle, _spawnedCharacter.Appearance.HairColor, _spawnedCharacter.Appearance.HairHighlightColor);
            //Finished setting players ped looks
            if (_spawnedCharacter.IsNew)
            {
                Utils.DebugLine("Character is new!", "CSpawnManager");
                API.RequestCollisionAtCoord(SpawnPos.X, SpawnPos.Y, SpawnPos.Z);
                await MainClient.Delay(50);
                Game.Player.Character.Position = SpawnPos;
                Game.Player.Character.Heading = SpawnHeading;
                
                CharacterModifier.EnableModifier(_spawnedCharacter, _sessionKey);

                return;
            }
            else
            {
                if (_spawnedCharacter.IsDead)
                {
                    SpawnHeading = new Random().Next(0, 359);

                    SessionManager.PlayerSession = new Session(_sessionKey).InitializeSession(_spawnedCharacter);
                }
                else
                {
                    if (_spawnedCharacter.LastPos != null)
                    {
                        SpawnPos.X = _spawnedCharacter.LastPos[0];
                        SpawnPos.Y = _spawnedCharacter.LastPos[1];
                        SpawnPos.Z = _spawnedCharacter.LastPos[2];
                    }
                    
                    SpawnHeading = new Random().Next(0, 359);
                    SessionManager.PlayerSession = new Session(_sessionKey).InitializeSession(_spawnedCharacter);
                }
            }

            if (World.RenderingCamera != null) { World.RenderingCamera = null; }
            

            API.RequestCollisionAtCoord(SpawnPos.X, SpawnPos.Y, SpawnPos.Z);
            API.ClearPedTasksImmediately(API.PlayerPedId());

            API.FreezeEntityPosition(API.PlayerPedId(), false);

            Game.Player.Character.Position = SpawnPos;
            Game.Player.Character.Heading = SpawnHeading;
            Utils.DebugLine("Spawned Players Character!", "CSpawnManager");

            //MainClient.TriggerEvent("playerSpawned", SpawnPos);
        }
    }
}
