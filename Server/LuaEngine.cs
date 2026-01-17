using GritNetworking;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.CoreLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;

namespace Server
{
    public class LuaServer
    {
        private Netwerkr.NetwerkrServer _server;

        public LuaServer(Netwerkr.NetwerkrServer server)
        {
            _server = server;
        }

        public void listen(string eventName, DynValue callback)
        {
            if (callback.Type != DataType.Function)
                throw new ScriptRuntimeException("callback must be a function");

            _server.listen(eventName, (clientId, data) =>
            {
                callback.Function.Call(clientId, data);
            });
        }

        public void fireAllClients(string eventName, string data)
        {
            _server.fireAllClients(eventName, data);
        }

        public void fireClient(string clientId, string eventName, string data)
        {
            _server.fireClient(clientId, eventName, data);
        }
    }

    public class LuaEngine
    {
        public Script script;
        private Dictionary<string, DynValue> loadedScripts = new Dictionary<string, DynValue>();

        public LuaEngine()
        {
            script = new Script();

            UserData.RegisterType<LuaServer>();
            UserData.RegisterType<NetVector>();
            UserData.RegisterType<NetEntity>();
            var netTable = DynValue.NewTable(script);
            netTable.Table.Set("vector", createFunction((Func<DynValue>)(() => { return UserData.Create(new NetVector()); })));
            netTable.Table.Set("entity", createFunction((Func<DynValue>)(() => { return UserData.Create(new NetEntity()); })));
            script.Globals["Net"] = netTable;
        }

        public void RegisterObject(string name, object obj)
        {
            UserData.RegisterType(obj.GetType());
            script.Globals[name] = UserData.Create(obj);
        }
        public void RegisterFunction(string name, Delegate del)
        {
            script.Globals[name] = createFunction(del);
        }

        public void RegisterEnum<T>(string name) where T : Enum
        {
            UserData.RegisterType<T>();

            Table enumRoot = script.Globals.Get("Enum").Table;
            if (enumRoot == null)
            {
                enumRoot = new Table(script);
                script.Globals["Enum"] = enumRoot;
            }

            var table = new Table(script);

            foreach (var value in Enum.GetValues(typeof(T)))
            {
                table.Set(value.ToString(), UserData.Create(value));
            }

            enumRoot[name] = table;
        }

        public DynValue createFunction(Delegate del)
        {
            return DynValue.NewCallback((context, args) =>
            {
                object[] realArgs = new object[args.Count];
                for (int i = 0; i < args.Count; i++)
                    realArgs[i] = args[i].ToObject();

                var result = del.DynamicInvoke(realArgs);

                return DynValue.FromObject(script, result);
            });
        }

        public void RunFile(string path)
        {
            try
            {
                loadedScripts[path] = script.DoFile(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lua error: " + ex.Message);
            }
        }

        public void RunString(string lua)
        {
            try
            {
                script.DoString(lua);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lua error: " + ex.Message);
            }
        }

        public void Call(string functionName, params object[] args)
        {
            try
            {
                var fn = script.Globals.Get(functionName);
                if (fn.IsNil()) return;

                script.Call(fn, args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lua function error: " + ex.Message);
            }
        }
    }
}