using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the downgrade to the splitter worker
//Its movement is super erratic and lacks the ability to evade. It is however, amazing at getting minerals and will do its damndest to do that
//Has 4 minerals by default, but needs 6 to make it back to normal

public class bluesplitter_scatterer : Base_Enemy_Script
{
    //Need to redo start event because the different idle values also upgrade points start at 4
    private new void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("manager");
        mango = manager.GetComponent<manager_script>();
        audiomanager = GameObject.FindGameObjectWithTag("audio manager").GetComponent<audio_manager>();
        boundary = GameObject.FindGameObjectWithTag("boundary").GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("player");
        speed = set_speed.Random;
        ogspeed = speed;
        maxspeed = ogspeed;
        velocity_angle = transform.eulerAngles.z;
        StartCoroutine(Base_Idle(new FloatRange(.25f, .4f), new FloatRange(7f, 9f), .75f)); //Starts the original coroutine
        AI = State.Idle;
        _material = gameObject.GetComponent<SpriteRenderer>().material;
        upgrade_points = 4;
        my_canvas = Instantiate(enemy_canvas);
    }

    private void FixedUpdate()
    {
        if (player == null)
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
                    StartCoroutine(Edge_Movement(20f, new FloatRange(8f, 10f)));
                    AI = State.Edge;
                }
                if (amount_nearby > 0)
                {//Switch to the Mine AI the moment there's a mineral nearby and gun for it
                    StopAllCoroutines();
                    StartCoroutine(Mineral_Movement(nearby_minerals, new Stopwatch(1), 15f, 5f, .5f, 18f + p_speed));
                    AI = State.Mine;
                }
                break;

            case State.Edge:
                if (boundary.bounds.Contains(new Vector2(Mathf.Abs(transform.position.x), Mathf.Abs(transform.position.y))))
                { //Switch to the Idle if you made it back
                    StopAllCoroutines();
                    StartCoroutine(Base_Idle(new FloatRange(.25f, .4f), new FloatRange(7f, 9f), .75f));
                    AI = State.Idle;
                }
                break;

            case State.Mine:
                if (amount_nearby == 0)
                { //Switch to Idle if there's no more minerals around
                    StopAllCoroutines();
                    StartCoroutine(Base_Idle(new FloatRange(.25f, .4f), new FloatRange(7f, 9f), .75f));
                    maxspeed = ogspeed;
                    turning_speed = 0;
                    AI = State.Idle;
                }
                break;
        }

        velocity = Speed_Management(transform, maxspeed);
        Transform_Management(transform, turning_speed, velocity, 1.6f);
        Upgrade_Enemy(upgrade_points, 6, upgrade_object);
    }
}
