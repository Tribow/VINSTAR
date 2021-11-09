using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This Enemy is way better at gathering minerals than most enemies
//Much faster than most enemies
//Leaves a trail of bullets behind and runs away if player is nearby
//Will prioritize mining over running away from player, but is very fast at going in and out

public class bluesplitter_fighter : Base_Enemy_Script
{
    [Header("Splitter Fighter Data")]
    public GameObject blue_worker;
    public GameObject splitter_part;
    public Sprite bullet_sprite;


    private List<GameObject> splitter_list = new List<GameObject>();
    private List<Material> material_part = new List<Material>();
    private Stopwatch splitter_fire_rate = new Stopwatch(.4f);

    //Need to redo start event because the different idle values
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
        splitter_fire_rate.initial_time = fire_rate;
        StartCoroutine(Base_Idle(new FloatRange(.45f, .8f), new FloatRange(1f, 7f), 8f)); //Starts the original coroutine
        AI = State.Idle;
        _material = gameObject.GetComponent<SpriteRenderer>().material;
        my_canvas = Instantiate(enemy_canvas);
        //By default, this must have 3 parts following it
        GameObject object1 = Instantiate(splitter_part, transform.position, Quaternion.identity);
        object1.GetComponent<splitter_part_script>().my_leader = gameObject;
        GameObject object2 = Instantiate(splitter_part, transform.position, Quaternion.identity);
        object2.GetComponent<splitter_part_script>().my_leader = gameObject;
        GameObject object3 = Instantiate(splitter_part, transform.position, Quaternion.identity);
        object3.GetComponent<splitter_part_script>().my_leader = gameObject;
        splitter_list.Add(object1);
        splitter_list.Add(object2);
        splitter_list.Add(object3);
        mango.Add_To_Enemy_List(object1);
        mango.Add_To_Enemy_List(object2);
        mango.Add_To_Enemy_List(object3);
        material_part.Add(object1.GetComponent<SpriteRenderer>().material);
        material_part.Add(object2.GetComponent<SpriteRenderer>().material);
        material_part.Add(object3.GetComponent<SpriteRenderer>().material);
    }

    //Need to redo OnTriggerEnter because the death handler is different here
    private new void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "boundary" &&
            collision.gameObject.tag != "mineral" &&
            collision.gameObject.tag != "whitemineral" &&
            collision.gameObject.tag != "powerup" &&
            collision.gameObject.tag != "bullet" &&
            collision.gameObject.tag != "ebullet" &&
            collision.gameObject.tag != "bossbullet" &&
            collision.gameObject.tag != "splitter") //All of these collision checks are for what the cant be bounced off of.
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
            health = Take_Damage(health, 10f);
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
            if(upgrade_points / 6f % 1 == 0)
            {
                GameObject the_object = Instantiate(splitter_part, transform.position, transform.rotation);
                the_object.GetComponent<splitter_part_script>().my_leader = gameObject;
                splitter_list.Add(the_object);
                mango.Add_To_Enemy_List(the_object);
                material_part.Add(the_object.GetComponent<SpriteRenderer>().material);
                health += 10;
            }
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "whitemineral")
        {
            Im_White();
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "powerup")
        {
            Which_Powerup(collision.GetComponent<p_script>().powerup);
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator Splitter_Flash_Off(Material the_material)
    {
        yield return new WaitForSeconds(.05f);
        the_material.SetFloat("_FlashAlpha", 0f);
    }

    public new float Take_Damage(float current_health, float bullet_strength)
    {
        if (my_canvas != null)
        {
            GameObject temp_text = Instantiate(damage_text); //Make the text object (this is to display the damage being dealt)
            temp_text.GetComponent<Text>().text = "" + (int)(bullet_strength * 10); //Display the damage amount
            temp_text.transform.SetParent(my_canvas.transform, false); //Make sure it is a child of the canvas
            _material.SetFloat("_FlashAlpha", .5f);
            Invoke("Flash_Off", .05f);
            for (int i = 0; i < material_part.Count; i++)
            {
                material_part[i].SetFloat("_FlashAlpha", .5f);
                StartCoroutine(Splitter_Flash_Off(material_part[i]));
            }
        }
        return current_health - bullet_strength; //lower health;
    }

    public void Death_Splitter_Handler(bool do_give_points)
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
            audiomanager.Play_Sound(audio_manager.Sound.explosion_01, transform.position);

            for (int i = 0; i < splitter_list.Count; i++)
            {
                //For every splitter worker in the splitter list, spawn a worker
                Quaternion new_rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360f));
                GameObject new_enemy = Instantiate(blue_worker, gameObject.transform.position, new_rotation);
                new_enemy.GetComponent<bluesplitter_worker>().upgrade_points = 12;
                manager.GetComponent<manager_script>().Add_To_Enemy_List(new_enemy);
                Destroy(splitter_list[i]);
            }

            if (am_i_white)
            {
                int tempRan = Random.Range(0, 4);
                if (tempRan == 0)
                {
                    //1/4th of a chance to spawn a white mineral if it got one
                    Instantiate(white_mineral, gameObject.transform.position, gameObject.transform.rotation);
                }
            }

            //Also drop any powerups owned
            powerup.Drop_Powerups(transform.position, 1);

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

        
        
        for(int i = 1; i < splitter_list.Count + 1; i++)
        {
            float angle = Get_Angle(transform.position, splitter_list[i-1].transform.position);
            splitter_list[i-1].transform.rotation = Get_Desired_Rotation(angle);
            splitter_list[i - 1].transform.localScale = transform.localScale;

            float distance = Vector3.Distance(transform.position, splitter_list[i-1].transform.position);

            if (distance > .5f * i)
            {
                float move_distance = distance - .5f * i;
                float x = Mathf.Cos(angle * Mathf.Deg2Rad) * move_distance;
                float y = Mathf.Sin(angle * Mathf.Deg2Rad) * move_distance;

                Vector3 new_position = new Vector3(x, y, 0f) + splitter_list[i-1].transform.position;
                splitter_list[i-1].transform.position = new_position;
            }
        }

        if (player != null)
        {
            if (Vector2.Distance(transform.position, player.transform.position) < 30f && AI != State.Mine)
            {
                splitter_fire_rate.initial_time = fire_rate;
                splitter_fire_rate.Countdown();
                if (splitter_fire_rate.isFinished())
                {
                    GameObject bullet = Instantiate(my_bullet, splitter_list[splitter_list.Count - 1].transform.position, Quaternion.identity);
                    enemy_bullet_script bullet_script = bullet.GetComponent<enemy_bullet_script>();
                    bullet_script.speed = 0 + p_bulletspeed;
                    bullet_script.destroy_timer = 10 + p_bulletlife;
                    p_bulletsize = Mathf.Clamp(p_bulletsize, 0f, 10f); //Cannot spawn bullet at a bigger scale than 10
                    bullet.transform.localScale = new Vector3(bullet.transform.localScale.x + p_bulletsize, bullet.transform.localScale.y + p_bulletsize, bullet.transform.localScale.z + p_bulletsize);
                    bullet.GetComponent<SpriteRenderer>().sprite = bullet_sprite;
                    mybullets.Add(bullet);
                    splitter_fire_rate.Reset();
                    audiomanager.Play_Sound(audio_manager.Sound.shoot_01, transform.position, 1.8f);
                }
            }
        }

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
                    StartCoroutine(Mineral_Movement(nearby_minerals, new Stopwatch(2), 10f, 5f, .8f, 20f + p_speed));
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
                    StartCoroutine(Base_Idle(new FloatRange(.45f, .8f), new FloatRange(1f, 7f), 8f));
                    AI = State.Idle;
                }
                break;

            case State.Mine:
                if (amount_nearby == 0)
                { //Switch to Idle if there's no more minerals around
                    StopAllCoroutines();
                    StartCoroutine(Base_Idle(new FloatRange(.45f, .8f), new FloatRange(1f, 7f), 8f));
                    maxspeed = ogspeed;
                    turning_speed = 0;
                    AI = State.Idle;
                }
                break;

            case State.Evade:
                //Currently, the splitter worker will evade off screen if need be, idk if I should allow it to get trapped if it's getting chased or not
                if (player != null)
                {
                    if (Vector2.Distance(transform.position, player.transform.position) > 30f)
                    { //Return to Idle if it manages to get 45 units away while evading
                        StopAllCoroutines();
                        StartCoroutine(Base_Idle(new FloatRange(.45f, .8f), new FloatRange(1f, 7f), 8f));
                        AI = State.Idle;
                    }
                }

                if (amount_nearby > 0)
                {//Will immediately get distracted if there are minerals nearby
                    StopAllCoroutines();
                    StartCoroutine(Mineral_Movement(nearby_minerals, new Stopwatch(2), 10f, 5f, .8f, 20f + p_speed));
                    AI = State.Mine;
                }
                break;
        }

        velocity = Speed_Management(transform, maxspeed);
        Transform_Management(transform, turning_speed, velocity, 1.6f);
    }
}
