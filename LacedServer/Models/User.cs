namespace LacedServer.Models
{
    using LacedShared.Enums;
    using System;
    public class BanStatus
    {
        public int BanID { get; protected set; }
        public bool IsBanned { get; set; } = false;
        public string Reason { get; set; } = "";
        public DateTime BanTime { get; set; }
        public string BannedBy { get; set; } = "";

        public BanStatus(int _banID)
        {
            BanID = _banID;
            IsBanned = true;
        }
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LicenseIdentifier { get; set; }
        public string SteamIdentifier { get; set; }
        public Permission GroupPerms { get; set; }
        public BanStatus BanData { get; set; }
        public bool IsWhitelisted { get; set; }
        public DateTime LastPlayed { get; set; }
    }
}
