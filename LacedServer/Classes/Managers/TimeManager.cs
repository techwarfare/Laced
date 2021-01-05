namespace LacedServer.Classes.Managers
{
    using CitizenFX.Core;
    using LacedServer.Models;
    using LacedShared.Libs;
    using Newtonsoft.Json;
    using System;

    public class TimeManager
    {
        public static DateTime CurrentTime;

        public TimeManager()
        {
            CurrentTime = DateTime.Now;
            IncreaseSecond();

            Utils.DebugLine("TimeManager Loaded!", "STimeManager");
        }

        public async void IncreaseSecond()
        {
            DateTime TimeStamp = DateTime.Now;
            CurrentTime = CurrentTime.AddMinutes(1);

            try
            {
                foreach (Session s in SessionManager.Sessions)
                {
                    s.Player.TriggerEvent("Laced:SetClientTime", JsonConvert.SerializeObject(CurrentTime));
                }
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex);
            }

            while (DateTime.Now < TimeStamp.AddMinutes(1)) { await BaseScript.Delay(1000); }
            IncreaseSecond();
        }
    }
}
