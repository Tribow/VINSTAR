using UnityEngine;
using UnityEngine.InputSystem;

public class player_ship_script : MonoBehaviour
{
    [Header("Required Fields")]
    public GameObject bullet;
    public int immune_timer;
    public AudioSource engine;
    public GameObject the_particles;

    [Header("Player Data")]
    [SerializeField]
    private float max_speed; //was originally 10
    [SerializeField]
    private float acceleration; //was originally .5
    [SerializeField]
    private float deceleration; //was originally .99
    [SerializeField]
    private float rotation_speed; //was originally 6
    [SerializeField]
    private Stopwatch bullet_timer = new Stopwatch(.1333f);

    [Header("Public Data - DO NOT EDIT")]
    public float speed;
    public Vector3 velocity;

    private manager_script manager;
    private audio_manager audiomanager;
    private Material _material;
    private Player_Actions.Player_ControlsActions player_input;
    private Stopwatch magnet_timer = new Stopwatch(1.5f);
    private Vector2 input_value;
    private Vector2 player_size = new Vector2(.5f, .5f);
    private Color outline_color;
    private int health = 1;
    private float outline_thickness;
    private float velocity_angle;
    private float max_og_speed;
    
    private void OnEnable()
    {
        Player_Actions player_actions = input_manager_script.input_actions;
        player_input = player_actions.Player_Controls;
        //Input
        player_input.Shoot.Enable();
        player_input.Acceleration.Enable();
        player_input.Deceleration.Enable();
        player_input.TurnLeft.Enable();
        player_input.TurnRight.Enable();
        player_input.Slow.Enable();
    }

    private void OnDisable()
    {
        player_input.Shoot.Disable();
        player_input.Acceleration.Disable();
        player_input.Deceleration.Disable();
        player_input.TurnLeft.Disable();
        player_input.TurnRight.Disable();
        player_input.Slow.Disable();
    }

    // Start is called before the first frame update
    void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("manager").GetComponent<manager_script>();
        audiomanager = GameObject.FindGameObjectWithTag("audio manager").GetComponent<audio_manager>();
        _material = GetComponent<SpriteRenderer>().material;
        immune_timer = 180;
        velocity_angle = transform.eulerAngles.z;
        max_og_speed = max_speed;
        outline_color = _material.GetColor("_OutlineColor");
        outline_thickness = _material.GetFloat("_OutlineThickness");
        _material.SetColor("_FlashColor", Color.red);

