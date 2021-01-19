using CitizenFX.Core.Native;
using LacedClient.Classes.Managers;
using LacedShared.Libs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedClient.Classes.Player
{
    public class DriftCounter
    {
        private double DriftScore;
        private bool IsDrifiting;
        private int DriftTime;
        private int IdleTime;
        private int CurrentAlpha;
        private double ScreenScore = 0;
        public DriftCounter()
        {
            MainClient.GetInstance().RegisterTickHandler(DriftCounterTick);
        }
        private int RoundPoints(double _points)
        {
            int points = int.Parse(Math.Floor(_points).ToString());

            if (points < 0.01)
            {
                points = 0;
            }
            else if (_points > 999999999)
            {
                _points = 999999999;
            }
            return points;
        }
        private double CalculatePoints(double _points)
        {
            return RoundPoints(_points);
        }

        private async Task DriftCounterTick()
        {
            await Task.FromResult(0);
            int pedID = API.PlayerPedId();
            if (API.IsPedDeadOrDying(pedID, true)) { return; }

            int vehID = -1;
            try
            {
                vehID = API.GetVehiclePedIsIn(pedID, false);
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex);
            }
            if (vehID <= 0 ) { return; }

            int tick = API.GetGameTimer();
            

            //If peds vehicle is on it's wheels and they are not in a flying vehicle
            if (API.IsVehicleOnAllWheels(vehID) && !API.IsPedInFlyingVehicle(pedID))
            {
                //Now we can start calculating the angle
                Tuple<double, double> vehicleAngles = Helpers.VehicleAngle(vehID);

                bool StopDrifting = tick - (IdleTime) < 1850;

                if (!StopDrifting && DriftScore > 0)
                {
                    Utils.WriteLine("Stopped drifitng!");
                    //We've stopped drifitng
                    int totalScore = int.Parse(CalculatePoints(DriftScore).ToString());
                    int cashAmount = totalScore / 400;
                    cashAmount = RoundPoints(cashAmount);
                    MainClient.TriggerServerEvent("Laced:DriftFinished", SessionManager.PlayerSession.getSessionKey(), cashAmount);

                    DriftScore = 0;
                    ScreenScore = 0;
                }

                if (vehicleAngles.Item1 != 0)
                {
                    if (DriftScore == 0)
                    {
                        IsDrifiting = true;
                        DriftTime = tick;
                    }

                    if (StopDrifting)
                    {
                        DriftScore = DriftScore + Math.Floor(vehicleAngles.Item1 * vehicleAngles.Item2);
                    }
                    else
                    {
                        DriftScore = Math.Floor(vehicleAngles.Item1 * vehicleAngles.Item2);
                    }
                    ScreenScore = CalculatePoints(DriftScore);

                    IdleTime = tick;
                }
            }

            if (tick - (IdleTime) < 3000)
            {
                if (CurrentAlpha < 255 && CurrentAlpha+10 < 255)
                {
                    CurrentAlpha = CurrentAlpha + 10;
                }
                else if (CurrentAlpha > 255)
                {
                    CurrentAlpha = 255;
                } else if (CurrentAlpha == 255)
                {
                    CurrentAlpha = 255;
                } else if (CurrentAlpha == 250)
                {
                    CurrentAlpha = 255;
                }
            }
            else
            {
                if (CurrentAlpha > 0 && CurrentAlpha-10 > 0)
                {
                    CurrentAlpha = CurrentAlpha - 10;
                }else if (CurrentAlpha < 0)
                {
                    CurrentAlpha = 0;
                }else if (CurrentAlpha == 5)
                {
                    CurrentAlpha = 0;
                }
            }
            if (ScreenScore == 0) { return; }
            Helpers.DrawHUDtext($"\n"+ScreenScore.ToString(), Color.FromArgb(255, 191, 0, CurrentAlpha), new CitizenFX.Core.Vector2(0.5f, 0.0f), new CitizenFX.Core.Vector2(0.7f, 0.7f));
        }
    }
}
