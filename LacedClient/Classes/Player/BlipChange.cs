using CitizenFX.Core;
using CitizenFX.Core.Native;
using LacedClient.Classes.Managers;
using LacedShared.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedClient.Classes.Player
{
    public class BlipChange
    {
        public BlipChange()
        {
            MainClient.GetInstance().RegisterTickHandler(ChangePlayerBlip);

            Utils.WriteLine("Blip Changer Initialized");
        }

        public async Task ChangePlayerBlip()
        {
            await Task.FromResult(0);

            int playerBlip = API.GetMainPlayerBlipId();
            
            if (Game.PlayerPed.IsInVehicle())
            {
                if (API.GetBlipSprite(playerBlip) != 225)
                {
                    API.SetBlipSprite(playerBlip, 225);
                }
            }
            else
            {
                if (API.GetBlipSprite(playerBlip) != 6)
                {
                    API.SetBlipSprite(playerBlip, 6);
                }
            }
        }
    }
}
