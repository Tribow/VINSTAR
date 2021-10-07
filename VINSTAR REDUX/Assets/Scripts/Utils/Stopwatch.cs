using System;
using UnityEngine;

//Helper class existing for countdowns.
[Serializable]
public class Stopwatch
{
    public float initial_time;
    public float current_time;
    public bool finished;


    public Stopwatch(float the_number)
    {
        initial_time = the_number;
        current_time = the_number;
        finished = false;
    }

    public void Countdown()
    {
        if (current_time > 0)
        {
            current_time -= Time.deltaTime;
        }
        else
        {
            finished = true;
        }
    }

    public void Reset()
    {
        finished = false;
        current_time = initial_time;
    }

    public bool isFinished()
    {
        return finished;
    }
}
