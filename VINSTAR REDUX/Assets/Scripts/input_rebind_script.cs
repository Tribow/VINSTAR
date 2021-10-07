using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class input_rebind_script : MonoBehaviour
{

    [SerializeField]
    private InputActionReference input_action_reference;
    [SerializeField]
    private bool exclude_mouse = true;
    [SerializeField]
    private int selected_binding;
    [SerializeField]
    private InputBinding.DisplayStringOptions display_string_options;
    [Header("Binding info - DO NOT EDIT")]
    [SerializeField]
    private InputBinding input_binding;
    private int binding_index;

    private string action_name;

    [Header("UI Fields")]
    [SerializeField]
    private Button rebind_button;
    [SerializeField]
    private Text rebind_text;
    [SerializeField]
    private Button reset_button;

    private void Awake()
    {
        if (input_action_reference != null)
        {
            Get_Binding_Info();
            Update_UI();

            input_manager_script.Load_Binding_Override(action_name);
        }
    }

    private void OnEnable()
    {
        rebind_button.onClick.AddListener(() => Do_Rebind());
        reset_button.onClick.AddListener(() => Reset_Binding());

        if (input_action_reference != null)
        {
            Get_Binding_Info();
            Update_UI();
        }

        //UpdateUI will be invoked when rebinding gets completed or canceled
        input_manager_script.Rebind_Complete += Update_UI;
        input_manager_script.Rebind_Canceled += Update_UI;
    }

    private void OnDisable()
    {
        input_manager_script.Rebind_Complete -= Update_UI;
        input_manager_script.Rebind_Canceled -= Update_UI;
    }

    private void OnValidate()
    {
        //in case it wasn't set to anything.
        if (input_action_reference == null)
            return;

        Get_Binding_Info();
        Update_UI();
    }

    private void Get_Binding_Info()
    {
        if (input_action_reference.action != null)
            action_name = input_action_reference.name;

        if(input_action_reference.action.bindings.Count > selected_binding)
        {
            input_binding = input_action_reference.action.bindings[selected_binding];
            binding_index = selected_binding;
        }
    }

    private void Update_UI()
    {
        if (rebind_text != null)
            rebind_text.text = action_name;

        if(rebind_text != null)
        {
            if (Application.isPlaying)
            {
                rebind_text.text = input_manager_script.GetBindingName(action_name, binding_index);
            }
            else
                rebind_text.text = input_action_reference.action.GetBindingDisplayString(binding_index);
                    
        }
    }

    public void Do_Rebind()
    {
        input_manager_script.Start_Rebind(action_name, binding_index, rebind_text, exclude_mouse);
    }

    private void Reset_Binding()
    {
        input_manager_script.Reset_Binding(action_name, binding_index);
        Update_UI();
    }

    public void Test()
    {
        gameObject.transform.position += new Vector3(5f, 0, 0);
    }
}

