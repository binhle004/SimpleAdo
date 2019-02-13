using Newtonsoft.Json;
using System;

internal static class ConsoleExtensions
{
    internal static void WriteToConsole(this object item)
    {
        Console.WriteLine(JsonConvert.SerializeObject(item, Formatting.Indented));
    }
}