using UnityEngine;

public class asteroid_script : MonoBehaviour
{
    public GameObject mineral;
    public bool is_baby_asteroid;
    GameObject manager;
    float rotation_speed;
    float movement_speed_x;
    float movement_speed_y;
    float[] shake_amt = new float[3];
    int shake = 0;
    float health = 50;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("manager");

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

        //health = 50 * (transform.localScale.x / 2); //Health will scale with size
        shake_amt[0] = -0.03f; shake_amt[1] = 0f; shake_amt[2] = 0.03f; //Assigning the shake numbers
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
                int tempRan = Random.Range(0, 5);
                if (tempRan == 0)
                {
                    Instantiate(mineral, transform.position, transform.rotation);
                }
            }
        }

        if (collision.gameObject.tag == "bullet")
        {
            if (manager != null)
            {   //You're my dad <3 -Liam        "n        i         c         e" -Trinity
                manager.GetComponent<manager_script>().Add_Ascore(2);
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


        if (transform.position.x > 180f || transform.position.x < -180f || transform.position.y > 152f || transform.position.y < -152f)
        {
            movement_speed_x *= -1;
            movement_speed_y *= -1; //reverse direction at the edge of the screen.
        }

        //Destroy
        if (health <= 0)
        {
            if(transform.localScale.x > 2.5f) //Spawn 2 more asteroids if the original asteroid was larger than 2.5
            {
                manager.GetComponent<manager_script>().Spawn_Asteroid(transform.position, transform.localScale.x / 2);
                manager.GetComponent<manager_script>().Spawn_Asteroid(transform.position, transform.localScale.x / 2);
            }
            Destroy(gameObject);
        }
    }
}
