using UnityEngine;
using UnityEngine.UI;

public class worker_script : MonoBehaviour
{
    public GameObject the_particles;
    public GameObject explode_sound;
    public GameObject enemy_canvas; //The prefab
    GameObject my_canvas; //For controlling its position
    public GameObject damage_text;
    public GameObject collected_minerals; //This is actually the prefab for minerals, but the enemy will blow up with how many minerals it has
    GameObject boundary;
    Collider2D boundary_check;
    GameObject manager;
    float health = 5;
    float speed = 0;
    float ogspeed = 0;
    float maxspeed = 0;
    float turning_speed = 0;
    float turning_amount = 1f;
    int start_turning = 120;
    float turn_time = 0;
    float[] turning_variable = new float[] { 4f, -4f, 6f, -6f };
    int choice;
    bool mineral_choose = false; //Thanks fixed update
    int mineral_change = 200; //Roughly 3 seconds. This countdown is for making sure an enemy isn't change one position for too long
    string AI;
    Vector3 target_position;
    public GameObject upgrade_object;
    int upgrade_points = 0;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("manager");
        target_position = new Vector3(0f, 0f, 0f);

        boundary = GameObject.FindGameObjectWithTag("boundary");
        speed = Random.Range(3f, 8f); //random speed at start
        ogspeed = speed;
        maxspeed = ogspeed;
        AI = "Base AI";
        boundary_check = boundary.GetComponent<Collider2D>();
        my_canvas = Instantiate(enemy_canvas);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "boundary")
        {
            if (collision.gameObject.tag != "mineral")
            {
                if (collision.gameObject.tag != "bullet")
                {
                    if (collision.gameObject.tag != "ebullet")
                    {
                        speed *= -2;
                    }
                }
            }
        }

        if (collision.gameObject.tag == "bullet")
        {
            health -= collision.gameObject.GetComponent<player_bullet_script>().damage;
            GameObject temp_text = Instantiate(damage_text); //Make the text object (this is to display the damage being dealt)
            temp_text.GetComponent<Text>().text = "" + (int)(collision.gameObject.GetComponent<player_bullet_script>().damage * 10); //Display the damage amount
            temp_text.transform.SetParent(my_canvas.transform, false); //Make sure it is a child of enemy_canvas
        }

        if (collision.gameObject.tag == "mineral")
        {
            upgrade_points++;
            Destroy(collision.gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //This is above the switch AI. These are parameters needed to actually perform some movements
        Collider2D[] near_mineral = Physics2D.OverlapCircleAll(transform.position, 60f, 1 << 8); //Is there a mineral nearby + how many

        var dir = target_position - transform.position; //This will guide the ship towards a position
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; //This will not decide how much it turns
        var center_direction = Quaternion.AngleAxis(angle, Vector3.forward); //Only gets the direction it needs to go
        var IsPointingAt/*aka result*/ = Mathf.Abs(transform.rotation.eulerAngles.z - center_direction.eulerAngles.z); //Won't actually turn the ship

        switch (AI)
        {
            case "Base AI":
                maxspeed = ogspeed / 2; //top speed of base AI

                if (start_turning != 0)
                {
                    start_turning--;
                }
                else if (start_turning == 0)
                {
                    start_turning = 120;
                    turn_time = Random.Range(10f, 15f); //How long it will turn in the base AI
                    choice = Random.Range(0, 2);
                }

                if (turn_time > 0)
                {
                    turn_time--;
                    turning_speed = turning_variable[choice]; //Base turning speed
                }
                else
                {
                    turning_speed = 0;
                }

                //Edge of screen for the enemy
                if (transform.position.x > 160f || transform.position.x < -160f || transform.position.y > 137.5f || transform.position.y < -137.5f)
                {
                    target_position = new Vector3(0f, 0f, 0f);
                    choice = Random.Range(2, 4);
                    AI = "Edge AI";
                }

                if (near_mineral.Length > 0)
                {
                    mineral_choose = true;
                    AI = "Mine AI";
                }

                break;

            case "Edge AI":
                maxspeed = ogspeed / 2; //top speed of base AI

                if (IsPointingAt > 30f)
                {
                    turning_speed = turning_variable[choice];
                }
                else if (IsPointingAt <= 30f)
                {
                    turning_speed = 0;
                }

                //Switching Cases
                if (boundary_check.bounds.Contains(new Vector2(Mathf.Abs(transform.position.x), Mathf.Abs(transform.position.y))))
                {
                    AI = "Base AI";
                }

                break;

            case "Mine AI":
                maxspeed = 8; //max speed of Mine AI

                if (mineral_choose == true)
                {
                    if (near_mineral != null)
                    {
                        int mineral_chosen = Random.Range(0, near_mineral.Length);
                        if (mineral_chosen < near_mineral.Length)
                        {
                            target_position = near_mineral[mineral_chosen].transform.position;
                            mineral_choose = false;
                        }
                    }
                }
                else //Once it has finally found the mineral it wants to chase, it will do the turning
                {
                    mineral_change--; //Start the countdown
                    turning_speed = 0; //using a different style of turning here, not random turns, but calculated ones.
                    if (IsPointingAt > 20f)
                    {
                        turning_amount = 3f;
                    }
                    else
                    {
                        turning_amount = .5f;
                    }

                    transform.rotation = Quaternion.RotateTowards(transform.rotation, center_direction, turning_amount); //New turning method
                    speed += .4f; //gradually raise the speed

                    if (mineral_change == 0) //To make sure they don't circle for too long
                    {
                        mineral_choose = true; //Choose new mineral
                        mineral_change = 200; //reset countdown
                    }
                }

                //Switching cases
                if (transform.position.x > 160f || transform.position.x < -160f || transform.position.y > 137.5f || transform.position.y < -137.5)
                {
                    target_position = new Vector3(0f, 0f, 0f);
                    choice = Random.Range(2, 4);
                    AI = "Edge AI";
                }

                if (near_mineral.Length == 0)
                {
                    AI = "Base AI";
                }

                break;

            default:
                print("oops it broke");

                break;

        }

        //Just so the enemy will keep pushing forward
        if (speed < ogspeed)
        {
            speed += .4f;
        }

        //max speed
        if (speed < -maxspeed * 2)
        {
            speed = -maxspeed * 2;
        }

        if (speed > maxspeed * 2)
        {
            speed = maxspeed * 2;
        }

        transform.Rotate(Vector3.forward * turning_speed); //This is what makes the ship turn

        transform.position += transform.right * Time.deltaTime * speed; //This is what actually makes the ship move

        my_canvas.transform.position = new Vector2(transform.position.x, transform.position.y + 1.6f); //the enemy_canvas position relative to the worker

        //Destroy
        if (health <= 0)
        {
            manager.GetComponent<manager_script>().Add_Score(1);
            manager.GetComponent<manager_script>().Enemy_Death();

            //Particle
            Instantiate(the_particles, transform.position, transform.rotation);

            //Sound
            Instantiate(explode_sound, transform.position, transform.rotation);

            if (upgrade_points > 0)
            {
                for (int drop = upgrade_points / 2; drop > 0; drop--)
                {   //For every upgrade point drop a mineral when dead
                    Instantiate(collected_minerals, transform.position, transform.rotation);
                }
            }

            Destroy(my_canvas);
            Destroy(gameObject);
        }

        if (upgrade_points == 8)
        {
            Instantiate(upgrade_object, transform.position, transform.rotation);
            Destroy(my_canvas);
            Destroy(gameObject);
        }
    }
}
