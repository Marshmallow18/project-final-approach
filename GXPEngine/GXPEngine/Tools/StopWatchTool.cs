using System;
using System.Collections.Generic;


public static class StopwatchTool
{
    static Dictionary<int, System.Diagnostics.Stopwatch> watches =
        new Dictionary<int, System.Diagnostics.Stopwatch>();

    static int nextId;

    public static int StartWatch()
    {
        int id = ++nextId;
        watches.Add(id, System.Diagnostics.Stopwatch.StartNew());
        return id;
    }

    public static void StopWatch(int id, string msg, bool ignoreDebug = false)
    {
        var watch = watches[id];
        watch.Stop();
        if (ignoreDebug || MyGame.Debug) Console.WriteLine($"{msg} | {watch.ElapsedMilliseconds} ms");

        watches.Remove(id);
    }

    public static void StopWatchTick(int id, string msg)
    {
        var watch = watches[id];
        watch.Stop();
        if (MyGame.Debug) Console.WriteLine($"{msg} | {watch.ElapsedTicks} ticks");

        watches.Remove(id);
    }
}