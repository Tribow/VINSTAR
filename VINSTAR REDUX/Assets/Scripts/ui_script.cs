using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ui_script : MonoBehaviour
{
    [Header("Public Data")]
    public GameObject manager;
    public GameObject main_menu;
    public GameObject options_menu;
    public GameObject lmao_text;
    public GameObject rebinds_menu;
    public Button reset_all_button_1;
    public Button reset_all_button_2;
    public GameObject pause_menu;
    public GameObject player_object;
    public GameObject radar;
    public GameObject radar_camera;
    public GameObject instruction_text;
    public GameObject boss_health;
    public EventSystem event_system;


    private manager_script mango;
    private bool controls = false;
    private Vector3 options_pos;
    private Vector3 rebinds_pos;
    private float option_t;
    private GameObject controller_toggle;
    private GameObject keyboard_toggle;
    private Slider boss_health_red;
    private Slider boss_health_white;

    public static bool game_is_paused = false;

    public void Start()
    {
        options_pos = options_menu.transform.position;
        #if (UNITY_WEBGL)
            rebinds_menu.transform.position += new Vector3(2000f, 0f, 0f);
        #endif
        rebinds_pos = rebinds_menu.transform.position;
        controller_toggle = rebinds_menu.transform.GetChild(2).gameObject;
        keyboard_toggle = rebinds_menu.transform.GetChild(3).gameObject;
        reset_all_button_1.onClick.AddListener(() => input_manager_script.Reset_All_Bindings());
        reset_all_button_2.onClick.AddListener(() => input_manager_script.Reset_All_Bindings());
        mango = manager.GetComponent<manager_script>();
        boss_health_red = boss_health.transform.GetChild(0).gameObject.GetComponent<Slider>();
        boss_health_white = boss_health.transform.GetChild(1).gameObject.GetComponent<Slider>();
        //PlayerPrefs.DeleteAll();
    }

    public void Update()
    {
        if (controls) //Whenever this bool is true, move the entire options menu to the side and show the input rebind menu
        {
            options_menu.transform.position = new Vector3(Mathf.Lerp(options_pos.x, options_pos.x - Screen.width, option_t), options_pos.y, options_pos.z);
            rebinds_menu.transform.position = new Vector3(Mathf.Lerp(rebinds_pos.x, rebinds_pos.x - Screen.width, option_t), rebinds_pos.y, rebinds_pos.z);
            if (option_t < 1.0f)
            {
                option_t += .8f * Time.fixedUnscaledDeltaTime;
            }
        }
        else //Move it back whenever the opposite is the case. This should effectively do nothing when it's already in the right spot
        {
            options_menu.transform.position = new Vector3(Mathf.Lerp(options_menu.transform.position.x, options_pos.x, option_t), options_pos.y, options_pos.z);
            rebinds_menu.transform.position = new Vector3(Mathf.Lerp(rebinds_menu.transform.position.x, rebinds_pos.x, option_t), rebinds_pos.y, rebinds_pos.z);
            if (option_t < 1.0f)
            {
                option_t += .8f * Time.fixedUnscaledDeltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if (mango.boss_object != null)
        {
            boss_health.SetActive(true);
            boss_health_white.maxValue = mango.boss_health;
            boss_health_red.maxValue = mango.boss_health;

            boss_health_white.value = mango.boss_object.GetComponent<Base_Enemy_Script>().health;
            boss_health_red.value = Mathf.Lerp(boss_health_red.value, boss_health_white.value, .08f);
        }
        else
            boss_health.SetActive(false);

    }

    public void Start_Game()
    {
        manager_script mango = manager.GetComponent<manager_script>();

        main_menu.SetActive(false);
        radar.SetActive(true);
        mango.player = Instantiate(player_object);
        manager.SetActive(true);
        Instantiate(radar_camera);
        instruction_text.SetActive(true);

        for (int i = 0; i < 100; i++)
        {
            int index = Random.Range(0, 5);
            float asteroid_scale = Random.Range(.6f, 1.5f);
            Vector2 asteroid_position = new Vector2(Random.Range(-178f, 178f), Random.Range(-150f, 150f));
            GameObject the_asteroid = Instantiate(mango.asteroids[index], asteroid_position, Quaternion.identity);
            the_asteroid.transform.localScale *= asteroid_scale;
            the_asteroid.transform.GetChild(0).transform.localScale /= asteroid_scale;
            mango.Add_To_Asteroid_List(the_asteroid);
        }

        for (int i = 0; i < 20; i++)
        {
            GameObject the_enemy;
            Vector2 enemy_position = new Vector2(Random.Range(-178, 178), Random.Range(-150, 150)); //make the random enemy position
            Quaternion enemy_rotation = new Quaternion();
            enemy_rotation.eulerAngles = new Vector3(0.0f, 0.0f, Random.Range(0.0f, 360f));
            the_enemy = Instantiate(mango.enemies[0], enemy_position, enemy_rotation);
            mango.Add_To_Enemy_List(the_enemy);
        }

        mango.game_over = false;
        mango.player_lives = 3;
        mango.enemy_score = 0;
        mango.asteroid_score = 0;
        mango.total_score = 0;
        mango.score_text.gameObject.SetActive(true);
        mango.lives_text.gameObject.SetActive(true);
        mango.kills_text.gameObject.SetActive(true);
        mango.level_text.gameObject.SetActive(true);
    }

    public void Quit_Game()
    {
    #if (UNITY_STANDALONE)
        Application.Quit();
    #elif (UNITY_WEBGL)
        lmao_text.SetActive(true);
    #endif
    }

    public void Options_Menu()
    {
       if(game_is_paused)
       {
            pause_menu.SetActive(false);
            options_menu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(options_menu.transform.GetChild(2).gameObject);
        }
       else
       {
            main_menu.SetActive(false);
            options_menu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(options_menu.transform.GetChild(2).gameObject);
       }
    }

    public void Controls_Menu()
    {
        controls = true;
        option_t = 0;
        //Go to the controls/rebinds menu and make sure the eventsystem selects an active object
        if (!rebinds_menu.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(rebinds_menu.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(rebinds_menu.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject);
        }
    }

    public void Rebind_Back_Button()
    {
        controls = false;
        option_t = 0;
        EventSystem.current.SetSelectedGameObject(options_menu.transform.GetChild(2).gameObject);
    }

    public void Switch_Rebinds_Button()
    {
        //If the keyboard inputs are not enabled
        if(!rebinds_menu.transform.GetChild(0).gameObject.activeSelf)
        {
            rebinds_menu.transform.GetChild(0).gameObject.SetActive(true);
            rebinds_menu.transform.GetChild(1).gameObject.SetActive(false);
            controller_toggle.SetActive(true);
            keyboard_toggle.SetActive(false);
            EventSystem.current.SetSelectedGameObject(controller_toggle);
        }
        else if (!rebinds_menu.transform.GetChild(1).gameObject.activeSelf) //If controller inputs are not enabled
        {
            rebinds_menu.transform.GetChild(1).gameObject.SetActive(true);
            rebinds_menu.transform.GetChild(0).gameObject.SetActive(false);
            keyboard_toggle.SetActive(true);
            controller_toggle.SetActive(false);
            EventSystem.current.SetSelectedGameObject(keyboard_toggle);
        }
        
    }

    public void Back_Button()
    {
        if (game_is_paused)
        {
            options_menu.SetActive(false);
            pause_menu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(pause_menu.transform.GetChild(2).gameObject);
        }
        else
        {
            options_menu.SetActive(false);
            main_menu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(main_menu.transform.GetChild(2).gameObject);

        }
    }

    public void Main_Menu()
    {
        manager_script mango = manager.GetComponent<manager_script>();

        Resume_Game();
        mango.Quit_To_Main();

        EventSystem.current.SetSelectedGameObject(main_menu.transform.GetChild(1).gameObject);
    }

    public void Resume_Game()
    {
        manager_script mango = manager.GetComponent<manager_script>();
        if (game_is_paused)
        {
            pause_menu.SetActive(false);
            mango.score_text.gameObject.SetActive(true);
            mango.lives_text.gameObject.SetActive(true);
            mango.kills_text.gameObject.SetActive(true);
            mango.level_text.gameObject.SetActive(true);
            Time.timeScale = 1;
            game_is_paused = false;
        }
    }

    public void Pause_Game()
    {
        if(!game_is_paused)
        {
            //print("hello??");
            pause_menu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(pause_menu.transform.GetChild(1).gameObject);
            Time.timeScale = 0;
            game_is_paused = true;
        }
    }

    /*public void Navigate(MoveDirection move_direction)
    {
        AxisEventData data = new AxisEventData(EventSystem.current);

        data.moveDir = move_direction;

        data.selectedObject = EventSystem.current.currentSelectedGameObject;

        ExecuteEvents.Execute(data.selectedObject, data, ExecuteEvents.moveHandler);
    }*/
}
