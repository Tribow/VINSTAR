using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup
{
    public enum P_Type { Default, Speed, Acceleration, Fire_Rate, Bullet_Life, Bullet_Speed, Handling, Ship_Size, Bullet_Size, Magnet_Range, Extra_Bullet };
    public P_Type my_powerup = P_Type.Default;
    public List<GameObject> powerup_list = new List<GameObject>();

    /// <summary>
    /// Add a powerup to the list
    /// </summary>
    /// <param name="the_powerup">The powerup being added</param>
    public void Add_Powerup(GameObject the_powerup)
    {
        powerup_list.Add(the_powerup);
    }

    /// <summary>
    /// Set which powerup is being held
    /// </summary>
    /// <param name="powerup_type">use the enum to set the type of powerup being held</param>
    public void Set_Powerup(P_Type powerup_type)
    {
        my_powerup = powerup_type;
    }

    /// <summary>
    /// Drop all powerups held and clear the list
    /// </summary>
    /// <param name="position">Where the powerups should drop</param>
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

    /// <summary>
    /// Drop a certain amount of powerups held and clear the list
    /// </summary>
    /// <param name="position">Where the powerups should drop</param>
    /// <param name="amount_dropped">the amount that will drop</param>
    public void Drop_Powerups(Vector3 position, int amount_dropped)
    {
        if (powerup_list.Count != 0)
        {
            for (int i = 0; i < powerup_list.Count; i++)
            {
                Object.Instantiate(powerup_list[i], position, Quaternion.identity);
                if(i >= amount_dropped)
                {
                    break;
                }
            }

            powerup_list.Clear();
        }
    }
}
