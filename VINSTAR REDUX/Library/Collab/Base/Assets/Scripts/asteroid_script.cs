using UnityEngine;

public class asteroid_script : MonoBehaviour
{
    public GameObject mineral;
    GameObject manager;
    float rotation_speed;
    float movement_speed_x;
    float movement_speed_y;
    float asteroid_x;
    float asteroid_y;
    float[] shake_amt = new float[3];
    int shake = 0;
    int health = 50;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("manager");

        movement_speed_x = Random.Range(-.01f, .01f); //giving random rotation and movement speeds
        movement_speed_y = Random.Range(-.01f, .01f);
        rotation_speed = Random.Range(-1.0f, 1.0f);

        asteroid_x = Random.Range(-178f, 178f);
        asteroid_y = Random.Range(-150f, 150f); //giving asteroids random position

        shake_amt[0] = -0.03f; shake_amt[1] = 0f; shake_amt[2] = 0.03f; //Assigning the shake numbers

        transform.position = new Vector2(asteroid_x, asteroid_y);
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
            {   //You're my dad <3 -Liam        "n        i         c         e" -Baptastic
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
            Destroy(gameObject);
        }
    }
}
