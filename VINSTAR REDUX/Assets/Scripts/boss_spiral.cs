using UnityEngine;
using UnityEngine.UI;

public class boss_spiral : Base_Enemy_Script
{
    private Stopwatch boss_fire_rate = new Stopwatch(.16f);
    private Stopwatch berserk_wait;
    private Stopwatch berserk_time;
    private Vector2 boss_size;
    private float oghealth;
    private bool got_hit = false;
    private bool berserk_phase;

    private new void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("manager");
        mango = manager.GetComponent<manager_script>();
        audiomanager = GameObject.FindGameObjectWithTag("audio manager").GetComponent<audio_manager>();
        boundary = GameObject.FindGameObjectWithTag("boundary").GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("player");
        boss_size = new Vector2(GetComponent<Collider2D>().bounds.extents.x, GetComponent<Collider2D>().bounds.extents.y);
        speed = set_speed.Random;
        ogspeed = speed;
        maxspeed = ogspeed;
        boss_fire_rate.initial_time = fire_rate;
        velocity_angle = Choice.Choose(45f, 135f, 225f, 315f);
        AI = State.Idle;
        _material = gameObject.GetComponent<SpriteRenderer>().material;
        my_canvas = Instantiate(enemy_canvas);
        health = health * mango.player_bullet_damage;
        oghealth = health;
        //Randomizing how the boss acts a bit when at half health
        berserk_wait = new Stopwatch(Random.Range(10f, 15f));
        berserk_time = new Stopwatch(Random.Range(2f, 4f));
    }

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "boundary" &&
            collision.gameObject.tag != "mineral" &&
            collision.gameObject.tag != "whitemineral" &&
            collision.gameObject.tag != "powerup" &&
            collision.gameObject.tag != "bullet" &&
            collision.gameObject.tag != "ebullet" &&
            collision.gameObject.tag != "bossbullet") //All of these collision checks are for what the cant be bounced off of.
        {
            velocity_angle += 90;
        }

        if (collision.gameObject.tag == "bullet") //Got hit by player's bullet? Take damage.
        {
            health = Take_Damage(health, collision.GetComponent<player_bullet_script>().damage);
            got_hit = true;
            Death_Handler(true);
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
                health = Take_Damage(health, 3f);
                Destroy(collision.gameObject);
                Death_Handler(false);
            }
        }

        if (collision.gameObject.tag == "mineral")
        { //Raise the upgrade points when coming into contact with mineral
            upgrade_points++;
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

    public new float Take_Damage(float current_health, float bullet_strength)
    {
        if (my_canvas != null)
        {
            GameObject temp_text = Instantiate(damage_text); //Make the text object (this is to display the damage being dealt)
            temp_text.GetComponent<Text>().text = "" + (int)(bullet_strength * 10); //Display the damage amount
            temp_text.GetComponent<Text>().fontSize = 100;
            temp_text.transform.SetParent(my_canvas.transform, false); //Make sure it is a child of the canvas
            _material.SetFloat("_FlashAlpha", .5f);
            Invoke("Flash_Off", .05f);
        }
        return current_health - bullet_strength; //lower health;
    }

    public override void Attack_Method()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject new_bullet = Instantiate(my_bullet, gameObject.transform.position, Quaternion.Euler(0f, 0f, gameObject.transform.rotation.eulerAngles.z - 50 + (90 * i)));
            enemy_bullet_script bullet_script = new_bullet.GetComponent<enemy_bullet_script>();
            bullet_script.speed = 5;
            bullet_script.destroy_timer = 600;
            mybullets.Add(new_bullet);
        }
        audiomanager.Play_Sound(audio_manager.Sound.shoot_01, transform.position, .5f);
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            //If the boss hasn't been hit yet and the player isn't close by, move at fast speed
            if (Vector2.Distance(transform.position, player.transform.position) > 100f && !got_hit)
            {
                maxspeed = 20 + p_speed;
                speed = 20 + p_speed;
            }
            else
            {
                //Boss has been hit, so keep track of berserk phase
                if (berserk_phase)
                {
                    maxspeed = 20 + p_speed;
                    speed = 20 + p_speed;
                }
                else
                {
                    maxspeed = ogspeed;
                }
            }

            //Start shooting when close enough to player, don't shoot during berserk movement
            if (Vector2.Distance(transform.position, player.transform.position) < 60f && !berserk_wait.isFinished())
            {
                boss_fire_rate.initial_time = fire_rate;
                if (boss_fire_rate.isFinished())
                {
                    Attack_Method();
                    boss_fire_rate.Reset();
                }
                else
                {
                    boss_fire_rate.Countdown();
                }
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("player");
        }

        //At half health the berserk phase becomes active
        if (health < oghealth / 2)
        {
            if (berserk_wait.finished)
            {
                berserk_time.Countdown();
                berserk_phase = true;
                if (berserk_time.finished)
                {
                    berserk_time.Reset();
                    berserk_wait.Reset();
                    berserk_phase = false;
                }
            }
            else
                berserk_wait.Countdown();
        }

        //Boss bounces off of the sides of the level itself as well to stay in bounds
        if (transform.position.x > mango.level_bounds.x - boss_size.x)
        {
            transform.position = new Vector3(mango.level_bounds.x - boss_size.x, transform.position.y, transform.position.z);
            velocity_angle += 90f;
        }

        if (transform.position.x < -mango.level_bounds.x + boss_size.x)
        {
            transform.position = new Vector3(-mango.level_bounds.x + boss_size.x, transform.position.y, transform.position.z);
            velocity_angle += 90f;
        }

        if (transform.position.y > mango.level_bounds.y - boss_size.y)
        {
            transform.position = new Vector3(transform.position.x, mango.level_bounds.y - boss_size.y, transform.position.z);
            velocity_angle += 90f;
        }

        if (transform.position.y < -mango.level_bounds.y + boss_size.y)
        {
            transform.position = new Vector3(transform.position.x, -mango.level_bounds.y + boss_size.y, transform.position.z);
            velocity_angle += 90f;
        }

        //velocity_angle = velocity.z;
        velocity = new Vector3(Mathf.Cos(velocity_angle * Mathf.PI / 180) * speed, Mathf.Sin(velocity_angle * Mathf.PI / 180) * speed);
        //Make sure the enemy never goes too fast
        velocity = new Vector3(Mathf.Clamp(velocity.x, -maxspeed * Mathf.Abs(velocity.normalized.x), maxspeed * Mathf.Abs(velocity.normalized.x)), Mathf.Clamp(velocity.y, -maxspeed * Mathf.Abs(velocity.normalized.y), maxspeed * Mathf.Abs(velocity.normalized.y)));

        transform.position += velocity * Time.deltaTime;
        transform.Rotate(0f, 0f, 1.7f);
        if (my_canvas != null)
        {
            my_canvas.transform.position = new Vector2(transform.position.x, transform.position.y + 5f); //the enemy_canvas position relative to the worker
        }

        //print(health);
    }
}
