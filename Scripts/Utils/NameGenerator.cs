using Godot;
using System;
using System.Collections.Generic;

public static class NameGenerator
{
    //private static bool isInitialized = Initialize();

    private const int averageNameLength = 6;
    private const int nameLengthVariance = 4;

#pragma warning disable IDE0044 // "Add readonly modifier"
    private static Dictionary<char, Dictionary<char, int>> maleCharWeights;
    private static Dictionary<char, Dictionary<char, int>> MaleCharWeights
    {
        get
        {
            if (maleCharWeights == null)
                Initialize();
            return maleCharWeights;
        }
    }
    private static Dictionary<char, Dictionary<char, int>> femaleCharWeights;
    private static Dictionary<char, Dictionary<char, int>> FemaleCharWeights
    {
        get
        {
            if (femaleCharWeights == null)
                Initialize();
            return femaleCharWeights;
        }
    }
#pragma warning restore IDE0044 // "Add readonly modifier"
    private static readonly Random rand = new Random();

    private static bool Initialize()
    {
        maleCharWeights = new Dictionary<char, Dictionary<char, int>>();
        femaleCharWeights = new Dictionary<char, Dictionary<char, int>>();

        for (int i = 64; i < 91; i++)
        {
            maleCharWeights.Add((char)i, new Dictionary<char, int>());
            femaleCharWeights.Add((char)i, new Dictionary<char, int>());
        }

        string[] lines = System.IO.File.ReadAllLines(
            AppDomain.CurrentDomain.BaseDirectory + "\\nameweights");

        for (int i = 2; i < lines.Length; i++)
        {
            string s = lines[i];
            string[] data = s.Split('&');

            if (data[0] == "M")
                maleCharWeights[data[1][0]].Add(data[2][0], int.Parse(data[3]));
            else
                femaleCharWeights[data[1][0]].Add(data[2][0], int.Parse(data[3]));
        }
        return true;
    }

    // ===== Accessors =====

    public static string GetRandomName()
    {
        return GenerateName(rand.Next(0, 2) % 1 == 1);
    }

    public static string GetRandomMaleName()
    {
        return GenerateName(true);

    }

    public static string GetRandomFemaleName()
    {
        return GenerateName(false);

    }

    // ===== Private methods =====

    private static string GenerateName(bool forMale)
    {
        string newName = "";
        int nameLength = averageNameLength;

        for (var i = 0; i < nameLengthVariance; i++)
            nameLength += rand.Next(-1, 2);

        char precedentChar = ' ';
        char nextChar = '@';
        char currentChar;

        while (newName.Length < nameLength)
        {
            currentChar = nextChar;
            do
            {
                nextChar = GetRandomCharAfter(nextChar, forMale);
            }
            while (currentChar == nextChar && precedentChar == currentChar);

            newName += nextChar;
            precedentChar = currentChar;
        }

        return newName;
    }

    private static char GetRandomCharAfter(char currentChar, bool forMaleName)
    {
        char c;
        c = '-';

        int TotalWeight = 0;
        foreach (KeyValuePair<char, int> kvp in forMaleName
            ? MaleCharWeights[currentChar]
            : FemaleCharWeights[currentChar])
            TotalWeight += kvp.Value;
        int rollValue = rand.Next(0, TotalWeight);

        foreach (KeyValuePair<char, int> kvp in forMaleName
            ? MaleCharWeights[currentChar]
            : FemaleCharWeights[currentChar])
        {
            rollValue -= kvp.Value;
            if (rollValue < 0 && c == '-')
                c = kvp.Key;
        }

        return c;
    }
}
