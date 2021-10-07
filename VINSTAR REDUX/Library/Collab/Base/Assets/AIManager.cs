using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static Dictionary<string, Behavior> Behaviors = new Dictionary<string, Behavior>();
    static Ship_Script[] Ships = new Ship_Script[0];
    static Behavior Base_Movement_Behavior = new Behavior(Base_Movement);
    // Start is called before the first frame update
    void Start()
    {
        new Behavior(1, Mine_Movement, "mineral");
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Ship_Script ship in Ships)
        {
            List<string> possibleBehaviors = new List<string>();
            Behavior behavior = Base_Movement_Behavior;
            foreach (string behaviorString in ship.PossibleBehaviors)
            {
                if (Behaviors[behaviorString].Priority >= behavior.Priority)
                {
                    behavior = Behaviors[behaviorString];
                }
            }
            ship.PossibleBehaviors.Clear();
            if (behavior != ship.CurrentBehavior)
            {
                ship.StopCoroutine(ship.CurrentBehavior.Coroutine(ship));
                ship.SetCurrentBehavior(behavior);
            }
        }
    }

    public static void AddShip(Ship_Script Ship)
    {
        Ship.CurrentBehavior = Base_Movement_Behavior;
        Ship.StartCoroutine(Ship.CurrentBehavior.Coroutine(Ship));
        //Ship.SetCurrentBehavior(Base_Movement_Behavior);
        foreach(string behaviorString in Ship.behaviors)
        {
            print("Adding " + behaviorString);
            Ship.StartCoroutine(Trigger(Ship, behaviorString));
        }
        List<Ship_Script> shipLst = new List<Ship_Script>(Ships);
        shipLst.Add(Ship);
        Ships = shipLst.ToArray();
    }

    static IEnumerator Base_Movement(Ship_Script Ship)
    {
        for (; ; )
        {
            Vector2 Choices = new Vector2(GamingUtils.InclusiveRandom(-1.0f, 1.0f), GamingUtils.InclusiveRandom(-1.0f, 1.0f));
            Ship.Velocity += Choices * Ship.Speed * Time.deltaTime;
            print("Idle");
            yield return new WaitForFixedUpdate();
        }
    }

    static IEnumerator Trigger(Ship_Script Ship, string Tag)
    {
        for (;;)
        {
            if (ProximityCheck(Tag, Ship))
            {
                Ship.PossibleBehaviors.Add(Tag);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Mine_Movement(Ship_Script Ship)
    {
        for (; ; )
        {
            print("mining");
            yield return new WaitForFixedUpdate();
        }
    }

    static bool ProximityCheck(string Tag, Ship_Script Ship)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(Tag);
        foreach(GameObject target in targets)
        {
            if (Vector3.Distance(Ship.transform.position, target.transform.position) < Ship.ScanDistance)
            {
                return true;
            }
        }

        return false;
    }

    void GoToTarget(Ship_Script Ship, Transform target)
    {
        float maxspeed = Ship.Speed / 2; //top speed of base AI

        if (Ship.start_turning != 0)
        {//if turning
            //timer stuff
            Ship.start_turning--;
        }
        else if (Ship.start_turning == 0)
        {//if not turning then start_turning
            Ship.start_turning = 120;
            Ship.turn_time = Random.Range(10f, 15f); //How long it will turn in the base AI
            Ship.choice = Random.Range(0, 2);
        }

        if (Ship.turn_time > 0)
        {
            Ship.turn_time--;
            Ship.turning_speed = Ship.turning_variable[Ship.choice]; //Base turning speed
        }
        else
        {
            Ship.turning_speed = 0;
        }

        //Edge of screen for the enemy
        if (transform.position.x > 160f || transform.position.x < -160f || transform.position.y > 137.5f || transform.position.y < -137.5f)
        {
            Ship.target_position = new Vector3(0f, 0f, 0f);
            choice = Random.Range(2, 4);
            AI = "Edge AI";
        }

        if (near_mineral.Length > 0)
        {
            mineral_choose = true;
            AI = "Mine AI";
        }
    }
}

public delegate IEnumerator MyCoroutine(Ship_Script Ship);

public class Behavior
{
    public MyCoroutine Coroutine;
    public int Priority;

    public Behavior(int priority, MyCoroutine coroutine, string Name)
    {
        Coroutine = coroutine;
        Priority = priority;
        AIManager.Behaviors.Add(Name, this);
    }

    public Behavior(MyCoroutine coroutine)
    {
        Coroutine = coroutine;
    }
}