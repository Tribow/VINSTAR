using UnityEngine;

public class mineral_script : MonoBehaviour
{
    public float velocity_angle;
    public float movement_speed_x;
    public float movement_speed_y;

    Vector3 velocity;
    int destroy_timer = 500;
    manager_script mango;

    // Start is called before the first frame update
    void Start()
    {
        mango = GameObject.FindGameObjectWithTag("manager").GetComponent<manager_script>();
        movement_speed_x = Random.Range(-.01f, .01f);
        movement_speed_y = Random.Range(-.01f, .01f);
        velocity_angle = transform.eulerAngles.z;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        destroy_timer--;

        velocity = new Vector3(Mathf.Cos(velocity_angle * Mathf.PI / 180) * movement_speed_x, Mathf.Sin(velocity_angle * Mathf.PI / 180) * movement_speed_y);

        transform.position += velocity;
        transform.Rotate(Vector3.forward * 7); //Mineral movement and rotation

        if (mango.make_next_level) //Mineral will remove itself if manager is making the next level
            destroy_timer = 0;

        if (destroy_timer == 0)
        {
            Destroy(gameObject);
        }
    }
}
