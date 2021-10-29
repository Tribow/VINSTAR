using UnityEngine;

public class LoadPrefab : MonoBehaviour
{
    //These will be the prefabs of each powerup
    public static GameObject speed_powerup = Resources.Load<GameObject>("PowerupSpeed");
    public static GameObject acceleration_powerup = Resources.Load<GameObject>("PowerupAcceleration");
    public static GameObject firerate_powerup = Resources.Load<GameObject>("PowerupFireRate");
    public static GameObject bulletlife_powerup;
    public static GameObject bulletspeed_powerup;
    public static GameObject handling_powerup;
    public static GameObject shipsize_powerup;
    public static GameObject bulletsize_powerup;
    public static GameObject magnetrange_powerup;
    public static GameObject extrabullet_powerup;
}
