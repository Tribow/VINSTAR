using UnityEngine;

public class enemy_bullet_script : MonoBehaviour
{
    public int destroy_timer = 60;
    public int speed = 30;
    public GameObject the_particles;
    public GameObject explode_sound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "asteroid")
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        destroy_timer--;
        transform.position += transform.right * Time.deltaTime * speed;

        if (destroy_timer == 0)
        {
            Destroy(gameObject);
        }
    }
}
