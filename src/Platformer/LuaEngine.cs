using GritNetworking;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.CoreLib;
using MoonSharp.Interpreter.Loaders;
using MoonSharp.Interpreter.REPL;
using Platformer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

public class LuaClient
{
    private Netwerkr.NetwerkrClient _client;

    public LuaClient(Netwerkr.NetwerkrClient client)
    {
        _client = client;
    }

    public void listen(string eventName, DynValue callback)
    {
        if (callback.Type != DataType.Function)
            throw new ScriptRuntimeException("callback must be a function");

        _client.listen(eventName, data =>
        {
            // Call Lua function safely
            callback.Function.Call(data);
        });
    }

    public void fire(string eventName, string data)
    {
        _client.fire(eventName, data);
    }
}

public class LuaEngine
{
    public Script script;
    private Dictionary<string, DynValue> loadedScripts = new Dictionary<string, DynValue>();

    public LuaEngine()
    {
        script = new Script();

        UserData.RegisterType<LuaClient>();
        UserData.RegisterType<NetVector>();
        UserData.RegisterType<NetEntity>();
        var netTable = DynValue.NewTable(script);
        netTable.Table.Set("vector", DynValue.NewCallback((ctx, args) =>
        {
            float x = 0, y = 0;

            if (args.Count > 0 && !args[0].IsNil()) x = Convert.ToSingle(args[0].ToObject());
            if (args.Count > 1 && !args[1].IsNil()) y = Convert.ToSingle(args[1].ToObject());

            return UserData.Create(new NetVector(x, y));
        }));
        netTable.Table.Set("entity", createFunction((Func<DynValue>)(() => { return UserData.Create(new NetEntity()); })));
        netTable.Table.Set("join", DynValue.NewCallback((ctx, args) =>
        {
            string separator = args.Count > 0 ? args[0].CastToString() : "";
            List<string> parts = new List<string>();

            for (int i = 1; i < args.Count; i++)
            {
                var arg = args[i];

                if (arg.Type == DataType.UserData)
                {
                    var obj = arg.UserData.Object;

                    if (obj is NetVector v)
                        parts.Add("V|" + v.ToString());
                    else if (obj is NetEntity e)
                        parts.Add("E|" + e.ToString());
                    else
                        parts.Add(arg.ToString());
                }
                else
                {
                    parts.Add(arg.ToString());
                }
            }

            return DynValue.NewString(string.Join(separator, parts));
        }));
        netTable.Table.Set("split", DynValue.NewCallback((ctx, args) =>
        {
            string separator = args.Count > 0 ? args[0].CastToString() : "";
            string str = args.Count > 1 ? args[1].CastToString() : "";

            var splits = str.Split(new[] { separator }, StringSplitOptions.None);
            var table = DynValue.NewTable(script);

            int index = 1;
            foreach (var s in splits)
            {
                if (s.StartsWith("V|"))
                {
                    table.Table.Set(index++,
                        UserData.Create(new NetVector(s.Substring(2))));
                }
                else if (s.StartsWith("E|"))
                {
                    var entity = new NetEntity();
                    entity.UpdateFromString(s.Substring(2), true);
                    table.Table.Set(index++, UserData.Create(entity));
                }
                else
                {
                    table.Table.Set(index++, DynValue.NewString(s));
                }
            }

            return table;
        }));
        script.Globals["Net"] = netTable;

        var stringTable = script.Globals.Get("string").Table;
        stringTable.Set("split", DynValue.NewCallback((ctx, args) =>
        {
            string input = args[0].CastToString();
            string sep = args.Count > 1 ? args[1].CastToString() : " ";

            var table = new Table(script);

            var parts = input.Split(new string[] { sep }, StringSplitOptions.None);
            for (int i = 0; i < parts.Length; i++)
                table.Set(i + 1, DynValue.NewString(parts[i])); // Lua is 1-based

            return DynValue.NewTable(table);
        }));

        UserData.RegisterType<System.Drawing.Color>();
        RegisterEnum<KnownColor>("Color");
        RegisterFunction("color", (Func<KnownColor, Color>)((kc) => Color.FromKnownColor(kc)));

        //RegisterFunction("time", (Func<float>)(() => OsTimeModule));

        UserData.RegisterType<InputInfo>();
        RegisterEnum<InputState>("InputState");
        RegisterEnum<InputType>("InputType");
        RegisterEnum<MouseButtons>("MouseButton");
        RegisterEnum<Keys>("Key");

        UserData.RegisterType<Vector>();
        script.Globals["Vector"] = DynValue.NewCallback((ctx, args) =>
        {
            float x = 0, y = 0;

            if (args.Count > 0 && !args[0].IsNil()) x = Convert.ToSingle(args[0].ToObject());
            if (args.Count > 1 && !args[1].IsNil()) y = Convert.ToSingle(args[1].ToObject());

            return UserData.Create(new Vector(x, y));
        });

        UserData.RegisterType<Sprite>();
        UserData.RegisterType<Entity>();
        UserData.RegisterType<PhysicsEntity>();
        UserData.RegisterType<CollisionEntity>();
        UserData.RegisterType<PlayerEntity>();
        UserData.RegisterType<PlatformEntity>();
        var entityTable = DynValue.NewTable(script);
        entityTable.Table.Set("base", createFunction((Func<DynValue>)(() => { return UserData.Create(new Entity()); })));
        entityTable.Table.Set("physics", createFunction((Func<DynValue>)(() => { return UserData.Create(new PhysicsEntity()); })));
        entityTable.Table.Set("collision", createFunction((Func<DynValue>)(() => { return UserData.Create(new CollisionEntity()); })));
        entityTable.Table.Set("player", createFunction((Func<DynValue>)(() => { return UserData.Create(new PlayerEntity()); })));
        entityTable.Table.Set("platform", createFunction((Func<DynValue>)(() => { return UserData.Create(new PlatformEntity()); })));
        script.Globals["Entity"] = entityTable;
    }

    public void SetPack(string pack)
    {
        FileSystemScriptLoader newScriptLoader = new FileSystemScriptLoader();
        newScriptLoader.ModulePaths = new string[]
        {
                Path.Combine(
                    //Application.StartupPath,
                    "packs",
                    pack,
                    "scripts",
                    "?.lua"
                )
         };
        
        newScriptLoader.IgnoreLuaPathGlobal = true;

        script.Options.ScriptLoader = newScriptLoader;
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
        catch (ScriptRuntimeException ex)
        {
            Console.WriteLine("Lua function call error: " + ex.DecoratedMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Function call error: " + ex.Message);
        }
    }
}
