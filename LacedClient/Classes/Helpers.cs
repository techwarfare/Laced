using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedClient.Classes
{
    using CitizenFX.Core;
    using CitizenFX.Core.Native;
    using CitizenFX.Core.UI;
    using LacedClient.Classes.Managers;
    using LacedShared.Libs;
    using System.Drawing;

    public class Helpers
    {
        public static void Draw3DText(Vector3 _pos, Color _color, string _text, Font _font, float _scale = 1f, bool _dropShadow = false)
        {
            Vector3 camCoord = API.GetGameplayCamCoord();
            
            //Get the distance between the cam coords (player cam) and the 3d text
            float dist = World.GetDistance(camCoord, _pos);
            float scale = (1 / dist) * 5;
            float fov = (1 / API.GetGameplayCamFov()) * 100;

            scale = scale * fov;

            API.SetTextScale(_scale * scale, _scale * scale);
            API.SetTextFont(Convert.ToInt32(_font));
            API.SetTextProportional(true);
            API.SetTextColour(_color.R, _color.G, _color.B, _color.A);

            if (_dropShadow)
            {
                API.SetTextDropshadow(1, 1, 1, 1, 255);
                API.SetTextDropShadow();
            }

            API.SetTextOutline();
            API.SetTextEntry("STRING");
            API.SetTextCentre(true);
            API.AddTextComponentString(_text);
            API.SetDrawOrigin(_pos.X, _pos.Y, _pos.Z, 0);
            API.DrawText(0f, 0f);
            API.ClearDrawOrigin();
        }
        public static uint GetVehicleHashFromString(string _vehicleString)
        {
            foreach (var v in Enum.GetValues(typeof(VehicleHash)))
            {
                if (v.ToString().ToLower() == _vehicleString.ToLower())
                {
                    return (uint)v;
                }   
            }

            return 0;
        }
        public static bool CheckSessionKey(string _seshKey)
        {
            if (_seshKey == null || _seshKey != SessionManager.PlayerSession.getSessionKey())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
