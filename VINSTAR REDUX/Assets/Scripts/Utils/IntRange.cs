using System;
using UnityEngine;

//This is just a helper class so I don't have to write a minimum and maximum value for every random thing in the map generation
[Serializable]
public class IntRange
{
    [Range(1, 800)]
    public int minNum; //The min value to use in the range
    [Range(2, 800)]
    public int maxNum; //The max value to use in the range

    public IntRange(int min, int max) //Constructor, you know, to set values
    {
        minNum = min;
        maxNum = max;
    }

    public int Random //Get random value within the range
    {
        get
        {
            return UnityEngine.Random.Range(minNum, maxNum);
        }
    }
}