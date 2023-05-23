using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using TMPro;
public class Registration : MonoBehaviour
{
    InputField username;
    InputField password;
    InputField email;
    Button submit;
    TextMeshPro register_feedback;
    TextMeshPro email_feedback;
    TextMeshPro username_feedback;
    TextMeshPro password_feedback;
    bool started = false;
    Audio_manager audio_Manager;
    Languages langs;
    private void Start()
    {
        Set_variables();
        Set_language();
        submit.interactable = false;
        email.Select();
        started = true;
    }

    private void Set_language()
    {
        TextMeshPro email_pholder = GameObject.Find("Reg_email_pholder").GetComponent<TextMeshPro>();
        email_pholder.text = langs.Return_language_string("Email");
        TextMeshPro uname_pholder = GameObject.Find("Reg_username_pholder").GetComponent<TextMeshPro>();
        uname_pholder.text = langs.Return_language_string("Username");
        TextMeshPro pw_pholder = GameObject.Find("Reg_pw_pholder").GetComponent<TextMeshPro>();
        pw_pholder.text = langs.Return_language_string("Password");
        TextMeshPro submit_text = GameObject.Find("Reg_submit_text").GetComponent<TextMeshPro>();
        submit_text.text = langs.Return_language_string("Submit");
        TextMeshPro back_text = GameObject.Find("Reg_back_text").GetComponent<TextMeshPro>();
        back_text.text = langs.Return_language_string("Back");
        TextMeshPro reg_title = GameObject.Find("Reg_title").GetComponent<TextMeshPro>();
        reg_title.text = langs.Return_language_string("Register");
        audio_Manager = Camera.main.GetComponent<Audio_manager>();
    }

    private void Set_variables()
    {
        username = GameObject.Find("Reg_input_username").GetComponent<InputField>();
        password = GameObject.Find("Reg_input_password").GetComponent<InputField>();
        email = GameObject.Find("Reg_input_email").GetComponent<InputField>();
        submit = GameObject.Find("Reg_submit").GetComponent<Button>();
        register_feedback = GameObject.Find("Reg_feedback").GetComponent<TextMeshPro>();
        email_feedback = GameObject.Find("Email_feedback").GetComponent<TextMeshPro>();
        username_feedback = GameObject.Find("Username_feedback").GetComponent<TextMeshPro>();
        password_feedback = GameObject.Find("Password_feedback").GetComponent<TextMeshPro>();
        langs = GameObject.Find("Language_holder").GetComponent<Languages>();
    }

    private void OnEnable()
    {
        if (started)
        {
            username.text = "";
            password.text = "";
            email.text = "";
            register_feedback.text = "";
            username_feedback.text = "";
            password_feedback.text = "";
            email.Select();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && submit.interactable)
        {
            Call_register();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (email.isFocused)
            {
                username.Select();
            }
            else if (username.isFocused)
            {
                password.Select();
            }
            else if (password.isFocused)
            {
                email.Select();
            }
        }
    }
    public void Call_register()
    {
        StartCoroutine(Register());
    }
    public void Go_back()
    {
        Main_menu_Script main_Menu_Script = GameObject.Find("Main_menu_canvas").GetComponent<Main_menu_Script>();
        StartCoroutine(main_Menu_Script.Panel_switch(gameObject, main_Menu_Script.panels["Login_menu"]));
    }
    public void Verify_email()
    {
        audio_Manager.Play_keyboard_press();
        email_feedback.text = Valid_email() ? "" : langs.Return_language_string("Email_wrong"); ;
        register_feedback.text = "";
        Verify_submit();
    }
    public void Verify_username()
    {
        audio_Manager.Play_keyboard_press();
        username_feedback.text = Text_not_empty(username.text) ? "" : langs.Return_language_string("Input_empty");
        if (username_feedback.text == "") username_feedback.text = username.text.Contains("/") ? langs.Return_language_string("Username_slash") : "";
        register_feedback.text = "";
        Verify_submit();
    }
    public void Verify_password()
    {
        audio_Manager.Play_keyboard_press();
        password_feedback.text = Text_not_empty(password.text) ? "" : langs.Return_language_string("Input_empty");
        register_feedback.text = "";
        Verify_submit();
    }
    private bool Valid_email()
    {
        return email.text.Contains("@") && email.text.Contains(".");
    }
    bool Text_not_empty(string text)
    {
        return !text.Equals("");
    }
    public void Verify_submit()
    {
        submit.interactable = Text_not_empty(username.text) && Text_not_empty(password.text) && Text_not_empty(email.text) && Valid_email() && !username.text.Contains("/");
    }
    IEnumerator Register()
    {
        register_feedback.color = Color.white;
        register_feedback.text = langs.Return_language_string("Connecting");
        WWWForm form = new WWWForm();
        int lang_index = langs.Return_name_of_selected_language() == "Magyar" ? 1 : 0;
        form.AddField("email", email.text);
        form.AddField("username", username.text);
        form.AddField("password", password.text);
        form.AddField("lang", lang_index);
        UnityWebRequest req = UnityWebRequest.Post("tictactoe3d.ddns.net/tictactoe/register/register.php", form);
        req.certificateHandler = new CertificateHandler_zsivany();
        yield return req.SendWebRequest();
        if (req.downloadHandler.text == "-1" || req.downloadHandler.text == "")
        {
            audio_Manager.Play_denied_buzzer();
            Connection_failed();
        }
        else if (req.downloadHandler.text == "0")
        {
            Debug.Log($"[{Time.frameCount}] Account {username.text} created");
            audio_Manager.Play_success();
            register_feedback.color = Color.green;
            register_feedback.text = $"{langs.Return_language_string("Account_created")}";
            yield return new WaitForSeconds(2f);
            Main_menu_Script main_Menu_Script = GameObject.Find("Main_menu_canvas").GetComponent<Main_menu_Script>();
            StartCoroutine(main_Menu_Script.Panel_switch(this.gameObject, main_Menu_Script.panels["Login_menu"]));
            
        }
        else
        {
            
            Account_exists();
        }
        req.Dispose();
        //Debug.Log($"[{Time.frameCount}] {req.downloadHandler.text}");
    }

    private void Connection_failed()
    {
        Debug.Log($"[{Time.frameCount}] Connection failed!");
        register_feedback.color = Color.red;
        register_feedback.text = $"{langs.Return_language_string("Connection_failed")}";
        email.Select();
    }

    private void Account_exists()
    {
        Debug.Log($"[{Time.frameCount}] Account already exists");
        audio_Manager.Play_denied_buzzer();
        register_feedback.color = Color.red;
        email.text = "";
        username.text = "";
        password.text = "";
        email_feedback.text = "";
        username_feedback.text = "";
        password_feedback.text = "";
        register_feedback.text = $"{langs.Return_language_string("Account_exists")}";
        email.Select();
    }
}
