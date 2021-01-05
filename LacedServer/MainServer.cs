namespace LacedServer
{
    using System;
    using CitizenFX.Core;
    using CitizenFX.Core.Native;
    using LacedShared.Libs;

    class MainServer : BaseScript
    {
        static MainServer _instance = null;

        public static MainServer GetInstance()
        {
            return _instance;
        }

        public static string ResouceName()
        {
            return API.GetCurrentResourceName();
        }

        public static string LoadResourceFile(string _resourceName, string _fileName)
        {
            return API.LoadResourceFile(_resourceName, _fileName);
        }

        public MainServer()
        {
            _instance = this;
            try
            {
                //Start loading server stuff
                Loader.Init();
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex);
            }
        }

        public void RegisterEventHandler(string _eventName, Delegate _action)
        {
            try
            {
                EventHandlers.Add(_eventName, _action);
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex);
            }
        }

        public void UnregisterEventHandler(string _eventName)
        {
            try
            {
                EventHandlers.Remove(_eventName);
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex);
            }
        }

        public void RegisterExport(string _exportName, Delegate _action)
        {
            try
            {
                Exports.Add(_exportName, _action);
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex);
            }
        }
        
        public ExportDictionary CallExport()
        {
            try
            {
                return Exports;
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex);
                return null;
            }
        }

        public void RegisterCommand(string _command, Delegate _action, bool _restricted)
        {
            API.RegisterCommand(_command, _action, _restricted);
        }
    }
}
