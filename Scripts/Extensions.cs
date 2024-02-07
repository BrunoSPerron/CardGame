using Newtonsoft.Json;
using System;
using System.Collections.Generic;


public static class Extensions
{

    ///<returns>The object serialized in a Json encoded string.</returns>
    public static string Serialize<T>(this T source)
    {
        return JsonConvert.SerializeObject(source);
    }

    /// <summary>
    /// Based on Fisher-Yates. https://stackoverflow.com/a/22668974
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        int index = list.Count;
        while (index > 1)
        {
            index--;
            int newIndex = RANDOM.rand.Next(index + 1);
            (list[index], list[newIndex]) = (list[newIndex], list[index]);
        }
    }
}
