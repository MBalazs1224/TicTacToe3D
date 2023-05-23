using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class Main_menu_Script : MonoBehaviour
{
    public Dictionary<string, GameObject> panels;
    GridSystem gridSystem;
    Vector3 panel_in_position;
    Vector3 panel_out_position;
    public bool panels_switching = false;
    public bool online;
    public User user;
    Options options;
    Audio_manager audio_Manager;
    void Start()
    {
        panels = new Dictionary<string, GameObject>();
        panels.Add("Main_menu", GameObject.Find("Main_menu_panel"));
        panels.Add("Options_menu", GameObject.Find("Options_panel"));
        panels.Add("Registration_menu", GameObject.Find("Registration_panel"));
        panels.Add("Login_menu", GameObject.Find("Login_panel"));
        panels.Add("Forgot_pw_menu",GameObject.Find("Forgot_pw_panel"));
        panels.Add("Choose_play_mode_menu",GameObject.Find("Choose_play_mode_panel"));
        panel_in_position = panels["Login_menu"].transform.position;
        panel_out_position = new Vector3(panels["Login_menu"].transform.position.x, panels["Login_menu"].transform.position.y, -40);
        panels["Main_menu"].SetActive(false);
        panels["Options_menu"].transform.position = panel_out_position;
        panels["Options_menu"].SetActive(true);
        panels["Registration_menu"].SetActive(false);
        panels["Login_menu"].SetActive(true);
        panels["Forgot_pw_menu"].SetActive(false);
        panels["Choose_play_mode_menu"].SetActive(false);
        //panels["Main_menu"].transform.position = panel_out_position;
        //panels["Registration_menu"].transform.position = panel_out_position;
        //panels["Login_menu"].transform.position = panel_in_position;
        //panels["Options_menu"].transform.position = panel_out_position;
        gridSystem = GameObject.Find("GridRunner").GetComponent<GridSystem>();
        options = panels["Options_menu"].GetComponent<Options>();
        online = false;
        audio_Manager = Camera.main.GetComponent<Audio_manager>();
        Debug.Log(panels);
    }
    public IEnumerator Reload_main()
    {
        GameObject obj = panels["Main_menu"];
        while (Vector3.Distance(obj.transform.position, panel_out_position) > 3)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, panel_out_position, 20f * Time.deltaTime);
            yield return null;
        }
        obj.SetActive(false);
        obj.SetActive(true);
        while (Vector3.Distance(obj.transform.position, panel_in_position) > 3)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, panel_in_position, 20f * Time.deltaTime);
            yield return null;
        }
    }
    public void Disable_panels()
    {
        foreach (var item in panels)
        {
            item.Value.SetActive(false);
        }
    }

    public void Main_menu_play()
    {
        if (gridSystem.Is_game_in_menu())
        {
            StartCoroutine(Panel_switch(panels["Main_menu"], panels["Choose_play_mode_menu"]));
        }
    }
    /// <summary>
    /// Moves first panel out of position, then the second one into position, sets the first one inactive, then the second one active. If second panel is not given, it moves the first panel out of position.
    /// </summary>
    public IEnumerator Panel_switch(GameObject panel_one, GameObject panel_two = null, bool sec = false)
    {
        panels_switching = true;
        Debug.Log("Panel switching started!");
        if (panel_two != null)
        {
            panel_two.transform.position = panel_out_position;
        }
        audio_Manager.Play_swoosh();
        while (Vector3.Distance(panel_one.transform.position, panel_out_position) > 3)
        {
            panel_one.transform.position = Vector3.MoveTowards(panel_one.transform.position, panel_out_position, 30f * Time.deltaTime);
            yield return null;
        }
        Debug.Log($"[{Time.frameCount}] First panel ({panel_one.name}) out of position");
        if (panel_two != null)
        {
            panel_two.SetActive(true);
            Debug.Log($"[{Time.frameCount}] Second panel ({panel_two.name}) movement started");
            audio_Manager.Play_swoosh();
            while (Vector3.Distance(panel_two.transform.position, panel_in_position) > 1)
            {
                panel_two.transform.position = Vector3.MoveTowards(panel_two.transform.position, panel_in_position, 30f * Time.deltaTime);
                yield return null;
            }
            panel_one.SetActive(sec);
        }
        Debug.Log($"[{Time.frameCount}] Panels switched / disappeared");
        panels_switching = false;
    }
    public void Main_menu_options()
    {
        if (!panels_switching)
        {
            Debug.Log($"[{Time.frameCount}] Options loaded");
            StartCoroutine(Panel_switch(panels["Main_menu"], panels["Options_menu"]));

        }
    }
    
    /// <summary>
    /// Moves the main camera into the starting position.
    /// </summary>
    /// <returns></returns>
    IEnumerator Game_start()
    {
        StartCoroutine(Panel_switch(panels["Main_menu"]));
        Camera_animations cam_anims = GameObject.Find("GridRunner").GetComponent<Camera_animations>();
        yield return new WaitForSeconds(.5f);
        Disable_panels();
        StartCoroutine(cam_anims.Camera_to_starting_position());
    }
    /// <summary>
    /// Exits the application.
    /// </summary>
    public void Main_menu_exit()
    {
        Debug.Log($"[{Time.frameCount}] Exit");
        Application.Quit();
    }
    /// <summary>
    /// Activates the basic main menu
    /// </summary>
    public void Activate_main_menu()
    {
        panels["Main_menu"].SetActive(true);
        panels["Options_menu"].SetActive(true);
        panels["Main_menu"].transform.position = panel_in_position;
        panels["Options_menu"].transform.position = panel_out_position;
    }
}