        //Sound settings
        engine.Play(); //Play sound immediately
        engine.loop = true; //Loop it
        engine.volume = 0; //Have it at zero so it makes no noise at first
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "boundary" && 
            collision.gameObject.tag != "mineral" &&
            collision.gameObject.tag != "whitemineral" &&
            collision.gameObject.tag != "bullet" && 
            collision.gameObject.tag != "ebullet" && 
            collision.gameObject.tag != "player")
        {
            if (collision.gameObject.tag == "enemy")
            { //This code would bounce the player based on where the enemy was moving but the player would end up moving through them sometimes so....

                Vector3 enemy_velocity = collision.gameObject.GetComponent<Base_Enemy_Script>().velocity;
                //print(Vector3.Angle(enemy_velocity, velocity));
                if (Vector3.Angle(enemy_velocity, velocity) > 90)
                {
                    velocity *= -1;
                    velocity += enemy_velocity;
                }
                else
                    velocity *= -1;
            }
            else
                velocity *= -1;
        }

        if (collision.gameObject.tag == "ebullet" || collision.gameObject.tag == "bossbullet")
        {
            if (immune_timer <= 0)
            {
                health--;
                _material.SetFloat("_FlashAlpha", 1f);
                Invoke("Flash_Off", .05f);
                if (health <= 0)
                {
                    //Particle
                    Instantiate(the_particles, transform.position, transform.rotation);

                    //Sound
                    audiomanager.Play_Sound(audio_manager.Sound.explosion_01, transform.position);

                    Destroy(gameObject); //It can only destroy player if the immune_timer is 0
                }
            }
        }

        if (collision.gameObject.tag == "mineral")
        {
            manager.Add_Damage();
            _material.SetColor("_OutlineColor", new Color(80f/255f, 120f/255f, 1f));
            _material.SetFloat("_OutlineThickness", 3f);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "whitemineral")
        {
            health += 1;
            outline_color = Color.white;
            _material.SetFloat("_OutlineThickness", health / 4f);
            Destroy(collision.gameObject);
        }
    }

    private void Flash_Off()
    {
        _material.SetFloat("_FlashAlpha", 0f);
    }

    private void Update()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        immune_timer--;
        Vector3 old_position = transform.position;

        input_value = new Vector2(player_input.TurnRight.ReadValue<float>() + (player_input.TurnLeft.ReadValue<float>() * -1f), player_input.Acceleration.ReadValue<float>() + (player_input.Deceleration.ReadValue<float>() * -1f));

        if (player_input.Shoot.ReadValue<float>() == 1f) //Start shooting when button is held
        {
            bullet_timer.Countdown();
            magnet_timer.Reset();
            if (bullet_timer.isFinished()) //only fire a shot when countdown is done
            {
                bullet_timer.Reset();
                Instantiate(bullet, transform.position, transform.rotation);
                audiomanager.Play_Sound(audio_manager.Sound.shoot_01, transform.position);
            }
        }
        else if (player_input.Shoot.ReadValue<float>() == 0f)
        {
            bullet_timer.current_time = 0;
            magnet_timer.Countdown();
            Collider2D[] nearby_minerals = new Collider2D[10];
            if (magnet_timer.isFinished()) //Won't activate magnet unless player has stopped shooting for 1.5 seconds
            {
                int amount_nearby = Physics2D.OverlapCircleNonAlloc(new Vector2(transform.position.x, transform.position.y), 1.5f, nearby_minerals, 1 << 8);
                if(nearby_minerals != null)
                {
                    for(int i = 0; i < nearby_minerals.Length; i++)
                    {
                        if(nearby_minerals[i] != null) //Pull close enough minerals into the player with a "magnet" if the player isn't shooting
                        {
                            mineral_script the_script = nearby_minerals[i].GetComponent<mineral_script>();
                            Quaternion look_rotation = Quaternion.LookRotation(Vector3.forward, transform.position - nearby_minerals[i].transform.position);
                            look_rotation = look_rotation * Quaternion.Euler(0, 0, 90);
                            the_script.velocity_angle = look_rotation.eulerAngles.z;
                            the_script.movement_speed_x = .08f;
                            the_script.movement_speed_y = .08f;
                        }
                    }
                }

            }
        }

        if (input_value.x > 0)
        {
            transform.Rotate(Vector3.forward * (-rotation_speed * input_value.x)); //rotation to the left
        }
        else if (input_value.x < 0)
        {
            transform.Rotate(Vector3.forward * (rotation_speed * -input_value.x)); //rotation to the right
        }

        if(player_input.Slow.ReadValue<float>() == 1f) //Lowers the max_speed to very slow when held
        {
            max_speed = Mathf.Lerp(max_speed, 2f, .1f);
        }
        else
        {
            max_speed = max_og_speed;
        }

        if (input_value.y != 0) //Control the velocity whenever the value isn't 0.
        {
            float acceleration_value = acceleration * input_value.y;
            velocity_angle = transform.eulerAngles.z; //update the angle of velocity while button is pressed
            velocity += new Vector3(Mathf.Cos(velocity_angle * Mathf.PI / 180) * acceleration_value, Mathf.Sin(velocity_angle * Mathf.PI / 180) * acceleration_value);
        }
        else //Decelerate when the input value is 0.
        {
                //The velocity angle is no longer being updated when not accelerating
                velocity = new Vector3(velocity.x * deceleration, velocity.y * deceleration);
        }
        //Make sure the player doesn't move too fast
        velocity = new Vector3(Mathf.Clamp(velocity.x, -max_speed * Mathf.Abs(velocity.normalized.x), max_speed * Mathf.Abs(velocity.normalized.x)), Mathf.Clamp(velocity.y, -max_speed * Mathf.Abs(velocity.normalized.y), max_speed * Mathf.Abs(velocity.normalized.y)));

        //print(velocity.x + ", " + velocity.y);

        transform.position += velocity * Time.deltaTime; //This is what actually makes the ship move
        speed = Vector3.Distance(old_position, transform.position) / Time.deltaTime;
        //print(speed);

        if (health > 1)
        {
            outline_thickness = health / 4;
            if (outline_thickness >= 3f) //Make sure it doesn't go too thick
            {
                outline_thickness = 3;
            }
        }

        //Return the outline color and thickness back to its current state if it was changed
        Color current_outline_color = _material.GetColor("_OutlineColor");
        float current_outline_thickness = _material.GetFloat("_OutlineThickness");
        if (current_outline_color != outline_color && health > 1) //Only change color if health is above 1
        {
            _material.SetColor("_OutlineColor", Color.Lerp(current_outline_color, outline_color, 0.02f));
        }
        if (current_outline_thickness > outline_thickness + .001)
        {
            _material.SetFloat("_OutlineThickness", Mathf.Lerp(current_outline_thickness, outline_thickness, .035f));
        }
        

        engine.volume = Mathf.Abs(speed / 30f);
        engine.pitch = 1 + Mathf.Abs(speed / 200f);

        //Boundary check
        //The number here are the current edges of the screen. If you ever change the size, please check these numbers. HARDCODE MASTER LMAOOOO
        if (transform.position.x > manager.level_bounds.x - player_size.x) // Right
        {
            transform.position = new Vector2(manager.level_bounds.x - player_size.x, transform.position.y);
        }
        if (transform.position.x < -manager.level_bounds.x + player_size.x) // Left
        {
            transform.position = new Vector2(-manager.level_bounds.x + player_size.x, transform.position.y);
        }
        if (transform.position.y > manager.level_bounds.y - player_size.y) //Top
        {
            transform.position = new Vector2(transform.position.x, manager.level_bounds.y - player_size.y);
        }
        if (transform.position.y < -manager.level_bounds.y + player_size.y) //Bottom
        {
            transform.position = new Vector2(transform.position.x, -manager.level_bounds.y + player_size.y);
        }

        if(!ui_script.game_is_paused && engine.mute)
        {
            engine.mute = false;
        }

        //print(manager.player_bullet_damage);
    }
}
