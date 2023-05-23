using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class Main_panel : MonoBehaviour
{
    Main_menu_Script main_Menu_Script;
    Button button;
    TextMeshPro online_text;
    TextMeshPro welcome_text;
    Vector3 text_pos;
    bool started = false;
    Languages langs;
    void Start()
    {
        Set_variables();
        text_pos = new Vector3(-1230,81,0);
        Set_language();
        started = true;
    }

    private void Set_variables()
    {
        main_Menu_Script = GameObject.Find("Main_menu_canvas").GetComponent<Main_menu_Script>();
        button = GameObject.Find("Main_menu_online").GetComponent<Button>();
        online_text = GameObject.Find("Main_menu_online_text").GetComponent<TextMeshPro>();
        welcome_text = GameObject.Find("Main_menu_welcome").GetComponent<TextMeshPro>();
        langs = GameObject.Find("Language_holder").GetComponent<Languages>();
    }

    private void Set_language()
    {
        TextMeshPro play_text = GameObject.Find("Main_menu_play_text").GetComponent<TextMeshPro>();
        play_text.text = langs.Return_language_string("Play_game");
        TextMeshPro options_text = GameObject.Find("Main_menu_options_text").GetComponent<TextMeshPro>();
        options_text.text = langs.Return_language_string("Options");
        TextMeshPro exit_text = GameObject.Find("Main_menu_exit_text").GetComponent<TextMeshPro>();
        exit_text.text = langs.Return_language_string("Exit");
    }

    private void OnEnable()
    {
        if (started)
        {
            button.onClick.RemoveAllListeners();
            if (!main_Menu_Script.online)
            {
                online_text.text = $"{langs.Return_language_string("Sign_in")}";
                button.onClick.AddListener(Sign_in);
                welcome_text.text = "";
            }
            else
            {
                online_text.text = $"{langs.Return_language_string("Sign_out")}";
                button.onClick.AddListener(Sign_out);
                welcome_text.text = $"{langs.Return_language_string("Welcome")}, {main_Menu_Script.user.Username}!";
            }
            welcome_text.transform.localPosition = text_pos;
        }
    }
    void Sign_in()
    {
        Debug.Log("Sign in");
        StartCoroutine(main_Menu_Script.Panel_switch(this.gameObject, main_Menu_Script.panels["Login_menu"]));
    }
    void Sign_out()
    {
        Debug.Log("Sign out");
        main_Menu_Script.user = null;
        main_Menu_Script.online = false;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(Sign_in);
        online_text.text = $"{langs.Return_language_string("Sign_in")}";
        welcome_text.text = "";
        Options options = GameObject.Find("Options_panel").GetComponent<Options>();
        options.Set_save(false);
        StartCoroutine(main_Menu_Script.Panel_switch(welcome_text.gameObject));
    }
}
