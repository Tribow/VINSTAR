using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class player_ship_script : MonoBehaviour
{
    
    public GameObject bullet;
    public int immune_timer;
    public float bullet_damage = 1;
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

    GameObject manager;
    audio_manager audiomanager;
    public float speed;
    public Vector3 velocity;
    float velocity_angle;
    private float max_og_speed;
    private Stopwatch magnet_timer = new Stopwatch(1.5f);
    
    private Vector2 input_value;

    Player_Actions.Player_ControlsActions player_input;

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
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("manager");
        audiomanager = GameObject.FindGameObjectWithTag("audio manager").GetComponent<audio_manager>();
        immune_timer = 180;
        velocity_angle = transform.eulerAngles.z;
        max_og_speed = max_speed;

        //Sound settings
        engine.Play(); //Play sound immediately
        engine.loop = true; //Loop it
        engine.volume = 0; //Have it at zero so it makes no noise at first
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "boundary" && 
            collision.gameObject.tag != "mineral" && 
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

        if (collision.gameObject.tag == "ebullet")
        {
            if (immune_timer <= 0)
            {
                //Particle
                Instantiate(the_particles, transform.position, transform.rotation);

                //Sound
                audiomanager.Play_Sound(audio_manager.Sound.explosion_01, transform.position);

                Destroy(gameObject); //It can only destroy player if the immune_timer is 0
            }
        }

        if (collision.gameObject.tag == "mineral")
        {
            manager.GetComponent<manager_script>().Add_Damage();
            Destroy(collision.gameObject);
        }
    }

    private void Update()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        immune_timer--;
        Vector3 old_position = transform.position;
        bullet_damage = manager.GetComponent<manager_script>().player_bullet_damage;

        input_value = new Vector2(player_input.TurnRight.ReadValue<float>() + (player_input.TurnLeft.ReadValue<float>() * -1f), player_input.Acceleration.ReadValue<float>() + (player_input.Deceleration.ReadValue<float>() * -1f));

        if (player_input.Shoot.ReadValue<float>() == 1f) //Start shooting when button is held
        {
            bullet_timer.Countdown();
            magnet_timer.Reset();
            if (bullet_timer.isFinished()) //only fire a shot when countdown is done
            {
                bullet_timer.Reset();
                Instantiate(bullet);
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

        if(player_input.Slow.ReadValue<float>() == 1f)
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

        engine.volume = Mathf.Abs(speed / 30);

        //Boundary check
        //The number here are the current edges of the screen. If you ever change the size, please check these numbers. HARDCODE MASTER LMAOOOO
        if (transform.position.x > 178f) // Right
        {
            transform.position = new Vector2(178f, transform.position.y);
        }
        if (transform.position.x < -178f) // Left
        {
            transform.position = new Vector2(-178f, transform.position.y);
        }
        if (transform.position.y > 150.7f) //Top
        {
            transform.position = new Vector2(transform.position.x, 150.7f);
        }
        if (transform.position.y < -150.7f) //Bottom
        {
            transform.position = new Vector2(transform.position.x, -150.7f);
        }

        if(!ui_script.game_is_paused && engine.mute)
        {
            engine.mute = false;
        }

        //print(manager.GetComponent<manager_script>().player_bullet_damage);
    }
}
