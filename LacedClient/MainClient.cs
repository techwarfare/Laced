namespace LacedClient
{
    using CitizenFX.Core;
    using System;
    using System.Threading.Tasks;
    using LacedShared.Libs;
    using CitizenFX.Core.Native;
    using Newtonsoft.Json;
    using System.Dynamic;

    public class MainClient : BaseScript
    {
        static MainClient _instance = null;
        static bool NUIMouseFocus = false;
        static bool NUIVisualFocus = false;
        public static MainClient GetInstance()
        {
            return _instance;
        }
        public static string ResourceName()
        {
            return API.GetCurrentResourceName();
        }
        public MainClient()
        {
            _instance = this;
            try
            {
                //Start loading client stuff, get us ready for server communication
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
                Utils.Throw(_ex, "Error with adding Event Handler!!");
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
                Utils.Throw(_ex, "Something went wrong when removing event handlers!!");
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
                Utils.Throw(_ex, "Something went wrong when registering exports!!");
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
                Utils.Throw(_ex, "Something went wrong when calling exports!");
                return null;
            }
        }

        public void RegisterTickHandler(Func<Task> _action)
        {
            try
            {
                Tick += _action;
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex, "Something went wrong when registering tick handlers!!");
            }
        }

        public void UnregisterTickHandler(Func<Task> _action)
        {
            try
            {
                Tick -= _action;
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex, "Something went wrong when unregistering tick handler!!");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_command">Without the + it will automatically do it</param>
        /// <param name="_description"></param>
        /// <param name="_controller"></param>
        /// <param name="_key"></param>
        public void RegisterKeyMapping(string _command, string _description, string _controller, string _key, Action _keydownAction, Action _keyupAction)
        {
            API.RegisterCommand("+"+_command, _keydownAction, false);
            API.RegisterCommand("-"+_command, _keyupAction, false);
            API.RegisterKeyMapping("+"+_command, _description, _controller, _key);
        }
        public Tuple<bool, bool> GetNUIFocus()
        {
            return Tuple.Create(NUIVisualFocus, NUIMouseFocus);
        }
        public void SetNUIFocus(bool _focus, bool _cursor)
        {
            API.SetNuiFocus(_focus, _cursor);
            
            NUIMouseFocus = _cursor;
            NUIVisualFocus = _focus;
        }

        public void SendNUIData(string _type, string _name, string _data = " ") => API.SendNuiMessage(JsonConvert.SerializeObject(new { type=_type, name=_name, data=_data }));
        
        public void RegisterNUICallback(string _type, Action<ExpandoObject, CallbackDelegate> _callback)
        {
            API.RegisterNuiCallbackType(_type);
            RegisterEventHandler($"__cfx_nui:{_type}", _callback);
        }
        public void UnregisterNUICallback(string _type)
        {
            UnregisterEventHandler($"__cfx_nui:{_type}");
        }
    }
}
