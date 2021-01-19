using CitizenFX.Core;
using CitizenFX.Core.Native;
using LacedServer.Models;
using LacedShared.Libs;
using LacedShared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedServer.Classes.Managers
{
    public class CommandManager
    {
        public CommandManager()
        {
            MainServer.GetInstance().RegisterCommand("spawncar", new Action<int, List<object>, string>(SpawnCar), false);
            MainServer.GetInstance().RegisterCommand("changecharacter", new Action<int, List<object>, string>(ChangeCharacter), false);
            MainServer.GetInstance().RegisterCommand("getpos", new Action<int, List<object>, string>(GetCharacterPosition), false);
            MainServer.GetInstance().RegisterCommand("giveweapon", new Action<int, List<object>, string>(GiveWeapon), false);
        }
        public void GetCharacterPosition(int _playerID, List<object> _args, string _raw)
        {
            if (_playerID > 0)
            {
                Session plySesh = SessionManager.Sessions.Find(s => s.Player.Handle == _playerID.ToString());
                MainServer.TriggerClientEvent(plySesh.Player, "Laced:GetCharacterPosition", plySesh.SessionKey);
            }
            else
            {
                Utils.WriteLine("Console cannont use this command!");
            }
        }
        public void GiveWeapon(int _playerID, List<object> _args, string _raw)
        {
            if (_playerID > 0)
            {
                if (_args.ToList().Count() > 0)
                {
                    Session plySesh = SessionManager.Sessions.Find(s => s.Player.Handle == _playerID.ToString());
                    MainServer.TriggerClientEvent(plySesh.Player, "Laced:AdminGiveWeapon", plySesh.SessionKey, _args[0].ToString());
                }
            }
        }
        public void SpawnCar(int _playerID, List<object> _args, string _raw)
        {
            if (_playerID > 0)
            {
                if (_args.ToList().Count() > 0)
                {
                    string vehicleModel = _args[0].ToString();
                    Utils.DebugLine($"Spawning vehicle [{vehicleModel}]", "SCommandManager");
                    Session plySesh = SessionManager.Sessions.Find(s => s.Player.Handle == _playerID.ToString());
                    MainServer.TriggerClientEvent(plySesh.Player, "Laced:AdminSpawnVehicle", plySesh.SessionKey, vehicleModel);
                }
            }
            else
            {
                Utils.WriteLine("Console cannot use this command!");
            }
        }
        public void ChangeCharacter(int _playerID, List<object> _args, string _raw)
        {
            if (_playerID > 0)
            {
                Session plySesh = SessionManager.Sessions.Find(s => s.Player.Handle == _playerID.ToString());
                if (plySesh.selectedCharacter == null)
                {
                    Utils.WriteLine("You don't have a character seleceted!");
                    return;
                }
                plySesh.selectedCharacter = null;
                MainServer.TriggerClientEvent(plySesh.Player, "Laced:ChangeCharacter", plySesh.SessionKey, JsonConvert.SerializeObject(plySesh.Characters));
            }
            else
            {
                Utils.WriteLine("Console cannot use this commnand!");
            }
        }
    }
}
