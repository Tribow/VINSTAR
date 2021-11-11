using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Be aware that some functions may need to be adjusted based on what new enemies can do

public class Base_Enemy_Script : MonoBehaviour
{
    [Header("Required References & Data")]
    public GameObject enemy_canvas; //The canvas
    public GameObject damage_text; //text used for canvas
    public GameObject collected_minerals; //This is actually the prefab for minerals, but the enemy will blow up with how many minerals it has
    public GameObject white_mineral;
    public GameObject my_bullet;
    public GameObject death_particles; //Particle objects
    public GameObject upgrade_object; //The object the enemy can upgrade to
    public FloatRange set_speed; //The random values the enemy will choose from for its speed
    public bool am_i_the_boss; //This is needed for the death handler
    public bool am_i_white;
    public int score;
    public float health;
    public float acceleration; //How fast the enemy can reach max speed
    public float deceleration; //How fast the enemy can slow down
    public float fire_rate; //How fast the enemy shoots
    public float mineral_radius; //How large the radius is for detecting minerals

    protected GameObject manager; //The manager of the object
    protected GameObject player;
    protected GameObject my_canvas; //reference for instantiation of the canvas
    protected Powerup powerup = new Powerup();
    protected manager_script mango;
    protected List<GameObject> mybullets = new List<GameObject>();
    protected List<GameObject> powerup_temp_list = new List<GameObject>();
    protected Collider2D boundary; //The boundaries the enemy can travel too
    protected Collider2D[] nearby_minerals = new Collider2D[5]; //The ship can see up to 5 minerals within its radius
    protected Coroutine my_coroutine;
    protected IEnumerator coroutine;
    protected State AI;
    protected Material _material;
    protected audio_manager audiomanager;
    protected Vector3 target_position;
    protected Vector3 og_scale;
    protected bool decelerating = false;
    protected bool flash = false;
    protected bool outline = false;
    protected float speed; //This is used for the current speed at all times
    protected float ogspeed; //This is meant to log to initial speed the enemy starts at
    protected float maxspeed; //absolute top speed the enemy can manage
    protected float velocity_angle; //The current angle of the velocity
    protected float turning_speed; //how fast the enemy can turns at any time
    protected Color my_outline_color;
    protected float my_outline_thickness;
    //Powerup Related Variables
    protected float p_speed;
    protected float p_acceleration;
    protected float p_firerate;
    protected float p_bulletlife;
    protected int p_bulletspeed;
    protected float p_handling;
    protected float p_shipsize;
    protected float p_bulletsize;
    protected bool p_magnetrange;
    protected bool p_extrabullet;

    [Header("Public Data - DO NOT EDIT")]
    public Vector3 velocity; //The velocity at any given time
    public enum State { Idle, Edge, Mine, Attack, Evade };
    public int upgrade_points; //The amount of upgrade points it has at any given time

