using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class UtilityRandom
{
    // Static random instance for consistent randomness across calls
    private static readonly System.Random random = new System.Random();

    // Shuffles the given list in place and returns it
    public static List<int> ShuffleList(List<int> listToRandomize)
    {
        for (int i = listToRandomize.Count - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);

            // Swap elements at i and j
            int temp = listToRandomize[i];
            listToRandomize[i] = listToRandomize[j];
            listToRandomize[j] = temp;
        }
        return listToRandomize;
    }

    // Prints the list in the Unity console
    public static void PrintTable(List<int> table)
    {
        string result = "Table: ";
        foreach (int item in table)
        {
            result += item + " ";
        }
        UnityEngine.Debug.Log(result);
    }
}
