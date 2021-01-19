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
            RegisterGarageEventHandlers();
            RegisterInventoryEventHandlers();
            RegisterCardealerEventHandlers();
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
        private void RegisterGarageEventHandlers()
        {
            MainServer.GetInstance().RegisterEventHandler("Laced:GarageStoreVehicle", new Action<Player, string, string, NetworkCallbackDelegate>(GarageStoreVehicle));
            MainServer.GetInstance().RegisterEventHandler("Laced:RetrieveGarageVehicle", new Action<Player, string, int, NetworkCallbackDelegate>(RetrieveGarageVehicle));
        }
        private void RegisterCardealerEventHandlers()
        {
            MainServer.GetInstance().RegisterEventHandler("Laced:BuyCardealerVehicle", new Action<Player, string, string, NetworkCallbackDelegate>(BuyCardealerVehicle));
            MainServer.GetInstance().RegisterEventHandler("Laced:SellCardealerVehicle", new Action<Player, string, string, NetworkCallbackDelegate>(SellCardealerVehicle));
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
        private void GarageStoreVehicle([FromSource] Player _player, string _seshKey, string _garageItem, NetworkCallbackDelegate _networkCallback)
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

            GarageItem storinggarageItem = JsonConvert.DeserializeObject<GarageItem>(_garageItem);
            foreach (GarageItem gI in foundSesh.selectedCharacter.Garage.garageItems)
            {
                Utils.WriteLine($"Storing vehicle[{storinggarageItem.garageID}], garage vehicle [{gI.garageID}]");
                //Get the garage item in the servers memory
                if (storinggarageItem.garageID == gI.garageID)
                {
                    //Check if the garage item is not stored and is not impounded
                    if (ConfigManager.ServerConfig.RPImpound) { if (gI.impounded) { Utils.WriteLine("Vehicle is impounded!");  return; } }
                    if (!gI.stored)
                    {
                        gI.setImpounded(false);
                        gI.setStored(true);
                        gI.setNetworkID(0);
                        foundSesh.UpdateCharacters(CharacterDBManager.UpdateCharacter(_player, foundSesh.User, foundSesh.selectedCharacter.Id));
                        _networkCallback(true);
                        return;
                    }
                }
            }

            _networkCallback(false);
        }
        private void RetrieveGarageVehicle([FromSource] Player _player, string _seshKey, int garageID, NetworkCallbackDelegate _networkCallback)
        {
            if (_seshKey == null)
            {
                Utils.WriteLine("Session key is missing!");
                return;
            }

            Session foundSesh = Sessions.Find(s => s.Player.Handle == _player.Handle);
            if (foundSesh == null || foundSesh.SessionKey != _seshKey)
            {
                Utils.WriteLine("Session either doesn't exist or the session key doesn't match up");
                return;
            }

            foreach (GarageItem gI in foundSesh.selectedCharacter.Garage.garageItems)
            {
                if (gI.garageID == garageID)
                {
                    if (ConfigManager.ServerConfig.RPImpound)
                    {
                        gI.setImpounded(false);
                    }
                    else
                    {
                        gI.setImpounded(true);
                    }
                    
                    gI.setStored(false);
                    
                    foundSesh.UpdateCharacters(CharacterDBManager.UpdateCharacter(_player, foundSesh.User, foundSesh.selectedCharacter.Id));

                    _networkCallback(true, JsonConvert.SerializeObject(gI));
                    return;
                }
            }
            _networkCallback(false, null);
        }
        private void BuyCardealerVehicle([FromSource] Player _player, string _seshKey, string _vehicleModel, NetworkCallbackDelegate _networkCallback)
        {
            Utils.WriteLine("Buying Cardealer vehicle!");
            if (_seshKey == null)
            {
                Utils.WriteLine("Session key is missing!");
                return;
            }

            Session foundSesh = Sessions.Find(s => s.Player.Handle == _player.Handle);
            if (foundSesh == null || foundSesh.SessionKey != _seshKey)
            {
                Utils.WriteLine("Session either doesn't exist or the session key doesn't match up");
                return;
            }

            Utils.WriteLine("Searching markers");
            foreach (string key in ConfigManager.MarkerConfig.Keys)
            {
                //Find the cardealers within the markers
                if (key.ToLower().Contains("dealer"))
                {
                    foreach(string dataKey in ConfigManager.MarkerConfig[key].MarkerData.Keys)
                    {
                        if (dataKey.ToLower().Contains("vehicles"))
                        {
                            string markerDataString = JsonConvert.SerializeObject(ConfigManager.MarkerConfig[key].MarkerData[dataKey]);
                            Dictionary<string, CardealerItem> markerData = JsonConvert.DeserializeObject<Dictionary<string, CardealerItem>>(markerDataString);
                            
                            foreach (string dataKey2 in markerData.Keys)
                            {
                                if (dataKey2.ToLower().Contains(_vehicleModel))
                                {
                                    //We have found the vehicle
                                    //We need to save the car to the garage and the garage to the database
                                    string jsonString = JsonConvert.SerializeObject(markerData[dataKey2]);
                                    CardealerItem cardealerItem = JsonConvert.DeserializeObject<CardealerItem>(jsonString);
                                    if (!foundSesh.selectedCharacter.BuyItem(cardealerItem.Price))
                                    {
                                        Utils.WriteLine($"Player doesn't have enough money![{foundSesh.Player.Name}]");
                                        _networkCallback(false, null);
                                        return;
                                    }
                                    int garageID = foundSesh.selectedCharacter.Garage.garageItems.Count + 1;
                                    GarageItem garageItem = new GarageItem(garageID, foundSesh.selectedCharacter.Id, cardealerItem.CarName, cardealerItem.CarModel, Utils.CreateVehicleNumberPlate(), false, false, new Dictionary<string, int>());
                                    if (ConfigManager.ServerConfig.RPImpound)
                                    {
                                        garageItem.setImpounded(false);
                                    }
                                    else
                                    {
                                        garageItem.setImpounded(true);
                                    }
                                    garageItem.setStored(false);
                                    foundSesh.selectedCharacter.Garage.garageItems.Add(garageItem);
                                    foundSesh.UpdateCharacters(CharacterDBManager.UpdateCharacter(_player, foundSesh.User, foundSesh.selectedCharacter.Id));

                                    _networkCallback(true, JsonConvert.SerializeObject(garageItem));
                                }
                            }
                        }
                    }
                }
            }
        }
        private void SellCardealerVehicle([FromSource] Player _player, string _seshKey, string _sellgarageItem, NetworkCallbackDelegate _networkCallback)
        {
            if (_seshKey == null)
            {
                Utils.WriteLine("Session key is missing!");
                return;
            }

            Session foundSesh = Sessions.Find(s => s.Player.Handle == _player.Handle);
            if (foundSesh == null || foundSesh.SessionKey != _seshKey)
            {
                Utils.WriteLine("Session either doesn't exist or the session key doesn't match up");
                return;
            }

            GarageItem sellgarageItem = JsonConvert.DeserializeObject<GarageItem>(_sellgarageItem);
            foreach (GarageItem gI in foundSesh.selectedCharacter.Garage.garageItems)
            {
                if (gI.garageID == sellgarageItem.garageID)
                {
                    foreach (string key in ConfigManager.MarkerConfig.Keys)
                    {
                        //Find the cardealers within the markers
                        if (key.ToLower().Contains("dealer"))
                        {
                            //Loop through vehicle dealer marker data
                            foreach (string dataKey in ConfigManager.MarkerConfig[key].MarkerData.Keys)
                            {
                                //Get the vehicles
                                if (dataKey.ToLower().Contains("vehicles"))
                                {
                                    string markerDataString = JsonConvert.SerializeObject(ConfigManager.MarkerConfig[key].MarkerData[dataKey]);
                                    Dictionary<string, CardealerItem> markerData = JsonConvert.DeserializeObject<Dictionary<string, CardealerItem>>(markerDataString);

                                    foreach (string dataKey2 in markerData.Keys)
                                    {
                                        if (dataKey2.ToLower().Contains(gI.vehicleModel))
                                        {
                                            //We have found the vehicle
                                            //We need to save the car to the garage and the garage to the database
                                            string jsonString = JsonConvert.SerializeObject(markerData[dataKey2]);
                                            CardealerItem cardealerItem = JsonConvert.DeserializeObject<CardealerItem>(jsonString);

                                            foundSesh.selectedCharacter.SellItem(cardealerItem.Price);
                                            foundSesh.selectedCharacter.SellVehicle(gI);

                                            foundSesh.UpdateCharacters(CharacterDBManager.UpdateCharacter(_player, foundSesh.User, foundSesh.selectedCharacter.Id));
                                            Utils.WriteLine("Updated character!");

                                            try
                                            {
                                                _networkCallback(true, JsonConvert.SerializeObject(gI), cardealerItem.Price);
                                                return;
                                            }
                                            catch (Exception _ex)
                                            {
                                                Utils.Throw(_ex);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            _networkCallback(false, null, 0);
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

            foundSesh.UpdateCharacters(CharacterDBManager.UpdateCharacter(_player, foundSesh.User, character.Id));

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
