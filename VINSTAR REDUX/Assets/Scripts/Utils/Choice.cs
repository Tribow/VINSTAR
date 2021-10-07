using System;
//using UnityEngine;

//Helper class for choosing specific numbers based on what you put in parameters
[Serializable]
public static class Choice
{
    public static float Choose(params float[] values)
    {
        int random_number = UnityEngine.Random.Range(0, values.Length);
        return values[random_number];
    }

    public static int Choose(params int[] values)
    {
        int random_number = UnityEngine.Random.Range(0, values.Length);
        return values[random_number];
    }
}
