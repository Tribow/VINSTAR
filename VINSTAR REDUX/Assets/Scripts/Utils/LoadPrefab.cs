using UnityEngine;

public class LoadPrefab : MonoBehaviour
{
    //These will be the prefabs of each powerup
    public static GameObject speed_powerup = Resources.Load<GameObject>("PowerupSpeed");
    public static GameObject acceleration_powerup = Resources.Load<GameObject>("PowerupAcceleration");
    public static GameObject firerate_powerup = Resources.Load<GameObject>("PowerupFireRate");
    public static GameObject bulletlife_powerup = Resources.Load<GameObject>("PowerupBulletLife");
    public static GameObject bulletspeed_powerup = Resources.Load<GameObject>("PowerupBulletSpeed");
    public static GameObject handling_powerup = Resources.Load<GameObject>("PowerupHandling");
    public static GameObject shipsize_powerup = Resources.Load<GameObject>("PowerupShipSize");
    public static GameObject bulletsize_powerup = Resources.Load<GameObject>("PowerupBulletSize");
    public static GameObject magnetrange_powerup = Resources.Load<GameObject>("PowerupMagnetRange");
    public static GameObject extrabullet_powerup = Resources.Load<GameObject>("PowerupExtraBullet");
}
