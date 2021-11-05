using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; //I guess I'm not usin it??

public class manager_script : MonoBehaviour
{

    [Header("Required References")] 
    public GameObject[] asteroids;
    public GameObject[] enemies;
    public GameObject[] bosses;
    public ui_script ui_manager;
    public Text score_text;
    public Text lives_text;
    public Text kills_text;
    public Text level_text;
    public GameObject next_level_text;
    public camera_script cam;
    public GameObject username;
    public GameObject submit;
    public GameObject leaderboard;
    public GameObject radar;
    public GameObject player_object; //The literal prefab of player
    public GameObject main_menu;
    public GameObject boss_pointer;

    [Header("Public Data - DO NOT EDIT")]
    public GameObject player; //what is used to see player in the scene
    public GameObject boss_object; //what is used to see the boss on the scene
    public Vector2 level_bounds;
    public int current_level = 1; //What level are you currently on
    public int enemy_score = 0;
    public int asteroid_score = 0;
    public int player_lives = 3; //How many lives the player has
    public int total_score = 0;
    public int kills_left; //Kills left in order to move on
    public float player_bullet_damage = 1f; //How much damage the player's bullet will do
    public float boss_health; //what is used to log the boss's health value
    public bool game_over = false; //is the game over yet
    public bool start_game = false; //For playing a second time
    public bool make_next_level = false; //Whether or not the next level is being made
    public Powerup player_powerups = new Powerup();
    public int p_speed;
    public int p_acceleration;
    public int p_firerate;
    public int p_bulletlife;
    public int p_bulletspeed;
    /*public int p_handling;
    public int p_shipsize;
    public int p_bulletsize;
    public int p_magnetrange;
    public int p_extrabullet;*/

    Stopwatch next_level_timer = new Stopwatch(3f);
    Stopwatch respawn_timer = new Stopwatch(2f); //how long it takes to respawn
    Stopwatch reset_countdown = new Stopwatch(5f); //The amount of time the leaderboard stays up until it kills entire game, ready to reset
    List<GameObject> asteroid_amount = new List<GameObject>();
    List<GameObject> enemy_amount = new List<GameObject>();
    Vector2 player_position;
    Quaternion player_rotation;
    //int time_score = 0;
    int kills_needed = 6; //Kills needed to move on
    int minimum_enemies; //This number is a modifier for the minimum enemies in the level
    int enemies_spawned = 20; //amount of enemies that spawn
    int enemy_difficulty = 500; //this number may not change, it is the number used to decide what spawns
    int boss_likelihood = 2000; //this number will likely not change for same reasons
    int tier2_spawnrate = 0; //How likely enemies in tier 2 will spawn (tier 2 are just harder than the base enemies)
    int tier3_spawnrate = 0; //How likely enemies in tier 3 will spawn (should stay lower than tier 2)
    int boss_spawnrate = 0; //How likely the boss will spawn. This value should get reset after 
    bool boss_exist;
    bool boss_died;
    Player_Actions.Player_ControlsActions player_input;

    private void OnEnable()
    {
        Player_Actions player_actions = input_manager_script.input_actions;
        player_input = player_actions.Player_Controls;

        player_input.RadarButton.Enable();
        player_input.Pause.Enable();

        player_input.RadarButton.started += Toggle_Radar;
        player_input.Pause.started += Pause_Button;
    }

    private void OnDisable()
    {
        player_input.RadarButton.Disable();
        player_input.Pause.Disable();
    }

    public void Toggle_Radar(InputAction.CallbackContext value)
    {
        //Radar on and off
        if (username.activeInHierarchy == false && leaderboard.activeInHierarchy == false)
        { //Radar can be toggled, but not when leaderboard related stuff is up
            if (radar.activeInHierarchy == true)
            {
                radar.SetActive(false);
            }
            else if (radar.activeInHierarchy == false)
            {
                radar.SetActive(true);
            }
        }
    }

