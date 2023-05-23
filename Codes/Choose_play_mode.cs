using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Choose_play_mode : MonoBehaviour
{
    TextMeshPro title;
    Dropdown dropdown;
    GridSystem gs;
    Main_menu_Script mms;
    TextMeshPro back_text;
    TextMeshPro play_text;
    void Start()
    {
        title = GameObject.Find("Choose_play_mode_title").GetComponent<TextMeshPro>();
        Languages lang = GameObject.Find("Language_holder").GetComponent<Languages>();
        title.text = lang.Return_language_string("Choose_play_mode");
        dropdown = GameObject.Find("Choose_play_dropdown").GetComponent<Dropdown>();
        List<string> list = new List<string>();
        list.Add(lang.Return_language_string("One_player"));
        list.Add(lang.Return_language_string("Two_player"));
        dropdown.options.Clear();
        dropdown.AddOptions(list);
        gs = GameObject.Find("GridRunner").GetComponent<GridSystem>();
        mms = GameObject.Find("Main_menu_canvas").GetComponent<Main_menu_Script>();
        back_text = GameObject.Find("Choose_play_back_text").GetComponent<TextMeshPro>();
        back_text.text = lang.Return_language_string("Back");
        play_text = GameObject.Find("Choose_play_play_text").GetComponent<TextMeshPro>();
        play_text.text = lang.Return_language_string("Play_game");
    }
    public void Dropdown_changed(int value)
    {
        gs.Set_play_mode(value == 0);
    }
    IEnumerator Game_start()
    {
        StartCoroutine(mms.Panel_switch(this.gameObject));
        yield return new WaitForSeconds(.5f);
        Camera_animations cam_anims = GameObject.Find("GridRunner").GetComponent<Camera_animations>();
        cam_anims.Call_camera_to_starting_position();
        mms.Disable_panels();
    }
    public void Play_game()
    {
        Camera_animations cam = GameObject.Find("GridRunner").GetComponent<Camera_animations>();
        cam.StopAllCoroutines();
        Debug.Log($"[{Time.frameCount}] Game started");
        gs.current_game_state = GridSystem.Game_state.Not_Won;
        gs.current_Player = 1;
        //mms.panels["Options_menu"].SetActive(false);
        StartCoroutine(Game_start());
    }
    public void Go_back()
    {
        StartCoroutine(mms.Panel_switch(this.gameObject,mms.panels["Main_menu"]));
    }
}
