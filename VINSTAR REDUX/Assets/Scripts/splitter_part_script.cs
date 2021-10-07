using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class splitter_part_script : MonoBehaviour
{
    public GameObject my_leader;
    private bluesplitter_fighter enemy_script;

    private void Start()
    {
        if (enemy_script != null)
        {
            enemy_script = my_leader.GetComponent<bluesplitter_fighter>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemy_script != null)
        {
            if (collision.gameObject.tag == "bullet" || collision.gameObject.tag == "bossbullet")
            {
                enemy_script.OnTriggerEnter2D(collision);
            }
        }
    }

    private void FixedUpdate()
    {
        if (enemy_script != null)
        {
            enemy_script = my_leader.GetComponent<bluesplitter_fighter>();
        }
    }
}