    public void Pause_Button(InputAction.CallbackContext value)
    {
        ui_manager.Pause_Game();
        player.GetComponent<player_ship_script>().engine.mute = true;
        score_text.gameObject.SetActive(false);
        lives_text.gameObject.SetActive(false);
        kills_text.gameObject.SetActive(false);
        level_text.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (next_level_timer.isFinished()) //Turn it off when at 0
        {
            next_level_text.SetActive(false);
        }
        else
        {
            next_level_timer.Countdown();
        }

        //Real quick make sure player's bullet damage isn't below 1
        if (player_bullet_damage < 1)
        {
            player_bullet_damage = 1;
        }

        if (player != null)
        {
            //As long as player exists, manager will log its position and rotation
            player_position = player.transform.position;
            player_rotation = player.transform.rotation;

            minimum_enemies = Mathf.RoundToInt(current_level / 25f); //Scaling math for the minimum amount of enemies spawned for each level
            if(minimum_enemies > 160)
            {
                minimum_enemies = 160;
            }

            //Checking Enemies
            if (enemy_amount.Count < 20 + minimum_enemies && !make_next_level && !boss_exist) //If the amount of enemies fall under this number, make a new one.
            {
                Spawn_Enemy();
            }

            //Initial part of making the next level
            if (kills_left <= 0 && !make_next_level && !boss_exist)
            {
                Clear_Level(); //Must destroy all enemies and asteroids before generating next level

                player.transform.position = new Vector3(0f, 0f, 0f); //move player back to the center
                player.GetComponent<player_ship_script>().immune_timer = 180;

                current_level += 1; //Display that it is the next level
                enemies_spawned += 10; //Increase the amount of enemies that will spawn
                tier2_spawnrate += 5; //Increase tier 2 spawn rate
                tier3_spawnrate += 2; //Increase tier 3 spawn rate
                kills_needed += 2; //Increase the amount of kills needed
                next_level_timer.Reset(); //Set the timer for the next level text
                next_level_text.SetActive(true); //Make the next level text active

                cam.transitioning = true;
                make_next_level = true;
            }
        }
        else //if the player is null
        {
            if (player_lives == 0 && !game_over)
            {
                username.SetActive(true);
                submit.SetActive(true);
                radar.SetActive(false);
                GameObject radar_camera = GameObject.FindGameObjectWithTag("radar");
                Destroy(radar_camera);
                game_over = true;
                //Also move the text out the way
                score_text.gameObject.SetActive(false);
                lives_text.gameObject.SetActive(false);
                kills_text.gameObject.SetActive(false);
                level_text.gameObject.SetActive(false);
                //Reset important text
                kills_left = 6;
                current_level = 1;
                //Reset important variables
                player_bullet_damage = 1;
                kills_needed = 6;
                enemies_spawned = 20;
                boss_spawnrate = 0;
                boss_likelihood = 2000;
                tier2_spawnrate = 0;
                tier3_spawnrate = 0;
            }
            else if (game_over == false)
            {
                respawn_timer.Countdown(); //If player still got lives, we gonna respawn
            }

            if (respawn_timer.isFinished())
            {
                player = Instantiate(player_object, player_position, player_rotation);
                player_bullet_damage -= 1.5f;
                player_lives--;
                respawn_timer.Reset();
            }
        }

        //Resetting the level
        if (leaderboard.activeSelf == true)
        {
            if (reset_countdown.isFinished())
            {
                //If that leaderboard is up and countdown made it to 0. Reset count and deactive leaderboard, but also kill all asteroids + enemies
                reset_countdown.Reset();
                leaderboard.SetActive(false);

                Clear_Level();

                main_menu.SetActive(true);
                ui_manager.event_system.SetSelectedGameObject(ui_manager.main_menu.transform.GetChild(1).gameObject);
                gameObject.SetActive(false);
            }
            else
            {
                reset_countdown.Countdown(); //this will countdown 
            }
        }


        //Setting scores.
        total_score = asteroid_score + enemy_score * 500;

        score_text.text = "SCORE:" + total_score;
        lives_text.text = "LIVES:" + player_lives;
        kills_text.text = "KILLS LEFT:" + kills_left;
        level_text.text = "LEVEL:" + current_level;

        if (boss_object == null)
        {
            boss_exist = false;
            boss_pointer.SetActive(false);
        }
        else
        {
            kills_text.text = "BOSS";
            boss_pointer.SetActive(true);
            boss_pointer.transform.position = player_position;
            Vector3 dir = boss_object.transform.position - boss_pointer.transform.position;
            boss_pointer.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, Vector3.forward);
            //boss_pointer.transform.eulerAngles = new Vector3(boss_pointer.transform.eulerAngles.x, 0, 0);
        }

        if (boss_died)
        {
            boss_spawnrate = 0;
            boss_likelihood += 2000;
            boss_died = false;
        }

