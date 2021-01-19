namespace LacedClient
{
    using LacedClient.Classes.Managers;
    using LacedClient.Classes.Player;
    using LacedClient.Menus;
    using LacedShared.Libs;
    public class Loader
    {
        public static void Init()
        {
            Utils.DebugLine("Loader Initializing", "CLoader");
            //Initialize ConfigManager first as everything relies on it
            new ConfigManager();

            new TimeManager();
            new WeatherManager();

            new SpawnManager();

            new CharacterModifier();

            new SessionManager();

            new DriftCounter();

            Utils.DebugLine("Loader Initialized", "CLoader");
        }
    }
}
