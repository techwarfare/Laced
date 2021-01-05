namespace LacedClient.Classes.Managers
{
    using CitizenFX.Core;
    using CitizenFX.Core.Native;
    using LacedShared.Libs;
    using System;
    public class TimeManager
    {
        DateTime CurrentTime;
        public TimeManager()
        {
            MainClient.GetInstance().RegisterEventHandler("Laced:SetClientTime", new Action<string>(SetClientTime));

            Utils.DebugLine("TimeManager Loaded!", "CTimeManager");
        }

        public void SetClientTime(string _time)
        {
            CurrentTime = Newtonsoft.Json.JsonConvert.DeserializeObject<DateTime>(_time);

            if (CurrentTime.Hour > 23 || CurrentTime.Minute > 60 || CurrentTime.Second > 60)
            {
                return;
            }

            World.CurrentDayTime = CurrentTime.TimeOfDay;

            API.NetworkOverrideClockTime(CurrentTime.Hour, CurrentTime.Minute, CurrentTime.Second);
            Utils.DebugLine("Time was set!", "CTimeManager");
        }
    }
}
