using UnityEngine;

public class damage_text_script : MonoBehaviour
{
    int death_timer = 60;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Count down the death timer and slowly move the damage upward
        death_timer--;
        transform.position = new Vector2(transform.position.x, transform.position.y + .01f);

        if (death_timer == 0)
        {
            Destroy(gameObject);
        }
    }
}
