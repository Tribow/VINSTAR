using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class splitter_part_script : MonoBehaviour
{
    public GameObject my_leader;
    private bluesplitter_fighter enemy_script;

    private void Start()
    {
        if (my_leader != null)
        {
            enemy_script = my_leader.GetComponent<bluesplitter_fighter>();
            print("bruh?");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemy_script != null)
        {
            if (collision.gameObject.tag == "bullet") //Got hit by player's bullet? Take damage.
            {
                enemy_script.health = enemy_script.Take_Damage(enemy_script.health, collision.GetComponent<player_bullet_script>().damage);
                enemy_script.Death_Splitter_Handler(true);
                print("yup!");
            }

            if (collision.gameObject.tag == "bossbullet")
            {
                enemy_script.health = enemy_script.Take_Damage(enemy_script.health, 10f);
                enemy_script.Death_Splitter_Handler(false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (my_leader != null)
        {
            enemy_script = my_leader.GetComponent<bluesplitter_fighter>();
            print("really nigga");
        }
    }
}
