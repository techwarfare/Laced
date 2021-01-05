using CitizenFX.Core;
using CitizenFX.Core.Native;
using LacedServer.Classes.Managers;
using LacedShared.Libs;
using LacedShared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedServer.Models
{
    public class Session
    {
        public Player Player { get; protected set; }
        public User  User { get; protected set; }
        public int Ping => API.GetPlayerPing(Player.Handle);
        public int LastMsg => API.GetPlayerLastMsg(Player.Handle);
        public string EndPoint => API.GetPlayerEndpoint(Player.Handle);
        public void Drop(string _reason) => API.DropPlayer(Player.Handle, _reason);
        public bool requestedKey { get; protected set; } = false;
        public string SessionKey { get; protected set; }
        public List<Character> Characters { get; protected set; } = new List<Character>();
        public Character selectedCharacter { get; set; }
        public bool Disconnected { get; set; } = false;
        ~Session()
        {
            Utils.WriteLine($"Destroyed server session! [{this.Player.Name}]");
        }
        public async void Initialize([FromSource] Player _player)
        {
            Utils.DebugLine($"Initializing Session [{_player.Name}]", "SSession");

            SessionKey = Utils.CreateRandomKey();

            User userData = await PlayerDBManager.GetPlayerData(_player);

            if (userData != null)
            {
                this.User = userData;
                this.Player = _player;
                Characters = CharacterDBManager.GetCharacters(_player, userData);
            }
            else
            {
                Utils.WriteLine("User Data not found!");
                this.Drop("Data not found!");
                return;
            }

            SessionManager.Sessions.Add(this);

            Player.TriggerEvent("Laced:SendClientWeather", Convert.ToInt32(WeatherManager.CurrentWeather));
            Player.TriggerEvent("Laced:SetClientTime", JsonConvert.SerializeObject(TimeManager.CurrentTime));

            Player.TriggerEvent("Laced:SendClientConfigs", "spawns", JsonConvert.SerializeObject(ConfigManager.SpawningConfig));
            Player.TriggerEvent("Laced:SendClientConfigs", "controls", JsonConvert.SerializeObject(ConfigManager.ControlsConfig));
            Player.TriggerEvent("Laced:SendClientConfigs", "character", JsonConvert.SerializeObject(ConfigManager.CharacterConfig));
            Player.TriggerEvent("Laced:SendClientConfigs", "markers", JsonConvert.SerializeObject(ConfigManager.MarkerConfig));

            Player.TriggerEvent("Laced:EnableCharacterSelection", JsonConvert.SerializeObject(this.Characters));
        }

        public void UpdateCharacters(List<Character> _newCharacters)
        {
            this.Characters = _newCharacters;
        }
    }
}