    // Awake is called the moment it is initialized
    public void Awake()
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
        StartCoroutine(Base_Idle(new FloatRange(.1667f, .25f), new FloatRange(4f, 6f), 2)); //Starts the original coroutine
        AI = State.Idle;
        _material = gameObject.GetComponent<SpriteRenderer>().material;
        my_canvas = Instantiate(enemy_canvas);
    }

    // Start is called before the first frame update
    public void Start() 
    {
        og_scale = gameObject.transform.localScale;
        Apply_Powerups(); //Apply any powerup it may have gotten from the previous object it upgraded from

        if (am_i_white) //If this enemy has am_i_white as true right at the start then give it the white buffs
        {
            Im_White();
        }

        _material.SetColor("_OutlineColor", my_outline_color);
        _material.SetFloat("_OutlineThickness", my_outline_thickness);

        //In case this object upgraded from a previous object, this is to make sure it has the powerups
        powerup.powerup_list = new List<GameObject>(powerup_temp_list);

        //DO NOT PUT FOR LOOPS IN THE START FUNCTION THAT IS OMEGA DUMB
    }

    public void OnDestroy()
    {
        Destroy(_material);
        for(int i = 0; i < mybullets.Count; i++)
        {
            Destroy(mybullets[i]);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "boundary" && 
            collision.gameObject.tag != "mineral" &&
            collision.gameObject.tag != "whitemineral" &&
            collision.gameObject.tag != "powerup" &&
            collision.gameObject.tag != "bullet" && 
            collision.gameObject.tag != "ebullet" &&
            collision.gameObject.tag != "bossbullet") //All of these collision checks are for what cant be bounced off of.
        {
            speed *= -2;
        }

        if (collision.gameObject.tag == "bullet") //Got hit by player's bullet? Take damage.
        {
            health = Take_Damage(health, collision.GetComponent<player_bullet_script>().damage);
            Death_Handler(true);
        }

        if (collision.gameObject.tag == "bossbullet")
        {
            health = Take_Damage(health, 10f);
            Death_Handler(false);
        }

        if (collision.gameObject.tag == "ebullet")
        {
            bool do_I_take_damage = true;
            for(int i = 0; i < mybullets.Count; i++)
            { //Loop through the mybullets list and make sure the enemy is not hitting itself with its own bullet
                if(collision.gameObject == mybullets[i])
                {
                    do_I_take_damage = false;
                    break;
                }
            }
            if(do_I_take_damage)
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
            if (collision.GetComponent<p_script>().tangible == true)
            {
                Which_Powerup(collision.GetComponent<p_script>().powerup);
                Destroy(collision.gameObject);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    /// <summary>
    /// Searches for minerals nearby.  It is important to use this before using the Mine AI
    /// </summary>
    /// <param name="position">The center position of the search radius</param>
    /// <param name="view_radius">The length of the search radius</param>
    /// <returns>returns a Collider2D[] containing data of what it found.</returns>
    public Collider2D[] Look_For_Minerals(Vector3 position, float view_radius)
    {
        return Physics2D.OverlapCircleAll(position, view_radius, 1 << 8);
    }

    /// <summary>
    /// Gets the angle of the vector between a target the object's own position returns with a float for the angle.
    /// </summary>
    /// <param name="target">The target's position</param>
    /// <param name="position">Your position (or some others if you're weird)</param>
    /// <returns>returns the angle of the two vectors in degrees</returns>
    public static float Get_Angle(Vector3 target, Vector3 position)
    {
        Vector3 dir = target - position; //to get the angle, first get the direction
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; //and this results in the angle
    }

    /// <summary>
    /// Returns a quaternion with the angle provided. (This rotation only really applies to 2D)
    /// </summary>
    /// <param name="angle_wanted">The angle you want this quaternion to have</param>
    /// <returns>returns a quaternion for the new rotation</returns>
    public static Quaternion Get_Desired_Rotation(float angle_wanted)
    {
        return Quaternion.AngleAxis(angle_wanted, Vector3.forward);
    }

    /// <summary>
    /// Returns the difference between rotations of two quaternions on the Z axis. This will always be a positive number between 0 and 180.
    /// </summary>
    /// <param name="current_rotation">The current rotation of the object</param>
    /// <param name="desired_rotation">The rotation the object wants</param>
    /// <returns>Returns as a float representing the difference in degrees</returns>
    public static float Distance_From_Desired_Rotation(Quaternion current_rotation, Quaternion desired_rotation)
    {
        float result = Mathf.Abs(Mathf.DeltaAngle(current_rotation.eulerAngles.z, desired_rotation.eulerAngles.z)); //desired_rotation.eulerAngles.z - current_rotation.eulerAngles.z;
        return result;
    }

    /// <summary>
    /// Gets the current speed of the object. You will need to be logging the old position somewhere. Call this after changing positions.
    /// </summary>
    /// <param name="old_position">The last position the object was at before it moved</param>
    /// <param name="current_position">Current position of the object after it moved</param>
    /// <returns>Returns with a float of the current speed</returns>
    public static float Get_Speed(Vector3 old_position, Vector3 current_position)
    {
        return Vector3.Distance(old_position, current_position) / Time.deltaTime;
    }

    /// <summary>
    /// Manages the speed of the object and its acceleration. If you want an enemy to move you must have this in a Update function
    /// </summary>
    /// <param name="object_transform">The current transform of the object</param>
    /// <param name="max_speed">The maximum speed value</param>
    /// <returns>Returns with a Vector3 for the current velocity</returns>
    public Vector3 Speed_Management(Transform object_transform, float max_speed)
    {

        velocity_angle = object_transform.eulerAngles.z;
        if (!decelerating)
        {
            speed += acceleration;
            speed = Mathf.Clamp(speed, -max_speed, max_speed);
        }
        else //if it is decelerating
        {
            //velocity.x -= deceleration;
            //velocity.y -= deceleration;
            speed -= deceleration;
            
        }
        velocity = new Vector3(Mathf.Cos(velocity_angle * Mathf.PI / 180) * speed, Mathf.Sin(velocity_angle * Mathf.PI / 180) * speed);
        //Make sure the enemy never goes too fast
        velocity = new Vector3(Mathf.Clamp(velocity.x, -max_speed * Mathf.Abs(velocity.normalized.x), max_speed * Mathf.Abs(velocity.normalized.x)), Mathf.Clamp(velocity.y, -max_speed * Mathf.Abs(velocity.normalized.y), max_speed * Mathf.Abs(velocity.normalized.y)));
        //print(speed);

        return velocity;
    }

    /// <summary>
    /// Handles the rotation, position, and the cavas position of the enemy object. Must be in Update function.
    /// </summary>
    /// <param name="object_transform">The object's transform</param>
    /// <param name="turning_speed">The current turning speed value</param>
    /// <param name="current_velocity">The current velocity of the object</param>
    /// <param name="canvas_offset">The offset of the canvas that displays the text</param>
    public void Transform_Management(Transform object_transform, float turning_speed, Vector3 current_velocity, float canvas_offset)
    {
        if (AI != State.Mine && AI != State.Attack && AI != State.Evade)
        { //Dont use this when the AI is targeting something
            object_transform.Rotate(Vector3.forward * turning_speed); //This is what makes the enemy turn
        }
        object_transform.position += current_velocity * Time.deltaTime; //This is what actually makes the ship move
        
        if(object_transform.localScale.x > 10) //Maximum size for performance reasons
        {
            object_transform.localScale = new Vector3(10, 10, 10);
        }
        else
        {
            float scale_value = og_scale.x + (p_shipsize / 2);
            scale_value = Mathf.Clamp(scale_value, .5f, 10f);
            object_transform.localScale = new Vector3(scale_value, scale_value, scale_value);
        }

        if (my_canvas != null)
        {
            my_canvas.transform.position = new Vector2(object_transform.position.x, object_transform.position.y + canvas_offset); //the enemy_canvas position relative to the worker
        }
    }

    protected void Flash_Off()
    {
        _material.SetFloat("_FlashAlpha", 0f);
    }

    /// <summary>
    /// Handles the logic for taking damage
    /// </summary>
    /// <param name="current_health">The object's current health</param>
    /// <param name="bullet_strength">The strength of the bullet that hit it</param>
    /// <returns>Returns with the float for new health</returns>
    public float Take_Damage(float current_health, float bullet_strength)
    {
        if (my_canvas != null)
        {
            GameObject temp_text = Instantiate(damage_text); //Make the text object (this is to display the damage being dealt)
            temp_text.GetComponent<Text>().text = "" + (int)(bullet_strength * 10); //Display the damage amount
            temp_text.transform.SetParent(my_canvas.transform, false); //Make sure it is a child of the canvas
            _material.SetFloat("_FlashAlpha", .5f);
            Invoke("Flash_Off", .05f);
        }
        return current_health - bullet_strength; //lower health;
    }

    /// <summary>
    /// Used whenever you need to check for death (likely after damage gets taken)
    /// </summary>
    /// <param name="the_manager">The game manager in hierarchy.</param>
    /// <param name="give_points">Whether or not the it gives points on death</param>
    public void Death_Handler(bool give_points)
    {
        if (health <= 0) //if health low enough, the time to kill it, dont do anything otherwise
        {
            if(give_points) //if give points get set, it will give points. This should be set to true if the player is dealing the killing blow
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

            if (upgrade_points > 0)
            {
                for (int drop = upgrade_points / 2; drop > 0; drop--)
                {   //For every upgrade point drop a mineral when dead
                    Instantiate(collected_minerals, gameObject.transform.position, gameObject.transform.rotation);
                }
            }

            if (am_i_white)
            {
                int tempRan = Random.Range(0, 4);
                if(tempRan == 0)
                {
                    //1/4th of a chance to spawn a white mineral if it got one
                    Instantiate(white_mineral, gameObject.transform.position, gameObject.transform.rotation);
                }
            }

            //Also drop any powerups owned
            if (am_i_the_boss) //Boss drops all powerups held
            {
                powerup.Drop_Powerups(transform.position);
            }
            else //Normal enemies can only drop up to 5
            {
                powerup.Drop_Powerups(transform.position, 5);
            }

            //Be sure to destroy extra objects
            Destroy(my_canvas);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// This should be called in the update function of an enemy that is capable of upgrading itself. 
    /// It will upgrade the enemy once it meets the requirements
    /// </summary>
    /// <param name="minerals_got">The amount of minerals the enemy has</param>
    /// <param name="minerals_needed">The amount the enemy needs to uprade</param>
    /// <param name="the_object"></param>
    /// <param name="upgraded_object">The object the enemy will upgrade into</param>
    public void Upgrade_Enemy(int minerals_got, int minerals_needed, GameObject upgraded_object)
    {
        GameObject new_object;
        if (minerals_got == minerals_needed)
            {
            new_object = Instantiate(upgraded_object, gameObject.transform.position,gameObject.transform.rotation);
            Base_Enemy_Script object_script = new_object.GetComponent<Base_Enemy_Script>();
            if (mango == null) //Be doubly sure to access the manager script
            {
                manager = GameObject.FindGameObjectWithTag("manager");
                mango = manager.GetComponent<manager_script>();
                mango.Add_To_Enemy_List(new_object);
            }
            else
            {
                mango.Add_To_Enemy_List(new_object);
            }

            if (am_i_white) //Make sure that the upgraded object is also white
            {
                object_script.am_i_white = true;
            }

            object_script.powerup_temp_list = new List<GameObject>(powerup.powerup_list);
            object_script.my_outline_color = my_outline_color;
            object_script.my_outline_thickness = my_outline_thickness;
            object_script.p_speed = p_speed;
            object_script.p_acceleration = p_acceleration;
            object_script.p_firerate = p_firerate;
            object_script.p_bulletlife = p_bulletlife;
            object_script.p_bulletspeed = p_bulletspeed;
            object_script.p_handling = p_handling;
            object_script.p_shipsize = p_shipsize;
            object_script.p_bulletsize = p_bulletsize;
            object_script.p_magnetrange = p_magnetrange;
            object_script.p_extrabullet = p_extrabullet;

            Destroy(my_canvas);
            Destroy(gameObject);
            }
    }

    #region Powerup Related Functions
    protected void Which_Powerup(Powerup.P_Type the_powerup)
    {
        switch (the_powerup) //Color of the asteroid will be different based on this value
        {
            case Powerup.P_Type.Speed:
                p_speed += 10;
                ogspeed += 10;
                powerup.Add_Powerup(LoadPrefab.speed_powerup);
                _material.SetColor("_OutlineColor", Color.cyan);
                _material.SetFloat("_OutlineThickness", 1f);
                my_outline_color = Color.cyan;
                my_outline_thickness = 1f;
                break;
            case Powerup.P_Type.Acceleration:
                acceleration += 0.2f;
                p_acceleration += 0.2f;
                powerup.Add_Powerup(LoadPrefab.acceleration_powerup);
                _material.SetColor("_OutlineColor", new Color(215f / 255f, 96f / 255f, 250f / 255f));
                _material.SetFloat("_OutlineThickness", 1f);
                my_outline_color = new Color(215f / 255f, 96f / 255f, 250f / 255f);
                my_outline_thickness = 1f;
                break;
            case Powerup.P_Type.Fire_Rate:
                fire_rate -= .04f;
                p_firerate += .04f;
                powerup.Add_Powerup(LoadPrefab.firerate_powerup);
                _material.SetColor("_OutlineColor", new Color(1f, 162f / 255f, 0f));
                _material.SetFloat("_OutlineThickness", 1f);
                my_outline_color = new Color(1f, 162f / 255f, 0f);
                my_outline_thickness = 1f;
                break;
            case Powerup.P_Type.Bullet_Life:
                p_bulletlife++;
                powerup.Add_Powerup(LoadPrefab.bulletlife_powerup);
                _material.SetColor("_OutlineColor", Color.magenta);
                _material.SetFloat("_OutlineThickness", 1f);
                my_outline_color = Color.magenta;
                my_outline_thickness = 1f;
                break;
            case Powerup.P_Type.Bullet_Speed:
                p_bulletspeed += 5;
                powerup.Add_Powerup(LoadPrefab.bulletspeed_powerup);
                _material.SetColor("_OutlineColor", Color.yellow);
                _material.SetFloat("_OutlineThickness", 1f);
                my_outline_color = Color.yellow;
                my_outline_thickness = 1f;
                break;
            case Powerup.P_Type.Handling:
                p_handling += 2;
                powerup.Add_Powerup(LoadPrefab.handling_powerup);
                _material.SetColor("_OutlineColor", Color.green);
                _material.SetFloat("_OutlineThickness", 1f);
                my_outline_color = Color.green;
                my_outline_thickness = 1f;
                break;
            case Powerup.P_Type.Ship_Size:
                p_shipsize++;
                powerup.Add_Powerup(LoadPrefab.shipsize_powerup);
                _material.SetColor("_OutlineColor", Color.red);
                _material.SetFloat("_OutlineThickness", 1f);
                my_outline_color = Color.white;
                my_outline_thickness = 1f;
                break;
            case Powerup.P_Type.Bullet_Size:
                p_bulletsize += 2;
                powerup.Add_Powerup(LoadPrefab.bulletsize_powerup);
                _material.SetColor("_OutlineColor", Color.blue);
                _material.SetFloat("_OutlineThickness", 1f);
                my_outline_color = Color.blue;
                my_outline_thickness = 1f;
                break;
            case Powerup.P_Type.Magnet_Range:
                _material.SetColor("_OutlineColor", new Color(1f, 192f / 255f, 140f / 255f));
                _material.SetFloat("_OutlineThickness", 1f);
                my_outline_color = new Color(1f, 192f / 255f, 140f / 255f);
                my_outline_thickness = 1f;
                break;
            case Powerup.P_Type.Extra_Bullet:
                _material.SetColor("_OutlineColor", new Color(0f, 1f, 135f / 255f));
                _material.SetFloat("_OutlineThickness", 1f);
                my_outline_color = new Color(0f, 1f, 135f / 255f);
                my_outline_thickness = 1f;
                break;
        }
    }

    protected void Apply_Powerups()
    {
        ogspeed += p_speed;
        acceleration += p_acceleration;
        fire_rate -= p_firerate;
    }

    public void Im_White()
    {
        am_i_white = true;
        _material.SetColor("_OutlineColor", Color.white);
        _material.SetFloat("_OutlineThickness", 1f);
        my_outline_color = Color.white;
        my_outline_thickness = 1f;
        if (mango == null) //Be doubly sure to access the manager script
        {
            manager = GameObject.FindGameObjectWithTag("manager");
            mango = manager.GetComponent<manager_script>();
            health += mango.player_bullet_damage;
        }
        else
        {
            health += mango.player_bullet_damage;
        }
    }
    #endregion

    /// <summary>
    /// Write over whenever the enemy is going to attack in some sort of way. Write how it attacks here
    /// If you don't replace this method and the enemy attacks it will use this as default
    /// </summary>
    public virtual void Attack_Method()
    {
        GameObject the_bullet = Instantiate(my_bullet, transform.position, transform.rotation);
        enemy_bullet_script bullet_script = the_bullet.GetComponent<enemy_bullet_script>();
        bullet_script.destroy_timer = 1 + p_bulletlife;
        bullet_script.speed = 30 + p_bulletspeed;
        p_bulletsize = Mathf.Clamp(p_bulletsize, 0f, 10f); //Cannot spawn bullet at a bigger scale than 10
        the_bullet.transform.localScale = new Vector3(the_bullet.transform.localScale.x + p_bulletsize, the_bullet.transform.localScale.y + p_bulletsize, the_bullet.transform.localScale.z + p_bulletsize);
        audiomanager.Play_Sound(audio_manager.Sound.shoot_01, transform.position, 1.8f);
        mybullets.Add(the_bullet);
    }

    /// <summary>
    /// The base Idle movement. This will be used as the default movement for most enemy ships. 
    /// It will keep moving forward and occassionally turn at set intervals.
    /// </summary>
    /// <param name="turning_time">How long the enemy can turn</param>
    /// <param name="turning_amount">How fast the enemy's turn speed will be</param>
    /// <param name="wait_time">How long it will wait before trying to turn (in seconds)</param>
    /// <returns></returns>
    public IEnumerator Base_Idle(FloatRange turning_time, FloatRange turning_amount, float wait_time)
    {
        //Make sure to loop these by wrapping them in an infinite for loop
        for (;;)
        {
            //Wait first for the alotted wait time
            yield return new WaitForSeconds(wait_time);

            //Set turning speed. The Random.Range at the end decides which direction it will turn...or if it turns at all
            //Wait for a random amount of time before finally...
            turning_speed = (turning_amount.Random + p_handling) * (Random.Range(0, 3) + -1);
            yield return new WaitForSeconds(turning_time.Random);

            //Setting the turn speed back to 0 and repeating the process
            turning_speed = 0;
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// This should be used the moment the object gets too far out to make sure they stay within the arena.
    /// The object will turn until its angle is within the distance desired from the angle towards the center of the game.
    /// </summary>
    /// <param name="distance_desired">How close the object's angle should be before it stops turning. (I recommend at least 45 degrees)</param>
    /// <param name="turning_velocity">How fast the object will turn</param>
    /// <returns>Waits for fixed update</returns>
    public IEnumerator Edge_Movement(float distance_desired, FloatRange turning_velocity)
    {
        float turn_direction = (turning_velocity.Random + p_handling) * Choice.Choose(-1f, 1f);
        for (;;)
        {
            target_position = new Vector3(0, 0, 0); //The new target for the enemy at this time is the center of the level
            float angle_wanted = Get_Angle(target_position, transform.position);
            Quaternion desired_angle = Get_Desired_Rotation(angle_wanted);
            float distance_from_angle = Distance_From_Desired_Rotation(transform.rotation, desired_angle);
            //Turn until you get close enough to the desired angle, from then on, you don't need to turn anymore
            if (distance_from_angle > distance_desired)
            {
                turning_speed = turn_direction;
            }
            else if (distance_from_angle <= distance_desired)
            {
                turning_speed = 0;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Movement used for running away from target instead of going towards it.
    /// It will turn until it is facing the desired distance from the angle towards the target
    /// </summary>
    /// <param name="target_position">The target that it will be running away from</param>
    /// <param name="distance_desired">How far away the angle should be away from the target. Cannot go above 180</param>
    /// <param name="turning_velocity">How fast it can turn</param>
    /// <returns>Waits for fixed update</returns>
    public IEnumerator Evade_Movement(Vector3 target_position, float distance_desired, FloatRange turning_velocity)
    {
        turning_speed = 0;
        float turn_speed = turning_velocity.Random;
        if (distance_desired > 180)
            distance_desired = 180;
        for (;;)
        {
            float angle_wanted = Get_Angle(target_position, transform.position);
            Quaternion desired_angle = Get_Desired_Rotation(angle_wanted);
            float distance_from_angle = Distance_From_Desired_Rotation(transform.rotation, desired_angle);
            if (distance_from_angle < distance_desired)
            {
                turning_speed = turn_speed + p_handling;
            }
            else if (distance_from_angle >= distance_desired)
            {
                turning_speed = 0;
            }

            desired_angle.eulerAngles = new Vector3(desired_angle.eulerAngles.x, desired_angle.eulerAngles.y, desired_angle.eulerAngles.z + 180);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desired_angle, turning_speed); //New turning method

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Movement used for running away from target instead of going towards it.
    /// It will turn until it is facing the desired distance from the angle towards the target
    /// </summary>
    /// <param name="target_position">The target that it will be running away from</param>
    /// <param name="distance_desired">How far away the angle should be away from the target. Cannot go above 180</param>
    /// <param name="turning_velocity">How fast it can turn</param>
    /// <returns>Waits for fixed update</returns>
    public IEnumerator Evade_Movement(GameObject target, float distance_desired, FloatRange turning_velocity)
    {
        turning_speed = 0; //Not already turning settings
        float turn_speed = turning_velocity.Random;
        Vector3 target_position;
        for (;;)
        {
            if (target != null)
            {
                target_position = target.transform.position;
                float angle_wanted = Get_Angle(target_position, transform.position);
                Quaternion desired_angle = Get_Desired_Rotation(angle_wanted);
                float distance_from_angle = Distance_From_Desired_Rotation(transform.rotation, desired_angle);
               
                if (distance_from_angle < distance_desired)
                {
                    turning_speed = turn_speed + p_handling;
                }
                else if (distance_from_angle >= distance_desired)
                {
                    turning_speed = 0;
                }

                desired_angle.eulerAngles = new Vector3(desired_angle.eulerAngles.x, desired_angle.eulerAngles.y, desired_angle.eulerAngles.z + 180);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, desired_angle, turning_speed); //New turning method
            }

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// This AI is focused around targeting minerals when they're close enough. It will choose a mineral to go for and attempt to grab it
    /// The accuracy in which an AI can grab the mineral should vary based on turning speed and how fast the enemy can move
    /// Make sure to set the maxspeed back to normal once you're done with this AI
    /// </summary>
    /// <param name="minerals_nearby">A Collider2D[] containing data of the minerals nearby</param>
    /// <param name="target_focus">How long it will focus on a mineral's position</param>
    /// <param name="distance_desired">How close the object's angle should be towards its target</param>
    /// <param name="mineral_turn_speed">How fast it will turn when focused on mineral</param>
    /// <param name="mineral_turn_accuracy">Modifier used when within the desired angle. Number cannot be above 1. 1 = perfect accuracy, 0 = no accuracy</param>
    /// <param name="new_max_speed">The new  max speed it will use for mining</param>
    /// <returns>Waits for fixed update</returns>
    public IEnumerator Mineral_Movement(Collider2D[] minerals_nearby, Stopwatch target_focus, float distance_desired, float mineral_turn_speed, float mineral_turn_accuracy, float new_max_speed)
    {
        //local variables
        bool choose_mineral = true;
        Vector3 target_position = new Vector3(0, 0, 0);
        turning_speed = 0; //Make sure you're not turning for some reason
        maxspeed = new_max_speed + p_speed; //Use new speed
        if (mineral_turn_accuracy > 1) //if it was above 1 it would miss the point of the variable
            mineral_turn_accuracy = 1;

        for (;;)
        {
            if (choose_mineral) //If it is time to choose which mineral to go for
            {
                if (minerals_nearby != null) //Check for any minerals nearby, then choose a single one to aim for
                {
                    int mineral_chosen = Random.Range(0, minerals_nearby.Length);
                    if (mineral_chosen < minerals_nearby.Length)
                    {
                        if(minerals_nearby[mineral_chosen] == null)
                            for(int i = 0; i < minerals_nearby.Length; i++)
                                if (minerals_nearby[i] != null)
                                { //Make sure to not actually end up with null
                                    mineral_chosen = i;
                                    break;
                                }
                        target_position = minerals_nearby[mineral_chosen].transform.position;
                        choose_mineral = false; //No need to check for minerals now that one has been chosen
                    }
                }
            }
            else //Once it has finally found the mineral it wants to chase, it will do the turning
            {
                float angle_wanted = Get_Angle(target_position, transform.position);
                Quaternion desired_angle = Get_Desired_Rotation(angle_wanted);
                float distance_from_angle = Distance_From_Desired_Rotation(transform.rotation, desired_angle);

                //DrawThis.Polygon(gameObject, 3, 3f, new Vector3(target_position.x, target_position.y, -5), .1f, .1f);

                target_focus.Countdown(); //begin to countdown how long it will focus on getting this mineral
                if (distance_from_angle > distance_desired)
                { //Use maximum turning speed if it's not close enough to distance_desired
                    turning_speed = mineral_turn_speed + p_handling;
                }
                else
                { //Apply this calculation once it's in the distance_desired. The larger the accuracy, the more likely it will pick up the mineral
                    turning_speed = (mineral_turn_speed + p_handling) * mineral_turn_accuracy;
                }

                transform.rotation = Quaternion.RotateTowards(transform.rotation, desired_angle, turning_speed); //New turning method

                if (target_focus.isFinished()) //To make sure they don't circle for too long
                {
                    choose_mineral = true; //Choose new mineral we're choosing a new mineral
                    target_focus.Reset(); //reset the focus timer
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// This AI will be focused on the player. It will give chase when it's too far, then begin to aim and shoot once it gets close enough
    /// A lot of adjustments can be made for how aggressive its shots are and how accurate its movements are
    /// Make sure to set the maxspeed back to normal once you're done with this AI
    /// </summary>
    /// <param name="new_max_speed">the new max speed</param>
    /// <param name="angle_desired">how close rotation angle should be to its target</param>
    /// <param name="distance_desired">how close the transform position should be to its target</param>
    /// <param name="firing_distance">how close it can get before firing</param>
    /// <param name="fire_rate">How fast the object can shoot</param>
    /// <param name="recharge_speed">How long it waits until shooting again</param>
    /// <param name="firing_time">How long it shoots once it starts shooting</param>
    /// <param name="aim_speed">How fast it moves once it's aiming (aims when within desired angle)</param>
    /// <param name="aim_turning">How fast it turns once it's aiming (aims when within desired angle)</param>
    /// <param name="chase_speed">How fast it turns when chasing. (Chases when outside desired distance)</param>
    /// <returns>waits for fixed update</returns>
    public IEnumerator Attack_Movement(float new_max_speed, float angle_desired, float distance_desired, float firing_distance, Stopwatch fire_rate, FloatRange recharge_speed, FloatRange firing_time, float aim_speed, float aim_turning, float chase_speed)
    {
        //local variables
        player = GameObject.FindGameObjectWithTag("player");
        Vector3 target_position;
        turning_speed = 0; //make sure you're not currently turning
        Stopwatch recharge = new Stopwatch(recharge_speed.Random);
        float firing_duration = firing_time.Random;

        for(;;)
        {   //Don't do anything if the player is suddenly dead.
            if(player != null)
            {
                target_position = player.transform.position; //Track the player
                float angle_wanted = Get_Angle(target_position, transform.position);
                Quaternion desired_angle = Get_Desired_Rotation(angle_wanted);
                float distance_from_angle = Distance_From_Desired_Rotation(transform.rotation, desired_angle);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, desired_angle, turning_speed); //New turning method
                decelerating = false; //by default it should be false

                if (Vector2.Distance(transform.position, target_position) < distance_desired)
                { //Once it gets close enough to the player, it will slow down to its aiming speed and turning
                    if (speed > aim_speed)
                        decelerating = true;
                    turning_speed = aim_turning + p_handling;
                }
                else
                { //If it isn't close enough then move faster and turn with the chase turning speed
                    turning_speed = chase_speed + p_handling;
                    maxspeed = ogspeed;
                    if (distance_from_angle <= angle_desired)
                        maxspeed = new_max_speed + p_speed;
                }

                if (Vector2.Distance(transform.position, target_position) < firing_distance)
                { //shoot at player if they're close enough (it is recommended to keep that distance below distance_desired)
                    recharge.Countdown();
                    if (recharge.current_time < firing_duration)
                    { //Shoot once recharge's time is lower than firing duration
                        fire_rate.Countdown();
                        if (fire_rate.isFinished())
                        {
                            Attack_Method();
                            fire_rate.Reset();
                        }
                    }

                    if (recharge.isFinished())
                    {
                        recharge.Reset();
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
