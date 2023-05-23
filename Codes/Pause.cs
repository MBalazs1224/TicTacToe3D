using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Pause : MonoBehaviour
{
    bool can_pause;
    public bool menu_shown;
    Camera_animations cam_anims;
    [SerializeField]GameObject pause_panel;
    Audio_manager am;
    Slider music_slider;
    Slider effect_slider;
    Options options_menu;
    float[] volumes;
    GridSystem gs;
    void Start()
    {
        can_pause = false;
        menu_shown = false;
        am = Camera.main.GetComponent<Audio_manager>();
        music_slider = GameObject.Find("Pause_menu_music_slider").GetComponent<Slider>();
        effect_slider = GameObject.Find("Pause_menu_effect_slider").GetComponent<Slider>();
        Set_language();
        pause_panel.SetActive(false);
    }
    public void Set_effect_music_volume(float effect_volume,float music_volume)
    {
        music_slider.value = music_volume;
        effect_slider.value = effect_volume;
    }
    private void OnLevelWasLoaded(int level)
    {
        cam_anims = GameObject.Find("GridRunner").GetComponent<Camera_animations>();
        options_menu = GameObject.Find("Options_panel").GetComponent<Options>();
        gs = GameObject.Find("GridRunner").GetComponent<GridSystem>();
        volumes = new float[2];
    }

    private void Set_language()
    {
        Languages lang = GameObject.Find("Language_holder").GetComponent<Languages>();
        TextMeshPro pause_title = GameObject.Find("Pause_menu_title").GetComponent<TextMeshPro>();
        pause_title.text = lang.Return_language_string("Pause");
        TextMeshPro music_slider_text = GameObject.Find("Pause_menu_music_text").GetComponent<TextMeshPro>();
        music_slider_text.text = lang.Return_language_string("Music_volume");
        TextMeshPro effect_slider_text = GameObject.Find("Pause_menu_effect_text").GetComponent<TextMeshPro>();
        effect_slider_text.text = lang.Return_language_string("Effect_volume");
        TextMeshPro resume_button_text = GameObject.Find("Resume_button_text").GetComponent<TextMeshPro>();
        resume_button_text.text = lang.Return_language_string("Resume");
        TextMeshPro back_to_menu_text = GameObject.Find("Back_to_menu_button_text").GetComponent<TextMeshPro>();
        back_to_menu_text.text = lang.Return_language_string("Back_to_main_menu");
    }

    public void Music_slider_change(float volume)
    {
        am.Set_music_volume_to_variable(volume);
        volumes[0] = volume;
    }
    public void Effect_slider_change(float volume)
    {
        am.Set_effect_volume_to_variable(volume);
        volumes[1] = volume;
    }
    public void Set_music_slider_value(float volume)
    {
        music_slider.value = volume;
    }
    public void Set_effect_slider_value(float volume)
    {
        effect_slider.value = volume;
    }
    public void Set_can_pause(bool value)
    {
        can_pause = value;
    }

    public void Resume()
    {
        pause_panel.SetActive(false);
        can_pause = true;
        menu_shown = false;
        Time.timeScale = 1;
        StartCoroutine(am.Increase_music_volume());
        gs.Set_game_state_to_before_pause();
        Debug.Log("Game resumed");
    }
    public void Call_back_to_menu()
    {
        StartCoroutine(Back_to_menu());
    }
    IEnumerator Back_to_menu()
    {
        Main_menu_Script mms = GameObject.Find("Main_menu_canvas").GetComponent<Main_menu_Script>();
        mms.StopAllCoroutines();
        mms.Activate_main_menu();
        GridSystem gs = GameObject.Find("GridRunner").GetComponent<GridSystem>();
        gs.GridReset();
        gs.StopAllCoroutines();
        gs.Set_game_state_to_in_menu();
        gs.Reset_scores();
        menu_shown = false;
        cam_anims.StopAllCoroutines();
        StartCoroutine(am.Increase_music_volume());
        Time.timeScale = 1;
        pause_panel.SetActive(false);
        Debug.Log("Back to main menu");
        yield return null;
        StartCoroutine(cam_anims.Back_to_main_menu(false));
        options_menu.Set_sliders_from_pause(volumes);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (can_pause && !menu_shown)
            {
                pause_panel.SetActive(true);
                can_pause = false;
                Time.timeScale = 0;
                menu_shown = true;
                StartCoroutine(am.Mute_music());
                gs.Set_game_state_to_paused();
                am.Play_keyboard_press();
                Debug.Log("Game paused");
            }
            else if (menu_shown)
            {
                Resume();
                am.Play_keyboard_press();
            }
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            can_pause = true;
        }
    }
}
