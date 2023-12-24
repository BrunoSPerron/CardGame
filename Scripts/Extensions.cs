using Newtonsoft.Json;
using System;
using System.Collections.Generic;


public static class Extensions
{
    private readonly static Random rng = new Random();

    ///<returns>The object serialized in a Json encoded string.</returns>
    public static string Serialize<T>(this T source)
    {
        return JsonConvert.SerializeObject(source);
    }

    /// <summary> Based on Fisher-Yates. </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        int index = list.Count;
        while (index > 1)
        {
            index--;
            int newIndex = rng.Next(index + 1);
            (list[index], list[newIndex]) = (list[newIndex], list[index]);
        }
    }
}
