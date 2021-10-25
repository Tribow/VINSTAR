using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class red_fighter : Base_Enemy_Script
{

    

    private void FixedUpdate()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("player");
        }
        int amount_nearby = Physics2D.OverlapCircleNonAlloc(new Vector2(transform.position.x, transform.position.y), mineral_radius, nearby_minerals, 1 << 8);
        //DrawThis.Polygon(gameObject, 50, 60f, new Vector3(transform.position.x, transform.position.y, -5f), .2f, .2f);

        switch (AI)
        {
            case State.Idle:
                if (transform.position.x > 160f || transform.position.x < -160f || transform.position.y > 137.5f || transform.position.y < -137.5f)
                { //Switch to the Edge AI if it gets too close to the edge of the arena
                    StopAllCoroutines();
                    StartCoroutine(Edge_Movement(30f, new FloatRange(2f, 4f)));
                    AI = State.Edge;
                }
                if (amount_nearby > 0)
                {//Switch to the Mine AI the moment there's a mineral nearby and gun for it
                    StopAllCoroutines();
                    StartCoroutine(Mineral_Movement(nearby_minerals, new Stopwatch(2), 20f, 3f, .16f, 8f + p_speed));
                    AI = State.Mine;
                }
                if (player != null)
                { //Switch to Attack AI once the player is close enough
                    if (Vector2.Distance(transform.position, player.transform.position) <= 40f)
                    {
                        StopAllCoroutines();
                        StartCoroutine(Attack_Movement(20f + p_speed, 10f, 30f, 20f, new Stopwatch(.167f), new FloatRange(1.67f, 3.33f), new FloatRange(.67f, .68f), .5f, 1f, 2f));
                        AI = State.Attack;
                    }
                }
                break;

            case State.Edge:
                if (boundary.bounds.Contains(new Vector2(Mathf.Abs(transform.position.x), Mathf.Abs(transform.position.y))))
                { //Switch to the Idle if you made it back
                    StopAllCoroutines();
                    StartCoroutine(Base_Idle(new FloatRange(.1667f, .25f), new FloatRange(4f, 6f), 2));
                    AI = State.Idle;
                }
                break;

            case State.Mine:
                if (amount_nearby == 0)
                { //Switch to Idle if there's no more minerals around
                    StopAllCoroutines();
                    StartCoroutine(Base_Idle(new FloatRange(.1667f, .25f), new FloatRange(4f, 6f), 2));
                    maxspeed = ogspeed;
                    AI = State.Idle;
                }
                if (player != null)
                { //Attack player even if mining
                    if (Vector2.Distance(transform.position, player.transform.position) <= 40f)
                    {
                        StopAllCoroutines();
                        StartCoroutine(Attack_Movement(20f + p_speed, 10f, 30f, 20f, new Stopwatch(.167f), new FloatRange(1.67f, 3.33f), new FloatRange(.67f, .68f), .5f, 1f, 2f));
                        AI = State.Attack;
                    }
                }
                break;

            case State.Attack:
                if (player != null)
                {
                    if (Vector2.Distance(transform.position, player.transform.position) > 40f)
                    {
                        StopAllCoroutines();
                        StartCoroutine(Base_Idle(new FloatRange(.1667f, .25f), new FloatRange(4f, 6f), 2));
                        mybullets.Clear(); //Clear the list since you're not attacking anymore
                        maxspeed = ogspeed;
                        decelerating = false; //Just in case
                        AI = State.Idle;
                    }
                }
                break;
        }
        velocity = Speed_Management(transform, maxspeed);
        Transform_Management(transform, turning_speed, velocity, 1.6f);
        Upgrade_Enemy(upgrade_points, 15, upgrade_object);
    }
}
