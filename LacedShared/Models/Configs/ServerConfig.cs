using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedShared.Models.Configs
{
    public class ServerConfig
    {
        [JsonProperty]
        public bool DBConnection { get; protected set; }
        [JsonProperty]
        public string DBConnectionString { get; protected set; }
        [JsonProperty]
        public bool LiteDatabase { get; protected set; }
        [JsonProperty]
        public string LiteDatabasePath { get; protected set; }
        [JsonProperty]
        public bool ServerWhitelisted { get; protected set; }
        [JsonProperty]
        public int ServerConnectionDelayTime { get; protected set; }
        [JsonProperty]
        public bool UseSteamId { get; protected set; }
        [JsonProperty]
        public bool UseLicenseId { get; protected set; }
        [JsonProperty]
        public bool TrySaveSessionBeforeDC { get; protected set; }
        [JsonProperty]
        public int SaveSessionTime { get; protected set; }
        [JsonProperty]
        public bool EnableATMS { get; protected set; }
        //Might need the map name if we ever swap out maps
        [JsonProperty]
        public string MapName { get; protected set; }
        //RP impound is to check if we automatically impound vehicles when retrieving them from garage/buying vehicles
        [JsonProperty]
        public bool RPImpound { get; protected set; }
        [JsonProperty]
        public string[] DebugClasses { get; protected set; }
    }
}
