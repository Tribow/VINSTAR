using UnityEngine;

public class camera_script : MonoBehaviour
{
    Transform player; //Must be player in the scene
    player_ship_script player_script;
    public manager_script manager;
    public Camera cam;
    public bool transitioning;
    float speed = 0.1f;
    float max_speed = 0.2f;
    float slowdown = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //GOOD ASS CAMERA
        if (player != null)
        {
            // + (cam.orthographicSize - 9f)*1.5f
            if (Vector2.Distance(player.position, transform.position) > 15)
            {

                if (transitioning)
                {
                    max_speed = 2f;

                }
                else
                {
                    if (player_script.speed / 50 > 2f)
                        max_speed = player_script.speed / 50;
                    else
                        max_speed = 2f;
                    speed = max_speed;
                }
            }
            else
            {
                transitioning = false;
                if (player_script.speed / 50 < .18f)
                    max_speed = .4f;
                else
                    max_speed = player_script.speed / 50;
            }

            if (Vector2.Distance(player.position, transform.position) < 5)
            {
                slowdown += 0.1f;

                speed -= 0.0055f / slowdown;

                transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.position.x, player.position.y, transform.position.z), speed);
            }
            else
            {
                slowdown = 1;
                speed += 0.005f;

                //Based on the speed above the camera will follow the player.
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.position.x, player.position.y, transform.position.z), speed);
            }


            if (!transitioning)
            {
                float max_cam_zoom = 9f;
                float zoom_speed = .01f;
                
                if (player_script.speed > 5f)
                {
                    max_cam_zoom = 9f + (player_script.speed - 5f) * 1.2f; //player_script.speed/2f + 9f
                }
                else
                {
                    zoom_speed = Mathf.Lerp(zoom_speed, .05f, .5f);
                }

                //print(manager.boss_object.activeInHierarchy);
                if (manager.boss_object != null)
                {
                    if (Vector2.Distance(transform.position, manager.boss_object.transform.position) < 80f)
                    {
                        //print(Vector2.Distance(transform.position, manager.boss_object.transform.position));
                        max_cam_zoom = 20f;
                        zoom_speed = .01f;
                    }
                }

                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, max_cam_zoom, zoom_speed);
            }
            else
            {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 9, .01f);
            }
        }
        else
        {
            transitioning = true;
            GameObject the_player = GameObject.FindGameObjectWithTag("player");
            if (the_player != null)
            {
                player = the_player.transform;
                player_script = player.GetComponent<player_ship_script>();
            }
        }

        if (speed > max_speed)
        {
            speed = max_speed; //Make sure speed never goes above max_speed
        }
        else if (speed < 0)
        {
            speed = 0; //Make sure it never goes into negatives
        }

        //print(Vector2.Distance(player.position, transform.position));

        //Boundary check
        //The number here are the current edges of the camera screen. If you ever change the size, please check these numbers.
        if (transform.position.x > manager.level_bounds.x - cam.orthographicSize* (16f / 9f)) // Right
        {
            transitioning = true;
            transform.position = new Vector3(manager.level_bounds.x - cam.orthographicSize* (16f / 9f), transform.position.y, transform.position.z);
        }
        if (transform.position.x < -manager.level_bounds.x + cam.orthographicSize * (16f/9f)) // Left
        {
            transitioning = true;
            transform.position = new Vector3(-manager.level_bounds.x + cam.orthographicSize * (16f / 9f), transform.position.y, transform.position.z);
        }
        if (transform.position.y > manager.level_bounds.y - cam.orthographicSize) //Top
        {
            transitioning = true;
            transform.position = new Vector3(transform.position.x, manager.level_bounds.y - cam.orthographicSize, transform.position.z);
        }
        if (transform.position.y < -manager.level_bounds.y + cam.orthographicSize) //Bottom
        {
            transitioning = true;
            transform.position = new Vector3(transform.position.x, -manager.level_bounds.y + cam.orthographicSize, transform.position.z);
        }

        //DrawThis.Polygon(gameObject, 10, 2f, new Vector3(transform.position.x, transform.position.y, -5f), .1f, .1f);

        //print(transform.position);
    }
}
