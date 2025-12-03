using MoonSharp.Interpreter;
using Platformer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class LuaEngine
{
    public Script script;
    private Dictionary<string, DynValue> loadedScripts = new Dictionary<string, DynValue>();

    public LuaEngine()
    {
        script = new Script();

        //this.RegisterFunction("print", (Action<string>)((input) => { Console.WriteLine(input); })); // Bruh I thought I had to define a print

        RegisterEnum<KnownColor>("Color");

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

        UserData.RegisterType<Entity>();
        UserData.RegisterType<PhysicsEntity>();
        var entityTable = DynValue.NewTable(script);
        entityTable.Table.Set("base", DynValue.NewCallback((ctx, args) =>
        {
            return UserData.Create(new Entity());
        }));
        entityTable.Table.Set("physics", DynValue.NewCallback((ctx, args) =>
        {
            return UserData.Create(new PhysicsEntity());
        }));
        entityTable.Table.Set("collision", DynValue.NewCallback((ctx, args) =>
        {
            return UserData.Create(new CollisionEntity());
        }));
        entityTable.Table.Set("player", DynValue.NewCallback((ctx, args) =>
        {
            return UserData.Create(new PlayerEntity());
        }));
        entityTable.Table.Set("platform", DynValue.NewCallback((ctx, args) =>
        {
            return UserData.Create(new PlatformEntity());
        }));
        script.Globals["Entity"] = entityTable;
    }

    public void RegisterObject(string name, object obj)
    {
        UserData.RegisterType(obj.GetType());
        script.Globals[name] = UserData.Create(obj);
    }
    public void RegisterFunction(string name, Delegate del)
    {
        script.Globals[name] = DynValue.NewCallback((context, args) =>
        {
            object[] realArgs = new object[args.Count];
            for (int i = 0; i < args.Count; i++)
                realArgs[i] = args[i].ToObject();

            var result = del.DynamicInvoke(realArgs);

            return DynValue.FromObject(script, result);
        });
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
