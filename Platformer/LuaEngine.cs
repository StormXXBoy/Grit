using MoonSharp.Interpreter;
using Platformer;
using System;
using System.Collections.Generic;

public class LuaEngine
{
    public Script script;
    private Dictionary<string, DynValue> loadedScripts = new Dictionary<string, DynValue>();

    public LuaEngine()
    {
        script = new Script();

        //this.RegisterFunction("print", (Action<string>)((input) => { Console.WriteLine(input); })); // Bruh I thought I had to define a print

        UserData.RegisterType<Vector>();
        UserData.RegisterType<InputInfo>();

        var vectorTable = DynValue.NewTable(script);
        vectorTable.Table.Set("new", DynValue.NewCallback((ctx, args) =>
        {
            float x = 0, y = 0;

            if (args.Count > 0 && !args[0].IsNil()) x = Convert.ToSingle(args[0].ToObject());
            if (args.Count > 1 && !args[1].IsNil()) y = Convert.ToSingle(args[1].ToObject());

            return UserData.Create(new Vector(x, y));
        }));
        script.Globals["Vector"] = vectorTable;
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