        if (make_next_level == true)
        {
            kills_left = kills_needed;
            Generate_Level();

            make_next_level = false;
        }
    }

    public void Add_To_Asteroid_List(GameObject asteroid)
    {
        asteroid_amount.Add(asteroid);
    }

    public void Add_To_Enemy_List(GameObject enemy)
    {
        enemy_amount.Add(enemy);
    }

    public void Spawn_Asteroid(Vector3 position, float scale)
    {
        int asteroid_index = Random.Range(0, 5);
        GameObject the_asteroid = Instantiate(asteroids[asteroid_index], position, Quaternion.identity);
        the_asteroid.transform.localScale *= scale;
        the_asteroid.transform.GetChild(0).transform.localScale /= scale;
        the_asteroid.GetComponent<asteroid_script>().is_baby_asteroid = true;
        Add_To_Asteroid_List(the_asteroid);
    }

    public void Add_Score(int score_amount)
    {
        enemy_score += score_amount;
    }

    public void Add_Ascore(int score_amount)
    {
        asteroid_score += score_amount;
    }

    public void Enemy_Death(bool is_boss)
    {
        if (!boss_exist)
        {
            int temp_number = Random.Range(0, boss_likelihood);
            if (temp_number < boss_spawnrate) //spawn boss
            {
                //spawn the boss and also add it to the enemy list
                int safety_net = 0;
                Vector2 boss_position = new Vector2(Random.Range(-160, 160), Random.Range(-130, 130)); //make the random enemy position
                Quaternion boss_rotation = new Quaternion();
                boss_rotation.eulerAngles = new Vector3(0.0f, 0.0f, Random.Range(0.0f, 360f)); //set the rotation

                if (player != null)
                {
                    while (Vector2.Distance(player_object.transform.position, boss_position) < 50) //so if the position is too close to player
                    {
                        boss_position = new Vector2(Random.Range(-160, 160), Random.Range(-130, 130)); //choose a different position

                        if (Vector2.Distance(player.transform.position, boss_position) >= 50) //it will keep doing it until this is true
                        {
                            break;
                        }

                        safety_net++;
                        if (safety_net == 10) //The safety net is here to stop the while loop from going too far
                        {
                            boss_position = new Vector2(160, 130); //If it tries 10 times to change position, the position will default to here
                            break;
                        }
                    }
                }

                boss_object = Instantiate(bosses[0], boss_position, boss_rotation);
                Add_To_Enemy_List(boss_object);
                boss_health = boss_object.GetComponent<Base_Enemy_Script>().health;
                boss_exist = true;
                boss_spawnrate = 0;
            }
            else //raise likelihood of boss spawning otherwise
            {
                boss_spawnrate++;
            }
        }
        else
        {
            if (is_boss)
            {
                boss_died = true;
            }
        }

        kills_left--;
    }

    public void Add_Damage()
    {
        player_bullet_damage += .1f;
    }

    //This is to make the levels, adjustments may need to be made to produce future levels
    public void Generate_Level()
    {
        for (int i = 0; i < 100; i++)
        {
            int asteroid_index = Random.Range(0, 5);
            float asteroid_scale = Random.Range(.6f, 1.5f);
            Vector2 asteroid_position = new Vector2(Random.Range(-level_bounds.x, level_bounds.x), Random.Range(-level_bounds.y, level_bounds.y));
            GameObject the_asteroid = Instantiate(asteroids[asteroid_index], asteroid_position, Quaternion.identity);
            the_asteroid.transform.localScale *= asteroid_scale;
            the_asteroid.transform.GetChild(0).transform.localScale /= asteroid_scale;
            Add_To_Asteroid_List(the_asteroid);
        }

        if (enemies_spawned >= 220)
        {
            enemies_spawned = 220; //Make sure there isn't too many enemies that spawn
        }

        for (int i = 0; i < enemies_spawned; i++)
        {
            Spawn_Enemy();
        }
    }

    public void Quit_To_Main()
    {
        radar.SetActive(false); //Radar should not be on in main menu
        GameObject radar_camera = GameObject.FindGameObjectWithTag("radar");
        Destroy(radar_camera); //Remove radar camera as well
        game_over = true; //That game state is now "game over"
        //Also move the text out the way
        score_text.gameObject.SetActive(false);
        lives_text.gameObject.SetActive(false);
        kills_text.gameObject.SetActive(false);
        level_text.gameObject.SetActive(false);
        //Reset important text
        kills_left = 6;
        current_level = 1;
        //Reset important variables
        player_bullet_damage = 1;
        kills_needed = 6;
        enemies_spawned = 20;
        boss_spawnrate = 0;
        boss_likelihood = 2000;
        tier2_spawnrate = 0;
        tier3_spawnrate = 0;

        reset_countdown.Reset();
        //Deactivate leaderboard
        leaderboard.SetActive(false);
        //Clear all level data
        Clear_Level();
        //Remove player as well
        Destroy(player);
        //Start the menu
        main_menu.SetActive(true);
        gameObject.SetActive(false); //deactivate self
    }

    void Spawn_Enemy()
    {
        int safety_net = 0;
        Vector2 enemy_position = new Vector2(Random.Range(-level_bounds.x, level_bounds.x), Random.Range(-level_bounds.y, level_bounds.y)); //make the random enemy position
        Quaternion enemy_rotation = new Quaternion();
        enemy_rotation.eulerAngles = new Vector3(0.0f, 0.0f, Random.Range(0.0f, 360f)); //set the rotation

        while (Vector2.Distance(player_object.transform.position, enemy_position) < 50) //so if the position is too close to player
        {
            enemy_position = new Vector2(Random.Range(-178, 178), Random.Range(-151, 151)); //choose a different position

            if (Vector2.Distance(player.transform.position, enemy_position) >= 50) //it will keep doing it until this is true
            {
                break;
            }

            safety_net++;
            if (safety_net == 10) //The safety net is here to stop the while loop from going too far
            {
                enemy_position = new Vector2(180, 160); //If it tries 10 times to change position, the position will default to here
                break;
            }
        }

        int enemy_index = Random.Range(0, enemy_difficulty);
        if (enemy_index < tier3_spawnrate)
        {
            GameObject the_enemy;
            the_enemy = Instantiate(enemies[Choice.Choose(3, 4)], enemy_position, enemy_rotation); //spawn tier 3 enemies
            Add_To_Enemy_List(the_enemy);
        }
        else if (enemy_index < tier2_spawnrate)
        {
            GameObject the_enemy;
            the_enemy = Instantiate(enemies[Choice.Choose(1, 2)], enemy_position, enemy_rotation); //spawn tier 2 enemies
            Add_To_Enemy_List(the_enemy);
        }
        else
        {
            GameObject the_enemy;
            the_enemy = Instantiate(enemies[0], enemy_position, enemy_rotation); //spawn worker
            Add_To_Enemy_List(the_enemy);
        }
    }

    void Clear_Level()
    {
        for (int i = 0; i < asteroid_amount.Count; i++) 
        { //Destroy all asteroids
            Destroy(asteroid_amount[i]);
        }
        asteroid_amount.Clear(); //Clear list 

        for (int i2 = 0; i2 < enemy_amount.Count; i2++)
        { //Destroy all enemies
            Destroy(enemy_amount[i2]); //Clear list
        }
        enemy_amount.Clear();

        GameObject[] temp_canvas_array = GameObject.FindGameObjectsWithTag("enemycanvas");
        for (int i3 = 0; i3 < temp_canvas_array.Length; i3++) //Remove enemy canvases
        {
            Destroy(temp_canvas_array[i3]);
        }

        GameObject[] temp_mineral_array = FindGameObjectsWithLayer(8);
        if (temp_mineral_array != null)
        {
            for (int i4 = 0; i4 < temp_mineral_array.Length; i4++) //Remove minerals
            {
                Destroy(temp_mineral_array[i4]);
            }
        }
    }

    //Using this specifically for all items No other object is going to need to do this except for the manager
    private GameObject[] FindGameObjectsWithLayer(int layer_mask)
    {
        GameObject[] obj_array = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        List<GameObject> obj_list = new List<GameObject>();
        
        for (int i = 0; i < obj_array.Length; i++)
        {
            if (obj_array[i].layer == layer_mask)
            {
                obj_list.Add(obj_array[i]);
            }
        }

        if (obj_list.Count == 0)
        {
            return null;
        }

        return obj_list.ToArray();
    }

    public void Drop_Player_Powerups()
    {
        List<GameObject> player_powerups = new List<GameObject>();
        int average_stats = (p_speed + p_acceleration + p_firerate + p_bulletlife + p_bulletspeed /*+ p_extrabullet + p_handling + p_magnetrange + p_shipsize + p_bulletsize*/) / 5;

        p_speed = New_Powerup_Stat(p_speed, average_stats, LoadPrefab.speed_powerup);
        p_acceleration = New_Powerup_Stat(p_speed, average_stats, LoadPrefab.acceleration_powerup);
        p_firerate = New_Powerup_Stat(p_speed, average_stats, LoadPrefab.firerate_powerup);
        p_bulletlife = New_Powerup_Stat(p_speed, average_stats, LoadPrefab.bulletlife_powerup);
        p_bulletspeed = New_Powerup_Stat(p_speed, average_stats, LoadPrefab.bulletspeed_powerup);
        /*p_extrabullet = New_Powerup_Stat(p_speed, average_stats, Powerup.extrabullet_powerup);
        p_handling = New_Powerup_Stat(p_speed, average_stats, Powerup.handling_powerup);
        p_magnetrange = New_Powerup_Stat(p_speed, average_stats, Powerup.magnetrange_powerup);
        p_shipsize = New_Powerup_Stat(p_speed, average_stats, Powerup.shipsize_powerup);
        p_bulletsize = New_Powerup_Stat(p_speed, average_stats, Powerup.bulletsize_powerup);*/


    }

    //This will just be used by the above function, it also spawns a powerup owned
    private int New_Powerup_Stat(int original_stat, int average_stat, GameObject the_powerup)
    {
        if (original_stat != 0 && average_stat != 0)
        {
            int new_stat = original_stat / average_stat;
            for (int i = 0; i < original_stat - new_stat; i++)
            {
                Instantiate(the_powerup, player.transform.position, Quaternion.identity);
            }
            return new_stat;
        }
        return original_stat;
    }
}
