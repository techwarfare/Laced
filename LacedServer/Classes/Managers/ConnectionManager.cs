namespace LacedServer.Classes.Managers
{
    using CitizenFX.Core;
    using LacedServer.Models;
    using LacedShared.Libs;
    using System;
    using System.Dynamic;
    using System.Threading.Tasks;

    public class ConnectionManager
    {
        public ConnectionManager()
        {
            MainServer.GetInstance().RegisterEventHandler("playerConnecting", new Action<Player, string, CallbackDelegate, ExpandoObject>(PlayerConnecting));
            
            Utils.DebugLine("Connection Manager Loaded!", "SConnectionManager");
        }

        private async void PlayerConnecting([FromSource] Player _player, string _playerName, dynamic _setKickReason, dynamic _deferrals)
        {
            Utils.DebugLine($"{_playerName} has started connecting!", "SConnectionManager");
            _deferrals.defer();

            _deferrals.update($"Loading");
            for (int i=0;i < ConfigManager.ServerConfig.ServerConnectionDelayTime; i++)
            {
                _deferrals.update($"Connecting, please wait. [{ConfigManager.ServerConfig.ServerConnectionDelayTime - i}]");
                await BaseScript.Delay(1000);
            }
            _deferrals.update($"Checking existing data");

            User plyUser = await PlayerDBManager.GetPlayerData(_player);

            await BaseScript.Delay(100);

            if (plyUser != null)
            {
                Utils.DebugLine($"Found {plyUser.Name} data!", "SConnectionManager");

                if (plyUser.BanData != null)
                {
                    if (plyUser.BanData.IsBanned)
                    {
                        _setKickReason("You are banned!");
                        _deferrals.update("You are banned!");
                        _deferrals.done("You are banned!");

                        return;
                    }
                }

                if (ConfigManager.ServerConfig.ServerWhitelisted)
                {
                    if (!plyUser.IsWhitelisted)
                    {
                        _setKickReason("You are not whitelisted");
                        _deferrals.update("You are not whitelisted!");
                        _deferrals.done("You are not whitelisted!");

                        return;
                    }

                    _deferrals.done();
                }

                _deferrals.update($"Found data for [{plyUser.Name}]");
                _deferrals.done();
            }
            else
            {
                int result = await PlayerDBManager.CreatePlayerData(_player);

                await BaseScript.Delay(100);
                if (result > 0)
                {
                    Utils.DebugLine($"Created {_player.Name} data in database!", "SConnectionManager");
                    if (ConfigManager.ServerConfig.ServerWhitelisted)
                    {
                        _deferrals.update($"Server is whitelisted only!");
                        _setKickReason("Server is whitelisted only");
                        _deferrals.done("Server is whitelisted only");
                        return;
                    }

                    _deferrals.done();
                }
                else
                {
                    Utils.WriteLine($"Couldn't create {_player.Name} data in database!");
                }
            }
        }
    }
}
