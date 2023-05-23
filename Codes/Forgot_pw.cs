using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class Forgot_pw : MonoBehaviour
{
    InputField email;
    TextMesh feedback;
    Button submit;
    Main_menu_Script main_Menu_Script;
    Languages langs;
    bool started = false;
    Audio_manager am;
    void Start()
    {
        email = GameObject.Find("Forgot_pw_email").GetComponent<InputField>();
        feedback = GameObject.Find("Forgot_pw_feedback").GetComponent<TextMesh>();
        submit = GameObject.Find("Forgot_pw_submit").GetComponent<Button>();
        submit.interactable = false;
        langs = GameObject.Find("Language_holder").GetComponent<Languages>();
        main_Menu_Script = GameObject.Find("Main_menu_canvas").GetComponent<Main_menu_Script>();
        am = Camera.main.GetComponent<Audio_manager>();
        Set_language();
        started = true;
    }
    private void OnEnable()
    {
        if (started)
        {
            email.text = "";
            feedback.text = "";
            submit.interactable = false;
        }
    }
    private void Set_language()
    {
        TextMeshPro submit = GameObject.Find("Forgot_pw_submit_text").GetComponent<TextMeshPro>();
        submit.text = langs.Return_language_string("Submit");
        TextMeshPro back = GameObject.Find("Forgot_pw_back_text").GetComponent<TextMeshPro>();
        back.text = langs.Return_language_string("Back");
        TextMeshPro forgot_pw_title = GameObject.Find("Forgot_pw_title").GetComponent<TextMeshPro>();
        forgot_pw_title.text = langs.Return_language_string("Forgot_password");
    }

    public void Verify_submit()
    {
        if (Verify_email())
        {
            submit.interactable = true;
            feedback.text = "";
        }
        else
        {
            feedback.color = Color.red;
            submit.interactable = false;
            feedback.text = langs.Return_language_string("Email_wrong");
        }
        am.Play_keyboard_press();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && submit.interactable)
        {
            Call_forgot_pw();
        }
    }
    public void Call_forgot_pw()
    {
        StartCoroutine(Forgot_pw_conn());
    }
    IEnumerator Forgot_pw_conn()
    {
        feedback.color = Color.white;
        feedback.text = langs.Return_language_string("Connecting");
        WWWForm form = new WWWForm();
        int lang = langs.Return_name_of_selected_language() == "Magyar" ? 1 : 0;
        form.AddField("email", email.text);
        form.AddField("lang", lang);
        UnityWebRequest req = UnityWebRequest.Post("tictactoe3d.ddns.net/tictactoe/forgot_pw/forgot_pw_mail.php", form);
        req.certificateHandler = new CertificateHandler_zsivany();
        yield return req.SendWebRequest();
        switch (req.downloadHandler.text)
        {
            case "0":
                Email_doesnt_exists();
                break;
            case "1":
                StartCoroutine(Successfull());
                break;
            default:
                Cant_connect_to_server();
                break;
        }
    }

    private IEnumerator Successfull()
    {
        am.Play_success();
        feedback.color = Color.green;
        feedback.text = langs.Return_language_string("Email_sent");
        yield return new WaitForSeconds(3f);
        StartCoroutine(main_Menu_Script.Panel_switch(this.gameObject, main_Menu_Script.panels["Login_menu"]));
    }
    public void Show_login()
    {
        StartCoroutine(main_Menu_Script.Panel_switch(this.gameObject, main_Menu_Script.panels["Login_menu"]));
    }
    private void Email_doesnt_exists()
    {
        am.Play_denied_buzzer();
        feedback.color = Color.red;
        feedback.text = langs.Return_language_string("Email_doesnt_exist");
    }

    private void Cant_connect_to_server()
    {
        feedback.color = Color.red;
        feedback.text = langs.Return_language_string("Connection_failed");
    }

    private bool Verify_email()
    {
        return email.text.Contains("@") && email.text.Contains(".");
    }
}
