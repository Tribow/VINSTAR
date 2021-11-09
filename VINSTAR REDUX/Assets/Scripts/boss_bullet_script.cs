using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss_bullet_script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "bossbullet"; //set this bullet to a boss bullet
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Make sure the bullet destroys itself on enemies
        if (collision.gameObject.tag == "enemy")
        {
            if (collision.gameObject.GetComponent<Base_Enemy_Script>().am_i_the_boss != true)
            {
                Destroy(gameObject);
            }
        }
    }
}
