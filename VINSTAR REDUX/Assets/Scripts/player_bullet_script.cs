using UnityEngine;

public class player_bullet_script : MonoBehaviour
{
    public float damage = 1;

    GameObject player;
    manager_script manager;
    Stopwatch destroy_timer;
    Vector3 velocity; //Not set to anything atm
    private int speed = 30;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("manager").GetComponent<manager_script>();
        player = GameObject.FindGameObjectWithTag("player");
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;

        destroy_timer = new Stopwatch(1f + (manager.p_bulletlife/10f));
        speed = 30 + manager.p_bulletspeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "asteroid" || collision.gameObject.tag == "enemy" || collision.gameObject.tag == "splitter")
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.right * Time.deltaTime * speed;

        damage = manager.player_bullet_damage;

        destroy_timer.Countdown();
        if (destroy_timer.isFinished()) //Destroy bullet once the timer is done
        {
            Destroy(gameObject);
        }
    }
}
