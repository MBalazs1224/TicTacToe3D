using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;
using TMPro;

public class Languages : MonoBehaviour
{
    TMP_Dropdown dropdown;
    int current_lang = 0;
    List<Dictionary<string, string>> languages = new List<Dictionary<string, string>>();
    List<string> options = new List<string>();
    TextMeshPro button_text;
    TextMeshPro choose_text;
    GameObject main_cube;
    Camera main_cam;
    TextMeshPro loading_text;
    Vector3 target;
    Vector3 target_back;
    Audio_manager audio_Manager;
    GameObject[] effect_bars = new GameObject[2];
    void Start()
    {
        Set_variables();
        Set_dont_destroy();
        Set_dropdown();
        Debug.Log($"[{Time.frameCount}] Intro startup");
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Set_language_from_OS();
    }
    public string Return_name_of_selected_language()
    {
        return options[current_lang];
    }
    public string Return_language_string(string key)
    {
        return languages[current_lang][key];
    }
    private void Set_language_from_OS()
    {
        if (Application.systemLanguage == SystemLanguage.Hungarian)
        {
            dropdown.value = options.IndexOf("Magyar");
        }
    }
    private void Awake()
    {
        Read_files();
    }
    private void Set_dropdown()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }

    private void Set_dont_destroy()
    {
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(Camera.main);
    }

    private void Read_files()
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/Languages", "*.ini");
        foreach (string file in files)
        {
            Read_lang_file(file);
        }
    }

    private void Set_variables()
    {
        dropdown = GameObject.Find("Dropdown").GetComponent<TMP_Dropdown>();
        button_text = GameObject.Find("Submit_text").GetComponent<TextMeshPro>();
        choose_text = GameObject.Find("Choose_lang").GetComponent<TextMeshPro>();
        main_cam = Camera.main;
        target = main_cam.transform.position + new Vector3(0, 0, 2.5f);
        target_back = main_cam.transform.position + new Vector3(0, .2f, -2);
        main_cube = GameObject.Find("Cube");
        loading_text = GameObject.Find("Loading_text").GetComponent<TextMeshPro>();
        audio_Manager = Camera.main.GetComponent<Audio_manager>();
        effect_bars[0] = GameObject.Find("Effect_bar_top");
        effect_bars[1] = GameObject.Find("Effect_bar_bottom");
        button_text.text = languages[current_lang]["Submit"];
        choose_text.text = languages[current_lang]["Choose_lang"];
    }

    public GameObject[] Return_effect_bars()
    {
        return effect_bars;
    }
    public void Set_language_index(int index)
    {
        current_lang = index;
        button_text.text = languages[current_lang]["Submit"];
        choose_text.text = languages[current_lang]["Choose_lang"];
    }
    private void Read_lang_file(string filepath)
    {
        using (StreamReader reader = new StreamReader(filepath))
        {
            Dictionary<string, string> lang = new Dictionary<string, string>();
            options.Add(reader.ReadLine());
            while (!reader.EndOfStream)
            {
                string[] temp = reader.ReadLine().Split('=');
                lang.Add(temp[0], temp[1]);
            }
            languages.Add(lang);
        }
    }
    public void Next_scene()
    {
        loading_text.text = languages[current_lang]["Loading"];
        Debug.Log($"[{Time.frameCount}] Next scene started loading!");
        StartCoroutine(Load_scene());
    }
    IEnumerator Load_scene()
    {
        yield return Drop_loading_screen();
        AsyncOperation op = SceneManager.LoadSceneAsync(1);
        op.allowSceneActivation = false;
        yield return new WaitUntil(() => op.progress == .9f);
        yield return Camera_anim();
        op.allowSceneActivation = true;
    }
    IEnumerator Drop_loading_screen()
    {
        Debug.Log($"[{Time.frameCount}] Loading screen started dropping");
        GameObject loading_cube = GameObject.Find("Loading_cube");
        Vector3 loading_target = loading_cube.transform.position - new Vector3(0,5f,0);
        while (Vector3.Distance(loading_cube.transform.position,loading_target) > 1)
        {
            loading_cube.transform.position = Vector3.MoveTowards(loading_cube.transform.position, loading_target, 8f * Time.deltaTime);
            yield return null;
        }
        //main_cube.SetActive(false);
        Disable_controls();
        Debug.Log($"[{Time.frameCount}] Loading screen finished dropping");
    }

    private void Disable_controls()
    {
        GameObject.Find("Dropdown").SetActive(false);
        GameObject.Find("Submit").SetActive(false);
        GameObject.Find("Choose_lang").SetActive(false);
    }

    IEnumerator Camera_anim()
    {
        Debug.Log($"[{Time.frameCount}] Back camera animation started");
        float first_speed = 2;
        yield return new WaitForSeconds(.1f);
        while (Vector3.Distance(main_cam.transform.position, target_back) > 1)
        {
            main_cam.transform.position = Vector3.Lerp(main_cam.transform.position, target_back, first_speed * Time.deltaTime);
            //first_speed -= .02f;
            yield return null;
        }
        Debug.Log($"[{Time.frameCount}] Forward camera animation started");
        audio_Manager.Play_intro_swoosh();
        yield return new WaitForSeconds(.4f);
        while (Vector3.Distance(main_cam.transform.position, target) > 1)
        {
            main_cam.transform.position = Vector3.MoveTowards(main_cam.transform.position, target, 10f * Time.deltaTime);
            yield return null;
        }
        Debug.Log($"[{Time.frameCount}] Camera animation finished");
    }
}
