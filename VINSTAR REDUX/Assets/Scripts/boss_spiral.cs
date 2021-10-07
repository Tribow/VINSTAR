using UnityEngine;
using UnityEngine.UI;

public class boss_spiral : Base_Enemy_Script
{
    private Stopwatch fire_rate = new Stopwatch(.16f);
    private Stopwatch berserk_wait;
    private Stopwatch berserk_time;
    private float oghealth;
    private bool got_hit = false;
    private bool berserk_phase;

    private new void Start()
    {
        manager = GameObject.FindGameObjectWithTag("manager");
        audiomanager = GameObject.FindGameObjectWithTag("audio manager").GetComponent<audio_manager>();
        boundary = GameObject.FindGameObjectWithTag("boundary").GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("player");
        speed = set_speed.Random;
        ogspeed = speed;
        maxspeed = ogspeed;
        velocity_angle = Choice.Choose(45f, 135f, 225f, 315f);
        AI = State.Idle;
        _material = gameObject.GetComponent<SpriteRenderer>().material;
        my_canvas = Instantiate(enemy_canvas);
        health = health * manager.GetComponent<manager_script>().player_bullet_damage;
        oghealth = health;
        berserk_wait = new Stopwatch(Random.Range(10f, 15f));
        berserk_time = new Stopwatch(Random.Range(2f, 4f));
    }

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "boundary" &&
            collision.gameObject.tag != "mineral" &&
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
            if (Vector2.Distance(transform.position, player.transform.position) > 100f && !got_hit)
            {
                maxspeed = 20;
                speed = 20;
            }
            else
            {
                if (berserk_phase)
                {
                    maxspeed = 20;
                    speed = 20;
                }
                else
                {
                    maxspeed = ogspeed;
                }
            }

            if (Vector2.Distance(transform.position, player.transform.position) < 60f && !berserk_wait.isFinished())
            {
                if (fire_rate.isFinished())
                {
                    Attack_Method();
                    fire_rate.Reset();
                }
                else
                {
                    fire_rate.Countdown();
                }
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("player");
        }

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

        if (transform.position.x > 168f)
        {
            transform.position = new Vector3(168f, transform.position.y, transform.position.z);
            velocity_angle += 90f;
        }

        if (transform.position.x < -168f)
        {
            transform.position = new Vector3(-168f, transform.position.y, transform.position.z);
            velocity_angle += 90f;
        }

        if (transform.position.y > 140.7f)
        {
            transform.position = new Vector3(transform.position.x, 140.7f, transform.position.z);
            velocity_angle += 90f;
        }

        if (transform.position.y < -140.7f)
        {
            transform.position = new Vector3(transform.position.x, -140.7f, transform.position.z);
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
