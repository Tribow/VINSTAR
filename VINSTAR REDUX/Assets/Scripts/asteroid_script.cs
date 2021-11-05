using UnityEngine;

public class asteroid_script : MonoBehaviour
{
    [Header("Required References")]
    public GameObject mineral;
    public GameObject white_mineral;
    public GameObject death_particle;
    public bool is_baby_asteroid;

    GameObject manager;
    GameObject my_powerup;
    manager_script mango;
    audio_manager audiomanager;
    Powerup powerup = new Powerup();
    SpriteRenderer sprite_renderer;
    Material _material;
    Color particle_color = Color.white;
    ParticleSystem.MainModule my_particle;
    bool white_asteroid = false;
    float rotation_speed;
    float movement_speed_x;
    float movement_speed_y;
    float[] shake_amt = new float[3];
    int shake = 0;
    float health = 50;

    // Start is called before the first frame update
    void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("manager");
        mango = manager.GetComponent<manager_script>();
        audiomanager = GameObject.FindGameObjectWithTag("audio manager").GetComponent<audio_manager>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        _material = sprite_renderer.material;

        shake_amt[0] = -0.03f; shake_amt[1] = 0f; shake_amt[2] = 0.03f; //Assigning the shake numbers
    }

    private void Start()
    {
        //giving random rotation and movement speeds
        if (is_baby_asteroid)
        {
            movement_speed_x = Choice.Choose(-.05f, .05f);
            movement_speed_y = Choice.Choose(-.05f, .05f);
        }
        else
        {
            movement_speed_x = Random.Range(-.01f, .01f);
            movement_speed_y = Random.Range(-.01f, .01f);
        }
        rotation_speed = Random.Range(-1.0f, 1.0f);

        //20% chance the asteroid is a white asteroid
        int health_chance = Random.Range(0, 100);
        if (health_chance < 20)
        {
            _material.SetFloat("_FlashAlpha", .5f);
            white_asteroid = true;
        }

        //5% chance the asteroid is a powerup asteroid
        int powerup_chance = Random.Range(0, 100);
        if (powerup_chance < 5)
        {
            powerup.Set_Powerup((Powerup.P_Type)Random.Range(1, 6)); //This number must be changed with each new powerup
            switch (powerup.my_powerup) //Color of the asteroid will be different based on this value
            {
                case Powerup.P_Type.Speed:
                    my_powerup = LoadPrefab.speed_powerup;
                    sprite_renderer.color = Color.cyan;
                    particle_color = Color.cyan;
                    break;
                case Powerup.P_Type.Acceleration:
                    my_powerup = LoadPrefab.acceleration_powerup;
                    sprite_renderer.color = new Color(215f / 255f, 96f / 255f, 250f / 255f); //purple
                    particle_color = new Color(215f / 255f, 96f / 255f, 250f / 255f);
                    break;
                case Powerup.P_Type.Fire_Rate:
                    my_powerup = LoadPrefab.firerate_powerup;
                    sprite_renderer.color = new Color(1f, 162f / 255f, 0f); //orange
                    particle_color = new Color(1f, 162f / 255f, 0f);
                    break;
                case Powerup.P_Type.Bullet_Life:
                    my_powerup = LoadPrefab.bulletlife_powerup;
                    sprite_renderer.color = Color.magenta;
                    particle_color = Color.magenta;
                    break;
                case Powerup.P_Type.Bullet_Speed:
                    my_powerup = LoadPrefab.bulletspeed_powerup;
                    sprite_renderer.color = Color.yellow;
                    particle_color = Color.yellow;
                    break;
                case Powerup.P_Type.Handling:
                    sprite_renderer.color = Color.green;
                    particle_color = Color.green;
                    break;
                case Powerup.P_Type.Ship_Size:
                    sprite_renderer.color = Color.red;
                    particle_color = Color.red;
                    break;
                case Powerup.P_Type.Bullet_Size:
                    sprite_renderer.color = Color.blue;
                    particle_color = Color.blue;
                    break;
                case Powerup.P_Type.Magnet_Range:
                    sprite_renderer.color = new Color(1f, 192f / 255f, 140f / 255f); //Tan Color
                    particle_color = new Color(1f, 192f / 255f, 140f / 255f);
                    break;
                case Powerup.P_Type.Extra_Bullet:
                    sprite_renderer.color = new Color(0f, 1f, 135f / 255f); //Turqoise Color
                    particle_color = new Color(0f, 1f, 135f / 255f);
                    break;
            }
        }

        //health = 50 * (transform.localScale.x / 2); //Health will scale with size
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "asteroid")
        {
            movement_speed_x *= -1;
            movement_speed_y *= -1;
        }

        if (collision.gameObject.tag != "asteroid")
        {
            if (collision.gameObject.tag != "boundary")
            {
                health--;
                shake = 10;
                int tempRan = Random.Range(0, 25);
                if (tempRan < 1 && white_asteroid)
                {
                    Instantiate(white_mineral, transform.position, transform.rotation);
                }
                else if (tempRan < 4)
                {
                    Instantiate(mineral, transform.position, transform.rotation);
                }
            }
        }

        if (collision.gameObject.tag == "bullet")
        {
            //You're my dad <3 -Liam        "n        i         c         e" -Trinity
            if (mango == null) //Be doubly sure to access the manager script
            {
                manager = GameObject.FindGameObjectWithTag("manager");
                mango = manager.GetComponent<manager_script>();
                mango.Add_Ascore(2);
            }
            else
            {
                mango.Add_Ascore(2);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector2(transform.position.x + movement_speed_x, transform.position.y + movement_speed_y);
        transform.Rotate(Vector3.forward * rotation_speed); //Asteroid movement and rotation


        //Shaking
        if (shake != 0)
        {
            shake--;
            int random_shake1 = Random.Range(0, 3);
            int random_shake2 = Random.Range(0, 3); //shakes in random direction
            transform.position = new Vector2(transform.position.x + shake_amt[random_shake1], transform.position.y + shake_amt[random_shake2]);
        }


        if (transform.position.x > mango.level_bounds.x+1 || 
            transform.position.x < -mango.level_bounds.x-1 || 
            transform.position.y > mango.level_bounds.y+1 || 
            transform.position.y < -mango.level_bounds.y-1)
        {
            movement_speed_x *= -1;
            movement_speed_y *= -1; //reverse direction at the edge of the screen.
        }

        //Destroy
        if (health <= 0)
        {
            if(transform.localScale.x > 2.5f) //Spawn 2 more asteroids if the original asteroid was larger than 2.5
            { //(This code currently doesn't even activate)
                mango.Spawn_Asteroid(transform.position, transform.localScale.x / 2);
                mango.Spawn_Asteroid(transform.position, transform.localScale.x / 2);
            }
            GameObject particle_object = Instantiate(death_particle, transform.position, Quaternion.identity);
            my_particle = particle_object.GetComponent<ParticleSystem>().main;
            my_particle.startColor = particle_color;

            audiomanager.Play_Sound(audio_manager.Sound.explosion_01, transform.position, .4f);

            if(my_powerup != null) //Create the powerup as long as you have one
            {
                Instantiate(my_powerup, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
