using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class PathHelper
{
    public static Tuple<string, string> GetNameAndMod(string name, BaseModel model)
    {
        string mod = model.Mod;
        string[] splittedName = name.Split(
            new string[] { "__" }, StringSplitOptions.None);
        string cardName = splittedName[0];
        if (splittedName.Length > 1)
        {
            mod = splittedName[0];
            cardName = splittedName[1];
        }
        return new Tuple<string, string>(cardName, mod);
    }
}

