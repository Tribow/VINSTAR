using UnityEngine;

public class enemy_bullet_script : MonoBehaviour
{
    public float destroy_timer = 1f;
    public int speed = 30;
    public GameObject the_particles;
    public GameObject explode_sound;

    private Stopwatch destroyer = new Stopwatch(1f);

    // Start is called before the first frame update
    void Start()
    {
        destroyer.initial_time = destroy_timer;
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
        destroyer.Countdown();
        transform.position += transform.right * Time.deltaTime * speed;

        if (destroyer.isFinished())
        {
            Destroy(gameObject);
        }
    }
}
