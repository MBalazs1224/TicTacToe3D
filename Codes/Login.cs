using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class Login : MonoBehaviour
{
    InputField username;
    InputField password;
    Button submit;
    TextMesh login_feedback;
    Main_menu_Script main_Menu_Script;
    bool started = false;
    TextMeshPro username_feedback;
    TextMeshPro password_feedback;
    Audio_manager audio_manager;
    Languages langs;
    void Start()
    {
        Set_variables();
        Set_language();
        username.Select();
        started = true;
    }
    private void Set_variables()
    {
        username = GameObject.Find("Log_input_username").GetComponent<InputField>();
        password = GameObject.Find("Log_input_password").GetComponent<InputField>();
        submit = GameObject.Find("Log_submit").GetComponent<Button>();
        login_feedback = GameObject.Find("Log_feedback").GetComponent<TextMesh>();
        username_feedback = GameObject.Find("Login_username_feedback").GetComponent<TextMeshPro>();
        password_feedback = GameObject.Find("Login_password_feedback").GetComponent<TextMeshPro>();
        main_Menu_Script = GameObject.Find("Main_menu_canvas").GetComponent<Main_menu_Script>();
        submit.interactable = false;
        langs = GameObject.Find("Language_holder").GetComponent<Languages>();
        audio_manager = Camera.main.GetComponent<Audio_manager>();
    }

    private void Set_language()
    {
        TextMeshPro submit_text = GameObject.Find("Log_submit_text").GetComponent<TextMeshPro>();
        submit_text.text = langs.Return_language_string("Submit");
        TextMeshPro register_text = GameObject.Find("Log_register_text").GetComponent<TextMeshPro>();
        register_text.text = langs.Return_language_string("Register");
        TextMeshPro offline_text = GameObject.Find("Log_offline_text").GetComponent<TextMeshPro>();
        offline_text.text = langs.Return_language_string("Continue_offline");
        TextMeshPro username_pholder = GameObject.Find("Log_username_pholder").GetComponent<TextMeshPro>();
        username_pholder.text = langs.Return_language_string("Username");
        TextMeshPro password_pholder = GameObject.Find("Log_password_pholder").GetComponent<TextMeshPro>();
        password_pholder.text = langs.Return_language_string("Password"); ;
        TextMeshPro forgot_pw_text = GameObject.Find("Forgot_pw_text").GetComponent<TextMeshPro>();
        forgot_pw_text.fontSize = langs.Return_name_of_selected_language() == "Magyar" ? 100 : 130;
        forgot_pw_text.text = langs.Return_language_string("Forgot_password");
        TextMeshPro login_title = GameObject.Find("Login_title").GetComponent<TextMeshPro>();
        login_title.text = langs.Return_language_string("Login");
    }

    private void OnEnable()
    {
        if (started)
        {
            username.text = "";
            password.text = "";
            login_feedback.text = "";
            username_feedback.text = "";
            password_feedback.text = "";
            username.Select();
        }
    }
    public void Verify_password()
    {
        audio_manager.Play_keyboard_press();
        password_feedback.text = Text_not_empty(password.text) ? "" : langs.Return_language_string("Input_empty");
        login_feedback.text = "";
        Verify_submit();
    }
    public void Verify_username()
    {
        audio_manager.Play_keyboard_press();
        username_feedback.text = Text_not_empty(username.text) ? "" : langs.Return_language_string("Input_empty");
        login_feedback.text = "";
        Verify_submit();
    }
    public void Verify_submit()
    {
        submit.interactable = Text_not_empty(username.text) && Text_not_empty(password.text);
    }
    bool Text_not_empty(string text)
    {
        return !text.Equals("");
    }
    public void Password_text()
    {
        Verify_submit();
    }
    public void Show_register()
    {
        StartCoroutine(main_Menu_Script.Panel_switch(this.gameObject, main_Menu_Script.panels["Registration_menu"]));

    }
    public void Show_forgot_pw()
    {
        StartCoroutine(main_Menu_Script.Panel_switch(this.gameObject, main_Menu_Script.panels["Forgot_pw_menu"]));
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && submit.interactable)
        {
            Call_login();
        }
        else if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (username.isFocused)
            {
                password.Select();
            }
            else if (password.isFocused)
            {
                username.Select();
            }
        }
    }
    public void Call_login()
    {
        StartCoroutine(Log_in());
    }
    public void Continue_offline()
    {
        Options options = GameObject.Find("Options_panel").GetComponent<Options>();
        options.Set_save(false);
        StartCoroutine(main_Menu_Script.Panel_switch(this.gameObject, main_Menu_Script.panels["Main_menu"],true));
    }
    IEnumerator Log_in()
    {
        login_feedback.color = Color.white;
        login_feedback.text = langs.Return_language_string("Connecting");
        submit.interactable = false;
        WWWForm form = new WWWForm();
        form.AddField("username", username.text);
        form.AddField("password", password.text);
        UnityWebRequest req = UnityWebRequest.Post("tictactoe3d.ddns.net/tictactoe/login/login.php", form);
        req.certificateHandler = new CertificateHandler_zsivany();
        req.timeout = 10;
        yield return req.SendWebRequest();
        try
        {
            if (req.downloadHandler.text == "-1" || req.downloadHandler.text == "")
            {
                throw new Exception($"{langs.Return_language_string("Connection_failed")}");
            }
            else if (req.downloadHandler.text == "0")
            {
                Debug.Log($"[{Time.frameCount}] Can't find user {username.text}");
                Log_in_failed();
                throw new Exception($"{langs.Return_language_string("Login_incorrect")}");
            }
            else
            {
                StartCoroutine(Log_in_successfull(req.downloadHandler.text));
            }
        }
        catch (Exception ex)
        {
            login_feedback.color = Color.red;
            login_feedback.text = $"{ex.Message}";
            Debug.Log($"[{Time.frameCount}] {login_feedback.text}");
        }
        req.Dispose();
    }

    private void Log_in_failed()
    {
        audio_manager.Play_denied_buzzer();
        username.text = "";
        password.text = "";
        username_feedback.text = "";
        password_feedback.text = "";
        username.Select();
    }

    IEnumerator Log_in_successfull(string data)
    {
        main_Menu_Script.user = new User(data);
        login_feedback.color = Color.green;
        login_feedback.text = $"{langs.Return_language_string("Login_success")}";
        audio_manager.Play_success();
        main_Menu_Script.online = true;
        Debug.Log($"[{Time.frameCount}] Login successfull to {main_Menu_Script.user.Username}");
        Options options = GameObject.Find("Options_panel").GetComponent<Options>();
        options.Set_settings();
        options.Set_save(true);
        Debug.Log($"[{Time.frameCount}] Options set: Res: {main_Menu_Script.user.Resolution_full} - Quality: {main_Menu_Script.user.Quality_index} - Fullscreen: {main_Menu_Script.user.Fullscreen} ");
        yield return new WaitForSeconds(.5f);
        StartCoroutine(main_Menu_Script.Panel_switch(main_Menu_Script.panels["Login_menu"], main_Menu_Script.panels["Main_menu"]));
    }
}
