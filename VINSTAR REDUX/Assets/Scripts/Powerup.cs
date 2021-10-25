using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup
{
    public enum P_Type { Default, Speed, Acceleration, Fire_Rate, Bullet_Life, Bullet_Speed, Handling, Ship_Size, Bullet_Size, Magnet_Range, Extra_Bullet };
    public P_Type my_powerup = P_Type.Default;
    public List<GameObject> powerup_list = new List<GameObject>();

    //Add a powerup to the list
    public void Add_Powerup(GameObject the_powerup)
    {
        powerup_list.Add(the_powerup);
    }

    //Set which powerup is being held
    public void Set_Powerup(P_Type powerup_type)
    {
        my_powerup = powerup_type;
    }

    //Drop all powerups held and clear the list
    public void Drop_Powerups(Vector3 position)
    {
        if (powerup_list.Count != 0)
        {
            for (int i = 0; i < powerup_list.Count; i++)
            {
                Object.Instantiate(powerup_list[i], position, Quaternion.identity);
            }

            powerup_list.Clear();
        }
    }
}
