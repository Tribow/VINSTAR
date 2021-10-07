using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class input_manager_script : MonoBehaviour
{
    public static Player_Actions input_actions;

    public static event Action Rebind_Complete;
    public static event Action Rebind_Canceled;
    public static event Action<InputAction, int> Rebind_Started;

    public void Awake()
    {
        //Safety check
        if (input_actions == null)
            input_actions = new Player_Actions();
    }

    public static void Start_Rebind(string action_name, int binding_index, Text status_text, bool exclude_mouse)
    {
        InputAction action = input_actions.asset.FindAction(action_name);
        if (action == null || action.bindings.Count <= binding_index) //SAFETY CHECK
        {
            Debug.Log("Couldn't find action or binding bro");
            return;
        }

        //If it's composite it will tell the rebind function to call itself recursively until it gets through the composite bindings
        if (action.bindings[binding_index].isComposite)
        {
            var first_part_index = binding_index = 1;
            if (first_part_index < action.bindings.Count && action.bindings[first_part_index].isComposite)
                Do_Rebind(action, binding_index, status_text, true, exclude_mouse);
        }
        else
            Do_Rebind(action, binding_index, status_text, false, exclude_mouse);
    }

    private static void Do_Rebind(InputAction action_to_rebind, int binding_index, Text status_text, bool all_composite_parts, bool exclude_mouse)
    {
        if (action_to_rebind == null || binding_index < 0) //SAFETY CHECK
            return;

        status_text.text = $"Waiting for input...";

        action_to_rebind.Disable(); //Do not allow the binding to be used at this time

        var rebind = action_to_rebind.PerformInteractiveRebinding(binding_index);

        //Re-enable the action to rebind once it is done.
        rebind.OnComplete(operation =>
        {
            action_to_rebind.Enable();
            operation.Dispose();

            if(all_composite_parts)
            {
                //Recursive call if it's composite, this will check if there is another part of the composite binding
                var next_binding_index = binding_index + 1;
                if (next_binding_index < action_to_rebind.bindings.Count && action_to_rebind.bindings[binding_index].isComposite)
                    Do_Rebind(action_to_rebind, next_binding_index, status_text, all_composite_parts, exclude_mouse);
            }

            Save_Binding_Override(action_to_rebind); //Be sure to save it
            Rebind_Complete?.Invoke();
        });
        
        rebind.OnCancel(operation =>
        {
            //Canceling will not save anything of course
            action_to_rebind.Enable();
            operation.Dispose();

            Rebind_Canceled?.Invoke();
        });

        //If this specific input gets pressed it will cancel (Not sure what to put here yet)
        //rebind.WithCancelingThrough("<Keyboard>/escape");

        if (exclude_mouse)
            rebind.WithControlsExcluding("Mouse");


        Rebind_Started?.Invoke(action_to_rebind, binding_index);
        rebind.Start(); //Starts the rebind process
    }

    public static string GetBindingName(string action_name, int binding_index)
    {
        if (input_actions == null) //SAFETY CHECK!!!
            input_actions = new Player_Actions();

        InputAction action = input_actions.asset.FindAction(action_name);
        return action.GetBindingDisplayString(binding_index);
    }


    private static void Save_Binding_Override(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }

    public static void Load_Binding_Override(string action_name)
    {
        if (input_actions == null) //SAFETY CHECK!!!!!!!!
            input_actions = new Player_Actions();

        InputAction action = input_actions.asset.FindAction(action_name);

        //Loop through the bindings and save each one to a string
        for(int i = 0; i < action.bindings.Count; i++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
                action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
        }
    }

    public static void Reset_Binding(string action_name, int binding_index)
    {
        InputAction action = input_actions.asset.FindAction(action_name);

        if (action == null || action.bindings.Count <= binding_index) //SAFETY GOD DAMN CHECK!!!!!!!!!!!!!!!
        {
            Debug.Log("Could not find action or binding bro");
            return;
        }

        //Loop through composite if it is on
        if (action.bindings[binding_index].isComposite)
        {
            for (int i = binding_index; i < action.bindings.Count && action.bindings[i].isComposite; i++)
                action.RemoveBindingOverride(i);
        }
        else
            action.RemoveBindingOverride(binding_index);

        Save_Binding_Override(action); //Save this for later
    }

    public static void Reset_All_Bindings()
    {
        if (input_actions == null) //SAFETY CHECK!!!!!!!!! AAAAA!!!!!!!!! SAFETY CHECK OF THE GODS!!!!!!!!
            input_actions = new Player_Actions();

        foreach (InputActionMap map in input_actions.asset.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }

        foreach (InputAction action in input_actions.asset.FindActionMap("Player_Controls", true).actions)
            Save_Binding_Override(action);

        Rebind_Complete?.Invoke();
    }
}
