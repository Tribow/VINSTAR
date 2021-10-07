using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class instruction_script : MonoBehaviour
{
    public Text movement_text;
    public Text slow_text;
    public Text magnet_text;
    public GameObject manager_object;
    public GameObject main_menu;
    public GameObject parent_tutorial_object;
    public GameObject username_input;
    public GameObject leaderboard;

    private Player_Actions.Player_ControlsActions player_input;
    private manager_script manager;

    private void OnEnable()
    {
        Player_Actions player_actions = input_manager_script.input_actions;
        player_input = player_actions.Player_Controls;
        manager = manager_object.GetComponent<manager_script>();
    }

    private void Update()
    {
        if (ui_script.game_is_paused)
        {
            parent_tutorial_object.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (manager.current_level == 1 && !main_menu.activeInHierarchy)
        {
            if (username_input.activeInHierarchy || leaderboard.activeInHierarchy)
            {
                parent_tutorial_object.SetActive(false);
            }
            else
            {
                parent_tutorial_object.SetActive(true);
                movement_text.text = "MOVE WITH " + player_input.Acceleration.GetBindingDisplayString(0) + "," + player_input.TurnLeft.GetBindingDisplayString(0) + ","
                + player_input.Deceleration.GetBindingDisplayString(0) + "," + player_input.TurnRight.GetBindingDisplayString(0) + " OR "
                + player_input.Acceleration.GetBindingDisplayString(1) + "," + player_input.TurnLeft.GetBindingDisplayString(1) + "," + player_input.Deceleration.GetBindingDisplayString(1)
                + "," + player_input.TurnRight.GetBindingDisplayString(1) + "\n\nSHOOT WITH " + player_input.Shoot.GetBindingDisplayString(0) + " OR " + player_input.Shoot.GetBindingDisplayString(1);
            }
        }
        else if (manager.current_level == 2)
        {
            if (username_input.activeInHierarchy)
            {
                slow_text.gameObject.SetActive(false);
            }
            else
            {
                parent_tutorial_object.SetActive(false);
                slow_text.gameObject.SetActive(true);
                slow_text.text = "SLOW DOWN WITH " + player_input.Slow.GetBindingDisplayString(0) + " OR " + player_input.Slow.GetBindingDisplayString(1) + ". " +
                    "\n\nUSE THIS FOR MORE PRECISE MOVEMENT";
            }
        }
        else if (manager.current_level == 3)
        {
            if(username_input.activeInHierarchy)
            {
                magnet_text.gameObject.SetActive(false);
            }
            else
            {
                slow_text.gameObject.SetActive(false);
                magnet_text.gameObject.SetActive(true);
                magnet_text.text = "A MAGNET ACTIVATES 1.5 SECONDS AFTER YOU STOP SHOOTING. \n\nONCE ACTIVATED, THE SHIP WILL PULL IN MINERALS IF THEY'RE WITHIN RANGE";
            }
        }
        else
        {
            parent_tutorial_object.SetActive(false);
            slow_text.gameObject.SetActive(false);
            magnet_text.gameObject.SetActive(false);
        }
    }
}
