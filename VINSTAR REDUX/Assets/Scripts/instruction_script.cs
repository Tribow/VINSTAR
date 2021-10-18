using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class instruction_script : MonoBehaviour
{
    [Header("Requirements")]
    public Text movement_text;
    public Text slow_text;
    public Text magnet_text;
    public GameObject manager_object;
    public GameObject parent_tutorial_object;
    public GameObject username_input;
    public GameObject leaderboard;

    private Player_Actions.Player_ControlsActions player_input;
    private manager_script manager;
    private InputDevice current_device;

    private void Awake()
    {
        current_device = InputSystem.devices[0];
    }

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

        InputSystem.onDeviceChange += (device, change) =>
        { //updating the current device whenever it is relevant
            switch (change)
            {
                case InputDeviceChange.Added:
                    current_device = device;
                    break;
                case InputDeviceChange.ConfigurationChanged:
                    current_device = device;
                    break;
                case InputDeviceChange.Enabled:
                    current_device = device;
                    break;
                case InputDeviceChange.Reconnected:
                    current_device = device;
                    break;
            }
        };
        
    }

    private void FixedUpdate()
    {
        if (manager.current_level == 1 && manager_object.activeInHierarchy)
        {
            if (username_input.activeInHierarchy || leaderboard.activeInHierarchy)
            { //Check for leaderboard as well on level 1 since by default current_level is always 1
                parent_tutorial_object.SetActive(false);
            }
            else
            {
                parent_tutorial_object.SetActive(true);
                //On level 1 display this text for the tutorial
                if (current_device is Gamepad)
                {
                    movement_text.text = "MOVE WITH " + player_input.Acceleration.GetBindingDisplayString(2) + "," + player_input.TurnLeft.GetBindingDisplayString(2) + ","
                    + player_input.Deceleration.GetBindingDisplayString(2) + "," + player_input.TurnRight.GetBindingDisplayString(2) + " OR "
                    + player_input.Acceleration.GetBindingDisplayString(3) + "," + player_input.TurnLeft.GetBindingDisplayString(3) + "," + player_input.Deceleration.GetBindingDisplayString(3)
                    + "," + player_input.TurnRight.GetBindingDisplayString(3) + "\n\nSHOOT WITH " + player_input.Shoot.GetBindingDisplayString(2) + " OR " + player_input.Shoot.GetBindingDisplayString(3);
                }
                else
                {
                    movement_text.text = "MOVE WITH " + player_input.Acceleration.GetBindingDisplayString(0) + "," + player_input.TurnLeft.GetBindingDisplayString(0) + ","
                    + player_input.Deceleration.GetBindingDisplayString(0) + "," + player_input.TurnRight.GetBindingDisplayString(0) + " OR "
                    + player_input.Acceleration.GetBindingDisplayString(1) + "," + player_input.TurnLeft.GetBindingDisplayString(1) + "," + player_input.Deceleration.GetBindingDisplayString(1)
                    + "," + player_input.TurnRight.GetBindingDisplayString(1) + "\n\nSHOOT WITH " + player_input.Shoot.GetBindingDisplayString(0) + " OR " + player_input.Shoot.GetBindingDisplayString(1);
                }
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
                //On Level 2 display this text for the tutorial
                if (current_device is Gamepad)
                {
                    slow_text.text = "SLOW DOWN WITH " + player_input.Slow.GetBindingDisplayString(2) + " OR " + player_input.Slow.GetBindingDisplayString(3) + ". " +
                        "\n\nUSE THIS FOR MORE PRECISE MOVEMENT";
                }
                else
                {
                    slow_text.text = "SLOW DOWN WITH " + player_input.Slow.GetBindingDisplayString(0) + " OR " + player_input.Slow.GetBindingDisplayString(1) + ". " +
                        "\n\nUSE THIS FOR MORE PRECISE MOVEMENT";
                }
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
                //On Level 3, display this text
                magnet_text.text = "A MAGNET ACTIVATES 1.5 SECONDS AFTER YOU STOP SHOOTING. \n\nONCE ACTIVATED, THE SHIP WILL PULL IN MINERALS IF THEY'RE WITHIN RANGE";
            }
        }
        else
        {
            //None of these should be active if the current level is not relevant
            parent_tutorial_object.SetActive(false);
            slow_text.gameObject.SetActive(false);
            magnet_text.gameObject.SetActive(false);
        }
    }
}
