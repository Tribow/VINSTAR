using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class red_worker : Base_Enemy_Script
{

    private void FixedUpdate()
    {
        int amount_nearby = Physics2D.OverlapCircleNonAlloc(new Vector2(transform.position.x, transform.position.y),mineral_radius, nearby_minerals, 1 << 8);
        //DrawThis.Polygon(gameObject, 50, 60f, new Vector3(transform.position.x, transform.position.y, -5f), .2f, .2f);
        
        

        switch(AI)
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
                    StartCoroutine(Mineral_Movement(nearby_minerals, new Stopwatch(2), 20f, 3f, .16f, 16f + p_speed));
                    AI = State.Mine;
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
                    turning_speed = 0;
                    AI = State.Idle;
                }
                break;
        }

        velocity = Speed_Management(transform, maxspeed);
        Transform_Management(transform, turning_speed, velocity , 1.6f);
        Upgrade_Enemy(upgrade_points, 8, upgrade_object);
        //Shader_Management();
    }

}
