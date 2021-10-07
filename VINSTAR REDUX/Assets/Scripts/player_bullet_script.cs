using UnityEngine;

public class player_bullet_script : MonoBehaviour
{
    GameObject player;
    int destroy_timer = 60;
    public float damage = 1;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
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
        destroy_timer--;
        transform.position += transform.right * Time.deltaTime * 30;

        if (player != null)
        {
            damage = player.GetComponent<player_ship_script>().bullet_damage;
        }

        if (destroy_timer == 0)
        {
            Destroy(gameObject);
        }
    }
}
