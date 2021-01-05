namespace LacedServer.Classes.Managers
{
    using System;
    using System.Collections.Generic;
    using CitizenFX.Core;
    using LacedServer.Models;
    using LacedShared.Classes;
    using LacedShared.Enums;
    using LacedShared.Libs;
    using LacedShared.Models;
    using Newtonsoft.Json;

    /// <summary>
    /// SessionManager, Manages every session made on the server.
    /// </summary>
    public class SessionManager
    {
        public static List<Session> Sessions = new List<Session>();
        public SessionManager()
        {
            MainServer.GetInstance().RegisterEventHandler("Laced:CreateSession", new Action<Player>(CreateSession));
            MainServer.GetInstance().RegisterEventHandler("playerDropped", new Action<Player, string>(PlayerDisconnected));
            RegisterCharacterEventHandlers();
            RegisterInventoryEventHandlers();
        }
        ~SessionManager()
        {
            Utils.WriteLine("SessonManager was destroyed!");
        }
        private void PlayerDisconnected([FromSource] Player _player, string _reason)
        {
            Utils.WriteLine($"Player disconnected! [{_player.Name}, {_reason}]");
            Session plySesh = Sessions.Find(s => s.Player.Handle == _player.Handle);
            Sessions.Remove(plySesh);
            plySesh = null;
            _player.Drop("Disconnected!");
        }
        private void RegisterInventoryEventHandlers()
        {
            MainServer.GetInstance().RegisterEventHandler("Laced:UseInventoryItem", new Action<Player, string, int, NetworkCallbackDelegate>(UseInventoryItem));
        }
        private void RegisterCharacterEventHandlers()
        {
            MainServer.GetInstance().RegisterEventHandler("Laced:CreateCharacter", new Action<Player, string, string, int>(CreateCharacter));
            MainServer.GetInstance().RegisterEventHandler("Laced:SelectCharacter", new Action<Player, int>(SelectCharacter));

            MainServer.GetInstance().RegisterEventHandler("Laced:FinishCharacterEditing", new Action<Player, string, string, NetworkCallbackDelegate>(FinishCharacterEditing));
        }
        private void CreateSession([FromSource] Player _player)
        {
            Session userSession = Sessions.Find(s => s.Player.Handle == _player.Handle);

            if (userSession == null)
            {
                Utils.DebugLine("Creating Player Session!", "SSesionManager");
                new Session().Initialize(_player);
            }
            else
            {
                Utils.WriteLine("Player Session is already in the server!");
                Sessions.Remove(userSession);
            }
        }
        private void CreateCharacter([FromSource] Player _player, string _firstName, string _lastName, int _gender)
        {
            Session foundSesh = Sessions.Find(s => s.Player.Handle == _player.Handle);
            if (foundSesh != null)
            {
                if (foundSesh.selectedCharacter != null)
                {
                    Utils.DebugLine("Couldn't create character! A character is already selected!", "SSessionManager");
                    return;
                }
                List<Character> newCharacters = CharacterDBManager.CreateCharacter(_player, foundSesh.User, _firstName, _lastName, (Genders)_gender);
                Utils.DebugLine(newCharacters.ToString(), "SSessionManager");
                foundSesh.UpdateCharacters(newCharacters);

                foundSesh.Player.TriggerEvent("Laced:UpdateCharacterSelection", JsonConvert.SerializeObject(newCharacters));
            }
        }
        private void SelectCharacter([FromSource] Player _player, int _charID)
        {
            Session foundSesh = Sessions.Find(s => s.Player.Handle == _player.Handle);
            if (foundSesh == null) return;

            if (foundSesh.selectedCharacter != null) { Utils.DebugLine("Couldn't select character! A character is already selected!", "SSessionManager"); return; }

            Character selectedCharacter = CharacterDBManager.SelectCharacter(_player, foundSesh.User, _charID);

            if (selectedCharacter != null)
            {
                //If the players character is not new then we can select the character server side
                if (!selectedCharacter.IsNew)
                {
                    foundSesh.selectedCharacter = selectedCharacter;
                }

                foundSesh.Player.TriggerEvent("Laced:SpawnPlayerCharacter", foundSesh.SessionKey, JsonConvert.SerializeObject(selectedCharacter));
            }
        }
        private void FinishCharacterEditing([FromSource] Player _player, string _sessionkey, string _characterString, NetworkCallbackDelegate _networkCallback)
        {
            if (_sessionkey == null)
            {
                Utils.WriteLine($"Player[{_player.Name}] didn't have a session key!");
                return;
            }
            Session foundSesh = Sessions.Find(s => s.Player.Handle == _player.Handle);

            if (foundSesh.SessionKey != _sessionkey)
            {
                Utils.WriteLine($"Players[{_player.Name}] session key didn't equal servers key!");
                return;
            }
            Utils.DebugLine("Player Character Finished Editing!", "SSessionManager");
            Character character = JsonConvert.DeserializeObject<Character>(_characterString);

            foundSesh.selectedCharacter = character;

            CharacterDBManager.UpdateCharacter(_player, foundSesh.User, character.Id);

            _networkCallback(JsonConvert.SerializeObject(character));
        }
        private void UseInventoryItem([FromSource] Player _player, string _seshKey, int _itemID, NetworkCallbackDelegate _networkCallback)
        {
            if (_seshKey == null)
            {
                Utils.WriteLine($"Player[{_player.Name}] didn't have a session key!");
                return;
            }
            Session foundSesh = Sessions.Find(s => s.Player.Handle == _player.Handle);

            if (foundSesh.SessionKey != _seshKey)
            {
                Utils.WriteLine($"Players[{_player.Name}] session key didn't equal servers key!");
                return;
            }

            InventoryItem foundItem = foundSesh.selectedCharacter.Inventory.InventoryItems[_itemID];

            if (foundItem != null || foundItem.ItemAmount > 0)
            {
                _networkCallback(JsonConvert.SerializeObject(foundItem));
            }
            else
            {
                Utils.WriteLine("Found Item was either null or none was left!");
            }
        }
    }
}
