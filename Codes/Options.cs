using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class Options : MonoBehaviour
{
    Resolution[] resolutions;
    List<string> res = new List<string>();
    Dropdown resolution_dropdown;
    Dropdown quality_dropdown;
    User user;
    Main_menu_Script main_Menu_Script;
    Toggle fullscreen;
    Button save;
    bool started = false;
    Slider music_volume;
    Slider effect_volume;
    Audio_manager audio_manager;
    Pause pause_menu;
    TextMeshPro save_feedback;
    Languages langs;
    void Start()
    {
        Set_variables();
        Resolution_Dropdown_Set();
        Set_language();
        started = true;
        Screen.fullScreenMode = FullScreenMode.MaximizedWindow;

    }
    private void OnEnable()
    {
        if (started)
        {
            save_feedback.text = "";
        }
    }
    /// <summary>
    /// Sets the texts to the correct language.
    /// </summary>
    private void Set_language()
    {
        TextMeshPro back = GameObject.Find("Options_back_text").GetComponent<TextMeshPro>();
        back.text = langs.Return_language_string("Back");
        TextMeshPro save = GameObject.Find("Options_button_save_text").GetComponent<TextMeshPro>();
        save.text = langs.Return_language_string("Save");
        TextMeshPro fs = GameObject.Find("Options_fullscreen_text").GetComponent<TextMeshPro>();
        fs.text = langs.Return_language_string("Fullscreen");
        TextMeshPro music_volume_text = GameObject.Find("Music_volume_text").GetComponent<TextMeshPro>();
        music_volume_text.text = langs.Return_language_string("Music_volume");
        TextMeshPro effect_volume_text = GameObject.Find("Effect_volume_text").GetComponent<TextMeshPro>();
        effect_volume_text.text = langs.Return_language_string("Effect_volume");
        TextMeshPro options_title = GameObject.Find("Options_title").GetComponent<TextMeshPro>();
        options_title.text = langs.Return_language_string("Options");
    }
    /// <summary>
    /// Sets the game's effect volume to the parameter value.
    /// </summary>
    /// <param name="volume">The value the volume should be set to.</param>
    public void Set_effect_volume(float volume)
    {
        if (main_Menu_Script.online) user.Effect_volume = volume;
        audio_manager.Set_effect_volume_to_variable(volume);
        pause_menu.Set_effect_slider_value(volume);
    }
    /// <summary>
    /// Sets the volumes from the pause menu sliders.
    /// </summary>
    /// <param name="values"></param>
    public void Set_sliders_from_pause(float[] values)
    {
        music_volume.value = values[0];
        effect_volume.value = values[1];
    }
    /// <summary>
    /// Sets the game's music volume to the parameter value.
    /// </summary>
    /// <param name="volume">The value the volume should be set to.</param>
    public void Set_menu_volume(float volume)
    {
        if (main_Menu_Script.online) user.Music_volume = volume;
        audio_manager.Set_music_volume_to_variable(volume);
        pause_menu.Set_music_slider_value(volume);
    }
    private void Set_variables()
    {
        main_Menu_Script = GameObject.Find("Main_menu_canvas").GetComponent<Main_menu_Script>();
        resolution_dropdown = GameObject.Find("Resolution_dropdown").GetComponent<Dropdown>();
        fullscreen = GameObject.Find("Fullscreen_toggle").GetComponent<Toggle>();
        quality_dropdown = GameObject.Find("Quality_dropd").GetComponent<Dropdown>();
        save = GameObject.Find("Options_button_save").GetComponent<Button>();
        langs = GameObject.Find("Language_holder").GetComponent<Languages>();
        audio_manager = Camera.main.GetComponent<Audio_manager>();
        music_volume = GameObject.Find("Music_volume").GetComponent<Slider>();
        effect_volume = GameObject.Find("Effect_volume").GetComponent<Slider>();
        pause_menu = GameObject.Find("Camera_canvas").GetComponent<Pause>();
        pause_menu.Set_effect_music_volume(effect_volume.value, music_volume.value);
        save_feedback = GameObject.Find("Options_save_feedback").GetComponent<TextMeshPro>();
        save_feedback.text = "";
    }
    public void Reset_save_feedback()
    {
        save_feedback.text = "";
    }
    /// <summary>
    /// Goes back to the main menu from the options menu.
    /// </summary>
    public void Options_back()
    {
        if (!main_Menu_Script.panels_switching)
        {
            Debug.Log($"[{Time.frameCount}] Options back");
            StartCoroutine(main_Menu_Script.Panel_switch(main_Menu_Script.panels["Options_menu"], main_Menu_Script.panels["Main_menu"], true));
            Reset_save_feedback();
        }

    }

    public void Set_save(bool click)
    {
        save.interactable = click;
    }
    public void Set_settings()
    {
        user = main_Menu_Script.user;
        Pull_settings_from_user();
    }
    /// <summary>
    /// Sets the in-game settings to the values saved in the database. 
    /// </summary>
    private void Pull_settings_from_user()
    {
        quality_dropdown.value = user.Quality_index;
        Quality_set(user.Quality_index);
        fullscreen.isOn = user.Fullscreen;
        Fullscreen_set(user.Fullscreen);
        Resolution_from_user();
        Set_sliders_from_user();
    }
    /// <summary>
    /// Sets the audio volumes to the values saved in the database.
    /// </summary>
    private void Set_sliders_from_user()
    {
        music_volume.value = user.Music_volume;
        effect_volume.value = user.Effect_volume;
        pause_menu.Set_effect_music_volume(effect_volume.value, music_volume.value);
    }

    /// <summary>
    /// Sets the resolution to the one saved into the database if the display supports it.
    /// </summary>
    private void Resolution_from_user()
    {
        int counter = 0;
        while (counter < res.Count && !res[counter].ToString().Equals(user.Resolution_full))
        {
            counter++;
        }
        if (counter < res.Count)
        {
            Resolution_set(counter);
            resolution_dropdown.value = counter;
            resolution_dropdown.RefreshShownValue();
        }
    }
    public void Call_save()
    {
        Debug.Log("Save to cloud");
        StartCoroutine(Save());

    }
    /// <summary>
    /// Starts the WebRequest that saves the settings into the database
    /// </summary>
    /// <returns></returns>
    IEnumerator Save()
    {
        save_feedback.color = Color.white;
        save_feedback.text = langs.Return_language_string("Connecting");
        save.interactable = false;
        WWWForm form = new WWWForm();
        int fs = user.Fullscreen ? 1 : 0;
        form.AddField("id", user.Id);
        form.AddField("res", user.Resolution_full);
        form.AddField("quality", user.Quality_index);
        form.AddField("fullscreen", fs);
        form.AddField("music_volume", user.Music_volume.ToString());
        form.AddField("effect_volume", user.Effect_volume.ToString());
        string valami = user.Fullscreen.ToString();
        UnityWebRequest req = UnityWebRequest.Post("tictactoe3d.ddns.net/tictactoe/update/update.php", form);
        req.certificateHandler = new CertificateHandler_zsivany();
        yield return req.SendWebRequest();
        if (req.downloadHandler.text == "-1" || req.downloadHandler.text == "")
        {
            Connection_failed();
        }
        else
        {
            Save_successfull();
        }
        save.interactable = true;
        req.Dispose();
    }

    private void Save_successfull()
    {
        save_feedback.color = Color.green;
        save_feedback.text = langs.Return_language_string("Save_successfull");
    }

    private void Connection_failed()
    {
        save_feedback.color = Color.red;
        save_feedback.text = langs.Return_language_string("Connection_failed");
    }

    public void Resolution_set(int index)
    {
        if (started)
        {
            Resolution res = resolutions[index];
            if (main_Menu_Script.online)
            {
                user.Res_width = res.width;
                user.Res_height = res.height;
                user.Resolution_full = $"{user.Res_width}x{user.Res_height}";
            }
            Screen.SetResolution(res.width, res.height, fullscreen.isOn);
        }
    }
    public void Quality_set(int index)
    {
        QualitySettings.SetQualityLevel(index);
        if (main_Menu_Script.online)
        {
            user.Quality_index = index;
        }
    }
    public void Fullscreen_set(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        if (main_Menu_Script.online) user.Fullscreen = isFullscreen;
    }
    private void Resolution_Dropdown_Set()
    {
        resolution_dropdown.ClearOptions();
        resolutions = Screen.resolutions;
        int current_resolution = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            if (!res.Contains(option))
            {
                res.Add(option);
            }
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                current_resolution = i;
            }
        }
        resolution_dropdown.AddOptions(res);
        resolution_dropdown.value = current_resolution;
        resolution_dropdown.RefreshShownValue();
    }
}
