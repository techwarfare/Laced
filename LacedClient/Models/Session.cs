using LacedClient.Classes.Managers;
using LacedClient.Classes.Player;
using LacedShared.Libs;
using LacedShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedClient.Models
{
    public class Session
    {
        public static Character SelectedCharacter { get; protected set; }
        private string SessionKey { get; }
        public MarkerManager MarkerManager { get; protected set; }
        public InventoryManager InventoryManager { get; protected set; }
        public string getSessionKey()
        {
            return SessionKey;
        }
        public Session(string _seshKey)
        {
            Utils.DebugLine($"Session Created! [{_seshKey}]", "CSession");
            MarkerManager = new MarkerManager();
            SessionKey = _seshKey;

            new BlipChange();
        }
        ~Session()
        {
            Utils.WriteLine("Session was destroyed!");
        }
        public Session InitializeSession(Character _selectedCharacter)
        {
            Utils.DebugLine("Initializing Session!", "CSession");
            SelectedCharacter = _selectedCharacter;
            
            MarkerManager.TurnConfigToMarkers(ConfigManager.MarkerConfig);

            InventoryManager = new InventoryManager(_selectedCharacter);

            return this;
        }
        public void ChangeCharacters()
        {
            SelectedCharacter = null;
            MarkerManager = null;

            GC.Collect();
        }
        public Character getSelectedCharacter()
        {
            return SelectedCharacter;
        }
    }
}
