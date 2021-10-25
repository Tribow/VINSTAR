using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class p_script : MonoBehaviour
{
    public Powerup.P_Type powerup;

    private manager_script mango;
    private float velocity_angle;
    private float movement_speed_x;
    private float movement_speed_y;
    private Vector3 velocity;
    private Vector2 power_size;

    // Start is called before the first frame update
    void Start()
    {
        mango = GameObject.FindGameObjectWithTag("manager").GetComponent<manager_script>();
        movement_speed_x = Random.Range(-.2f, .2f);
        movement_speed_y = Random.Range(-.2f, .2f);
        velocity_angle = Random.Range(0f, 360f);
        power_size = GetComponent<Collider2D>().bounds.extents;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity = new Vector3(Mathf.Cos(velocity_angle * Mathf.PI / 180) * movement_speed_x, Mathf.Sin(velocity_angle * Mathf.PI / 180) * movement_speed_y);

        if (transform.position.x > mango.level_bounds.x - power_size.x)
        {
            transform.position = new Vector3(mango.level_bounds.x - power_size.x, transform.position.y, transform.position.z);
            velocity_angle += 90f;
        }

        if (transform.position.x < -mango.level_bounds.x + power_size.x)
        {
            transform.position = new Vector3(-mango.level_bounds.x + power_size.x, transform.position.y, transform.position.z);
            velocity_angle += 90f;
        }

        if (transform.position.y > mango.level_bounds.y - power_size.y)
        {
            transform.position = new Vector3(transform.position.x, mango.level_bounds.y - power_size.y, transform.position.z);
            velocity_angle += 90f;
        }

        if (transform.position.y < -mango.level_bounds.y + power_size.y)
        {
            transform.position = new Vector3(transform.position.x, -mango.level_bounds.y + power_size.y, transform.position.z);
            velocity_angle += 90f;
        }

        transform.position += velocity; //Movement
    }
}
