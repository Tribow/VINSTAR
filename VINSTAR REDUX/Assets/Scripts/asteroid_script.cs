using UnityEngine;

public class asteroid_script : MonoBehaviour
{
    public GameObject mineral;
    public GameObject white_mineral;
    public GameObject death_particle;
    public bool is_baby_asteroid;
    GameObject manager;
    manager_script mango;
    audio_manager audiomanager;
    Material _material;
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
        _material = GetComponent<SpriteRenderer>().material;

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

        int health_chance = Random.Range(0, 100);
        if (health_chance < 20)
        {
            _material.SetFloat("_FlashAlpha", .5f);
            white_asteroid = true;
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
            if (manager != null)
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
            Instantiate(death_particle, transform.position, Quaternion.identity);
            audiomanager.Play_Sound(audio_manager.Sound.explosion_01, transform.position, .4f);
            Destroy(gameObject);
        }
    }
}
