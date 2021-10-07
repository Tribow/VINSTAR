using System;
using UnityEngine;

//This is just a helper class so I don't have to write a minimum and maximum value for every random thing in the map generation
[Serializable]
public class FloatRange
{
    [Range(0, 800)]
    public float minNum; //The min value to use in the range
    [Range(.001f, 800)]
    public float maxNum; //The max value to use in the range

    public FloatRange(float min, float max) //Constructor, you know, to set values
    {
        minNum = min;
        maxNum = max;
    }

    public float Random //Get random value within the range
    {
        get
        {
            return UnityEngine.Random.Range(minNum, maxNum);
        }
    }
}