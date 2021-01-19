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
        public static void DrawHUDtext(string _text, Color _color, Vector2 _screenPos, Vector2 _scale)
        {
            API.SetTextFont(1);
            API.SetTextScale(_scale.X, _scale.Y);
            API.SetTextColour(_color.R, _color.G, _color.B, _color.A);
            API.SetTextDropshadow(0, 0, 0, 0, _color.A);
            API.SetTextDropShadow();
            API.SetTextOutline();
            API.SetTextEntry("STRING");
            API.AddTextComponentString(_text);
            API.EndTextCommandDisplayText(_screenPos.X, _screenPos.Y);
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
        /// <summary>
        /// Spawn vehicle using vehicle hash, a number plate, and a spawn position. If none is set will use the players position
        /// </summary>
        /// <param name="_vehicleHash"></param>
        /// <param name="_spawnPos"></param>
        /// <returns></returns>
        public async static Task<int> SpawnVehicle(uint _vehicleHash, string _numberPlate, Vector3 _spawnPos = new Vector3())
        {
            if (!API.IsModelInCdimage(_vehicleHash) || !API.IsModelAVehicle(_vehicleHash))
            {
                Utils.WriteLine("Model doesn't exist!");
                return -1;
            }

            API.RequestModel(_vehicleHash);

            while (!API.HasModelLoaded(_vehicleHash))
            {
                Utils.DebugLine("Waiting for model to load!", "CCommandManager");
                await MainClient.Delay(500);
            }

            //If spawn pos is equal to a new vector then we use the players ped position to spawn the vehicle
            Vector3 spawnPos = _spawnPos;
            if (spawnPos == new Vector3())
            {
                spawnPos = Game.PlayerPed.Position;
            }

            int playerPedID = API.PlayerPedId();
            int vehicle = API.CreateVehicle(_vehicleHash, spawnPos.X, spawnPos.Y, spawnPos.Z, API.GetEntityHeading(playerPedID), true, false);
            await MainClient.Delay(50);
            API.SetVehicleNumberPlateText(vehicle, _numberPlate);
            API.SetPedIntoVehicle(playerPedID, vehicle, -1);

            //API.SetEntityAsNoLongerNeeded(ref vehicle);

            API.SetModelAsNoLongerNeeded(_vehicleHash);
            
            return vehicle;
        }
        /// <summary>
        /// Get the angle of the vehicle
        /// </summary>
        /// <param name="_vehID"></param>
        /// <returns>Angle and Velocity</returns>
        public static Tuple<double,double> VehicleAngle(int _vehID)
        {
            //Vehicles speed
            Vector3 vehVelocity = API.GetEntityVelocity(_vehID);
            double vehVel = Math.Sqrt((vehVelocity.X * vehVelocity.X) + (vehVelocity.Y * vehVelocity.Y));

            Vector3 vehRotation = API.GetEntityRotation(_vehID, 0);
            Utils.WriteLine(vehRotation.ToString());
            double vehSin = -Math.Sin(ConvertDegToRad(vehRotation.Z));
            double vehCos = Math.Cos(ConvertDegToRad(vehRotation.Z));
            
            if ((API.GetEntitySpeed(_vehID) * 3.6) < 30 || API.GetVehicleCurrentGear(_vehID) == 0) { return Tuple.Create(0.00, vehVel); }

            double cosX = (vehSin * vehVelocity.X + vehCos * vehVelocity.Y) / vehVel;
            if (cosX > 0.966 || cosX < 0) { return Tuple.Create(0.00, vehVel); }
            
            return Tuple.Create(ConvertRadToDeg(Math.Acos(cosX))*0.5, vehVel);
        }

        public static double ConvertRadToDeg(double _radians)
        {
            return (180 / Math.PI) * _radians;
        }
        public static double ConvertDegToRad(double _degrees)
        {
            return (Math.PI / 180) * _degrees;
        }
    }
}
