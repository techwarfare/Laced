namespace LacedClient
{
    using LacedClient.Classes.Managers;
    using LacedClient.Menus;
    using LacedShared.Libs;
    public class Loader
    {
        public static void Init()
        {
            Utils.DebugLine("Loader Initializing", "CLoader");
            new ConfigManager();

            new TimeManager();
            new WeatherManager();

            new SpawnManager();

            new CharacterModifier();

            new SessionManager();

            Utils.DebugLine("Loader Initialized", "CLoader");
        }
    }
}
