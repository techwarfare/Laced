using CitizenFX.Core;
using LacedShared.Classes;
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
    public class InventoryManager
    {
        private Character PlayerCharacter;
        private static Dictionary<string, Delegate> ItemActions = new Dictionary<string, Delegate>()
        {
            ["waterbottle"] = new Action<InventoryItem>((_inventoryItem) => { SessionManager.PlayerSession.getSelectedCharacter().IncreaseThirst(50); })
        };
        public InventoryManager(Character _selectedCharacter)
        {
            PlayerCharacter = _selectedCharacter;

            MainClient.GetInstance().RegisterKeyMapping("inventory", "Open the inventory", "keyboard", "i", new Action(InventoryOpen), new Action(() => { }));
        }
        public void RegisterNUICallbacks()
        {
            MainClient.GetInstance().RegisterNUICallback("laced_inventory_close", CloseInventory);

            MainClient.GetInstance().RegisterNUICallback("laced_inventory_use", UseItem);
            MainClient.GetInstance().RegisterNUICallback("laced_inventory_take", TakeItem);
        }
        public void UnregisterNUICallbacks()
        {
            MainClient.GetInstance().UnregisterNUICallback("laced_inventory_close");
            MainClient.GetInstance().UnregisterNUICallback("laced_inventory_use");
            MainClient.GetInstance().UnregisterNUICallback("laced_inventory_take");
        }
        private async void UseItem(dynamic _data, CallbackDelegate _callback)
        {
            await MainClient.Delay(500);
            MainClient.GetInstance().SetNUIFocus(false, false);
            foreach (var v in _data)
            {
                Utils.WriteLine(v);
            }
            if (_data.slotid == null) { return; }
            //InventoryItem inventoryItem = PlayerCharacter.Inventory.InventoryItems[_data.ItemID];
            Utils.WriteLine("Triggering server event!");
            //Send invetory id over to server and the server will use the item
            MainClient.TriggerServerEvent("Laced:UseInventoryItem", SessionManager.PlayerSession.getSessionKey(), _data.slotid, new Action<string>(UseItemCallback));
            Utils.WriteLine("Triggering callback!");
            _ = _callback(new { ok = true });
        }
        private void TakeItem(dynamic _data, CallbackDelegate _callback)
        {
            //Send container id and item id that we wanna take
            MainClient.TriggerServerEvent("Laced:TakeContainerItem", SessionManager.PlayerSession.getSessionKey(), _data.ContainerID, _data.ItemID, new Action<bool,string,int>(TakeItemCallback));

            _callback(new { ok = true });
        }
        private void UseItemCallback(string _item)
        {
            InventoryItem inventoryItem = JsonConvert.DeserializeObject<InventoryItem>(_item);

            foreach (var v in ItemActions)
            {
                if (v.Key == inventoryItem.ItemName.ToLower())
                {
                    Action itemAction = (Action)ItemActions[v.Key];

                    itemAction();
                    InventoryItem CharacterItem = SessionManager.PlayerSession.getSelectedCharacter().Inventory.InventoryItems[inventoryItem.SlotID];
                    if (inventoryItem.ItemAmount == 1)
                    {
                        //Reset the item to null/nothing since we are using it all
                        SessionManager.PlayerSession.getSelectedCharacter().Inventory.InventoryItems[inventoryItem.SlotID] = new InventoryItem(inventoryItem.SlotID);
                    }
                    else
                    {
                        if (SessionManager.PlayerSession.getSelectedCharacter().Inventory.InventoryItems[inventoryItem.SlotID].UseItem())
                        {
                            Utils.WriteLine($"Used Item [{inventoryItem.ItemName}] [{inventoryItem.ItemAmount}]");

                        }
                        else
                        {
                            Utils.WriteLine($"Couldn't use item![{inventoryItem.ItemName}]");
                        }
                    }
                }
            }
        }
        private void TakeItemCallback(bool _canTakeItem, string _itemToTake, int _slotID)
        {
            if (_canTakeItem)
            {
                InventoryItem itemToTake = JsonConvert.DeserializeObject<InventoryItem>(_itemToTake);
                SessionManager.PlayerSession.getSelectedCharacter().Inventory.InventoryItems[_slotID] = itemToTake;
            }
        }
        private void CloseInventory(dynamic _data, CallbackDelegate _callback)
        {
            MainClient.GetInstance().SetNUIFocus(false, false);

            UnregisterNUICallbacks();

            _callback(new { ok = true });
        }
        private void InventoryOpen()
        {
            RegisterNUICallbacks();
            MainClient.GetInstance().SetNUIFocus(true, true);

            List<InventoryItem> ContainerInventory = new List<InventoryItem>();


            MainClient.GetInstance().SendNUIData("laced_inventory", "OpenMenu", JsonConvert.SerializeObject(new { PlayerCharacter.Inventory, ContainerInventory }));
        }
    }
}
