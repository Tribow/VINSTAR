using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This enemy is better at picking up minerals than the reds are, but they move slower
//It will also try to run away from the player
//Will prioritize gathering minerals over anything else
//Gets larger the more minerals it has. It will grow at a rate of .0667 based on how many minerals it has (should cap at 12)
//When dying, it should will spawn blue_scatterers for every 4 minerals it has. If it never collected that many it wont do this

public class bluesplitter_worker : Base_Enemy_Script
{
    [Header("Splitter Worker Data")]
    public GameObject blue_scatterer;

    //Need to redo start event because the different idle values
    private new void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("manager");
        audiomanager = GameObject.FindGameObjectWithTag("audio manager").GetComponent<audio_manager>();
        boundary = GameObject.FindGameObjectWithTag("boundary").GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("player");
        speed = set_speed.Random;
        ogspeed = speed;
        maxspeed = ogspeed;
        velocity_angle = transform.eulerAngles.z;
        StartCoroutine(Base_Idle(new FloatRange(.1667f, .25f), new FloatRange(4f, 9f), 3f)); //Starts the original coroutine
        AI = State.Idle;
        _material = gameObject.GetComponent<SpriteRenderer>().material;
        my_canvas = Instantiate(enemy_canvas);
    }

    //Need to redo OnTriggerEnter because the death handler is different here
    private new void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "boundary" &&
            collision.gameObject.tag != "mineral" &&
            collision.gameObject.tag != "whitemineral" &&
            collision.gameObject.tag != "bullet" &&
            collision.gameObject.tag != "ebullet" &&
            collision.gameObject.tag != "bossbullet") //All of these collision checks are for what the cant be bounced off of.
        {
            speed *= -2;
        }

        if (collision.gameObject.tag == "bullet") //Got hit by player's bullet? Take damage.
        {
            health = Take_Damage(health, collision.GetComponent<player_bullet_script>().damage);
            Death_Splitter_Handler(true);
        }

        if (collision.gameObject.tag == "bossbullet")
        {
            Take_Damage(health, 10f);
            Death_Splitter_Handler(false);
        }

        if (collision.gameObject.tag == "ebullet")
        {
            bool do_I_take_damage = true;
            for (int i = 0; i < mybullets.Count; i++)
            { //Loop through the mybullets list and make sure the enemy is not hitting itself with its own bullet
                if (collision.gameObject == mybullets[i])
                {
                    do_I_take_damage = false;
                    break;
                }
            }
            if (do_I_take_damage)
            { //If it's not the enemy's own bullet, take damage
                health = Take_Damage(health, 1f);
                Destroy(collision.gameObject);
                Death_Splitter_Handler(false);
            }
        }

        if (collision.gameObject.tag == "mineral")
        {
            upgrade_points++;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "whitemineral")
        {
            Im_White();
            Destroy(collision.gameObject);
        }
    }

    private void Death_Splitter_Handler(bool do_give_points)
    {
        //I AM COPYING THE DEATH HANDLER METHOD HERE SINCE THIS ENEMY HANDLES THINGS DIFFERENTLY
        if (health <= 0) //if health low enough, the time to kill it, dont do anything otherwise
        {
            if (do_give_points)
            {
                if (mango == null) //Be doubly sure to access the manager script
                {
                    manager = GameObject.FindGameObjectWithTag("manager");
                    mango = manager.GetComponent<manager_script>();
                    mango.Add_Score(score);
                    mango.Enemy_Death(am_i_the_boss);
                }
                else
                {
                    mango.Add_Score(score);
                    mango.Enemy_Death(am_i_the_boss);
                }
            }

            //Particle
            Instantiate(death_particles, gameObject.transform.position, gameObject.transform.rotation);

            //Sound
            if (audiomanager != null)
            {
                audiomanager.Play_Sound(audio_manager.Sound.explosion_01, transform.position);
            }

            //Do I spawn scatterer?
            int spawncheck = 0;
            for (int i = 0; i < upgrade_points; i++)
            {
                spawncheck++;
                if (spawncheck == 4)
                { //If spawncheck counts to 4, that means you can spawn a scatterer
                    Quaternion new_rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360f));
                    GameObject new_enemy = Instantiate(blue_scatterer, gameObject.transform.position, new_rotation);
                    manager.GetComponent<manager_script>().Add_To_Enemy_List(new_enemy);
                    spawncheck = 0;
                }
            }

            //Be sure to destroy extra objects
            Destroy(my_canvas);
            Destroy(gameObject);
        }
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
                    StartCoroutine(Edge_Movement(20f, new FloatRange(5f, 8f)));
                    AI = State.Edge;
                }
                if (amount_nearby > 0)
                {//Switch to the Mine AI the moment there's a mineral nearby and gun for it
                    StopAllCoroutines();
                    StartCoroutine(Mineral_Movement(nearby_minerals, new Stopwatch(2), 10f, 5f, .3f, 9f));
                    AI = State.Mine;
                }
                if (player != null)
                {
                    if (Vector2.Distance(transform.position, player.transform.position) <= 15f)
                    { //Switch to Evade AI if the player gets too close
                        StopAllCoroutines();
                        StartCoroutine(Evade_Movement(player, 150f, new FloatRange(4f, 7f)));
                        AI = State.Evade;
                    }
                }
                break;

            case State.Edge:
                if (boundary.bounds.Contains(new Vector2(Mathf.Abs(transform.position.x), Mathf.Abs(transform.position.y))))
                { //Switch to the Idle if you made it back
                    StopAllCoroutines();
                    StartCoroutine(Base_Idle(new FloatRange(.1667f, .25f), new FloatRange(4f, 9f), 3f));
                    AI = State.Idle;
                }
                break;

            case State.Mine:
                if (amount_nearby == 0)
                { //Switch to Idle if there's no more minerals around
                    StopAllCoroutines();
                    StartCoroutine(Base_Idle(new FloatRange(.1667f, .25f), new FloatRange(4f, 9f), 3f));
                    maxspeed = ogspeed;
                    turning_speed = 0;
                    AI = State.Idle;
                }
                break;

            case State.Evade:
                //Currently, the splitter worker will evade off screen if need be, idk if I should allow it to get trapped if it's getting chased or not
                if (player != null)
                {
                    if (Vector2.Distance(transform.position, player.transform.position) > 45f)
                    { //Return to Idle if it manages to get 45 units away while evading
                        StopAllCoroutines();
                        StartCoroutine(Base_Idle(new FloatRange(.1667f, .25f), new FloatRange(4f, 9f), 3f));
                        AI = State.Idle;
                    }
                }

                if (amount_nearby > 0)
                {//Will immediately get distracted if there are minerals nearby
                    StopAllCoroutines();
                    StartCoroutine(Mineral_Movement(nearby_minerals, new Stopwatch(2), 10f, 5f, .3f, 9f));
                    AI = State.Mine;
                }
                break;
        }

        velocity = Speed_Management(transform, maxspeed);
        Transform_Management(transform, turning_speed, velocity, 1.6f);
        Upgrade_Enemy(upgrade_points, 15, upgrade_object);

        //This object will adjust its scale depending on how many minerals it has.
        float scale_modifier = 1f + (upgrade_points * .0667f);
        if (scale_modifier >= 1.8f)
            scale_modifier = 1.8f;
        gameObject.transform.localScale = new Vector3(1 * scale_modifier, 1 * scale_modifier, 1 * scale_modifier);
    }
}
