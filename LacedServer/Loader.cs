using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedServer
{
    using LacedServer.Classes.Managers;
    using LacedShared.Libs;
    public class Loader
    {
        
        public static void Init()
        {
            Utils.DebugLine("Loader Initializing!", "SLoader");

            //Start Config Manager first to get all server configurations for starting
            new ConfigManager();
            //Start connection manager incase players try to connect asap
            new ConnectionManager();
            
            new DatabaseManager();

            new TimeManager();
            new WeatherManager();

            new CommandManager();

            new SessionManager();
        }
    }
}
