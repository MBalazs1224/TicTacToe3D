using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Camera_animations : MonoBehaviour
{
    Camera main_camera;
    Vector3 camera_Starting_Menu_Position = new Vector3(23.6f, 3.4f, -23);
    float Cam_Movement_speed = 3;
    Canvas_Script canvas_script;
    GridSystem gridSystem;
    float Cam_Rotation_speed = 5;
    public Vector3 camera_starting_game_position = new Vector3(5, 3.8f, -15);
    Audio_manager audio_manager;
    Quaternion Camera_starting_menu_rotation;
    GameObject[] effect_bars;
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        main_camera = Camera.main;
        Debug.Log(main_camera);
        //main_camera.transform.position = GameObject.Find("Main_menu_panel").transform.position - new Vector3(0, 3, 0);
        canvas_script = GameObject.Find("Canvas").GetComponent<Canvas_Script>();
        gridSystem = GameObject.Find("GridRunner").GetComponent<GridSystem>();
        audio_manager = Camera.main.GetComponent<Audio_manager>();
        Languages lang = GameObject.Find("Language_holder").GetComponent<Languages>();
        effect_bars = lang.Return_effect_bars();
        Set_bars_to_state(false);
        StartCoroutine(Cam_level_start());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10)) 
        { 
            StopAllCoroutines();
            main_camera.transform.position = camera_starting_game_position;
            main_camera.transform.rotation = Quaternion.Euler(0, 5, 0);
        }

    }
    public void Call_camera_to_starting_position()
    {
        StartCoroutine(Camera_to_starting_position());
    }

    public IEnumerator Back_to_main_menu(bool from_game)
    {
        Vector3 first_target = Vector3.zero;
        if (from_game)
        {
            first_target = main_camera.transform.position - new Vector3(0, 0, 5);
        }
        if (first_target != Vector3.zero)
        {
            while (Vector3.Distance(main_camera.transform.position, first_target) > 1f)
            {
                main_camera.transform.position = Vector3.LerpUnclamped(main_camera.transform.position, first_target, 1f * Time.deltaTime);
                yield return null;
            }
        }
        StartCoroutine(Camera_to_starting_menu_rotation());
        audio_manager.Play_intro_swoosh();
        while (Vector3.Distance(main_camera.transform.position,camera_Starting_Menu_Position) > .1f)
        {
            main_camera.transform.position = Vector3.LerpUnclamped(main_camera.transform.position,camera_Starting_Menu_Position,2f * Time.deltaTime);
            yield return null;
        }
        gridSystem.Set_game_state_to_in_menu();
    }

    private IEnumerator Camera_to_starting_menu_rotation()
    {
        while (Quaternion.Angle(main_camera.transform.rotation,Camera_starting_menu_rotation) > 1)
        {
            main_camera.transform.rotation = Quaternion.LerpUnclamped(main_camera.transform.rotation, Camera_starting_menu_rotation, 2f * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator Cam_level_start()
    {
        Camera_starting_menu_rotation = main_camera.transform.rotation;
        float speed = 2f;
        while (Vector3.Distance(main_camera.transform.position, camera_Starting_Menu_Position) > .5f)
        {
            main_camera.transform.position = Vector3.Lerp(main_camera.transform.position, camera_Starting_Menu_Position, speed * Time.deltaTime);
            //speed -= .01f;
            yield return null;
        }
    }
    public IEnumerator Camera_Draw_Animation(Vector3 position)
    {
        while (Vector3.Distance(main_camera.transform.position,position) > 1f)
        {
            main_camera.transform.position = Vector3.Slerp(main_camera.transform.position,position,Cam_Movement_speed * Time.deltaTime);
            yield return null;
        }
    }
    private void OnLevelWasLoaded(int level)
    {
        main_camera = Camera.main;
    }
    public IEnumerator Camera_win_anim(Vector3 First_Target, Vector3 Second_Target, Vector3 Last_Target, Vector3 Panel_Target, int[] scores)
    {
        Debug.Log($"[Frame:{Time.frameCount}] Moving cam to first target");
        while (Vector3.Distance(main_camera.transform.position, First_Target) > 1f)
        {
            main_camera.transform.position = Vector3.Slerp(main_camera.transform.position, First_Target, Cam_Movement_speed * Time.deltaTime);
            yield return null;
        }
        Debug.Log($"[Frame:{Time.frameCount}] Moving cam to second target");
        StartCoroutine(audio_manager.Slow_down_music());
        audio_manager.Play_slowmo();
        StartCoroutine(Move_effect_bars(true, true, false));
        Time.timeScale = .7f;
        Draw_line draw_Line = GameObject.Find("GridRunner").GetComponent<Draw_line>();
        StartCoroutine(draw_Line.Draw_Line());
        while (Vector3.Distance(main_camera.transform.position, Second_Target) > 1f)
        {
            main_camera.transform.position = Vector3.MoveTowards(main_camera.transform.position, Second_Target, Cam_Movement_speed * 1.2f * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(Move_effect_bars(false, false, true));
        StartCoroutine(audio_manager.Speed_up_music());
        Time.timeScale = 1;
        canvas_script.Panel_Set(Panel_Target, scores);
        Debug.Log($"[Frame:{Time.frameCount}] Moving cam to third target");
        while (Vector3.Distance(main_camera.transform.position, Last_Target) > 1f)
        {
            main_camera.transform.position = Vector3.Slerp(main_camera.transform.position, Last_Target, Cam_Movement_speed * Time.deltaTime);
            yield return null;
        }
        Debug.Log($"[Frame:{Time.frameCount}] Cam movement ended");
    }
    
    IEnumerator Move_effect_bars(bool top_bar,bool bottom_bar,bool set_inactive)
    {
        Set_bars_to_state(true);
        StartCoroutine(Move_top_effect_bar(top_bar));
        yield return StartCoroutine(Move_bottom_effect_bar(bottom_bar));
        if(set_inactive) Set_bars_to_state(false);
    }

    private void Set_bars_to_state(bool v)
    {
        effect_bars[0].SetActive(v);
        effect_bars[1].SetActive(v);
    }

    IEnumerator Move_bottom_effect_bar(bool move_up)
    {
        GameObject bottom_bar = effect_bars[1];
        Vector3 target = new Vector3();
        if (move_up) target =  bottom_bar.transform.localPosition + new Vector3(0, 400, 0);
        else target = bottom_bar.transform.localPosition - new Vector3(0, 400, 0);
        while (Vector3.Distance(bottom_bar.transform.localPosition, target) > .1f)
        {
            bottom_bar.transform.localPosition = Vector3.MoveTowards(bottom_bar.transform.localPosition, target, 1500 * Time.deltaTime);
            yield return null;
        }
    }
    IEnumerator Move_top_effect_bar(bool move_down)
    {
        GameObject top_bar = effect_bars[0];
        Vector3 target = new Vector3();
        if (move_down) target = top_bar.transform.localPosition - new Vector3(0, 400, 0);
        else target = top_bar.transform.localPosition + new Vector3(0, 400, 0);
        while (Vector3.Distance(top_bar.transform.localPosition, target) > .1f)
        {
            top_bar.transform.localPosition = Vector3.MoveTowards(top_bar.transform.localPosition, target, 1500 * Time.deltaTime);
            yield return null;
        }
    }
    public IEnumerator Camera_Reset()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log($"[Frame:{Time.frameCount}] Camera reset started");
        while (Vector3.Distance(main_camera.transform.position, camera_starting_game_position) > 1f)
        {
            main_camera.transform.position = Vector3.SlerpUnclamped(main_camera.transform.position, camera_starting_game_position, Cam_Movement_speed / 2.5f * Time.deltaTime);
            yield return null;
        }
        gridSystem.Set_game_state_to_none();
        Pause pause = GameObject.Find("Camera_canvas").GetComponent<Pause>();
        pause.Set_can_pause(true);
        Debug.Log($"[Frame:{Time.frameCount}] Reset finished, game is playable");
    }
    public IEnumerator Camera_win_rotate(Quaternion desired_Rotation_Q, Quaternion default_Rotation_Pos, Vector3 Second_Target, Vector3 End_Target)
    {
        Debug.Log($"[Frame:{Time.frameCount}] First rotation started");
        while (Quaternion.Angle(main_camera.transform.rotation, desired_Rotation_Q) > 1 || Vector3.Distance(main_camera.transform.position, Second_Target) > 1f)
        {
            main_camera.transform.rotation = Quaternion.Slerp(main_camera.transform.rotation, desired_Rotation_Q, Cam_Rotation_speed * Time.deltaTime);
            yield return null;
        }
        Debug.Log($"[Frame:{Time.frameCount}] Second rotation started");
        while (Quaternion.Angle(main_camera.transform.rotation, default_Rotation_Pos) > .5f || Vector3.Distance(main_camera.transform.position, End_Target) > 1f)
        {
            main_camera.transform.rotation = Quaternion.Slerp(main_camera.transform.rotation, default_Rotation_Pos, Cam_Rotation_speed * 2 * Time.deltaTime);
            yield return null;
        }
        Debug.Log($"[Frame:{Time.frameCount}] Rotation ended");
    }
    public IEnumerator Camera_to_starting_position()
    {
        Pause p = GameObject.Find("Camera_canvas").GetComponent<Pause>();
        p.Set_can_pause(true);
        Vector3 first_target = new Vector3(main_camera.transform.position.x+8,main_camera.transform.position.y+2,main_camera.transform.position.z);
        Vector3 third_target = new Vector3(camera_starting_game_position.x, camera_starting_game_position.y, -40); ;
        main_camera = Camera.main;
        Debug.Log($"[{Time.frameCount}] First cam movement started");
        while (Vector3.Distance(main_camera.transform.position,first_target)> 1)
        {
            main_camera.transform.position = Vector3.LerpUnclamped(main_camera.transform.position, first_target, 5f * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(.2f);
        StartCoroutine(Camera_to_starting_rotation());
        audio_manager.Play_intro_swoosh();
        while (Vector3.Distance(main_camera.transform.position, third_target) > 1f)
        {
            main_camera.transform.position = Vector3.LerpUnclamped(main_camera.transform.position, third_target, 2f * Time.deltaTime);
            yield return null;
        }
        Debug.Log($"[{Time.frameCount}] Second cam movement started");
        while (Vector3.Distance(main_camera.transform.position, camera_starting_game_position) > 1f)
        {
            main_camera.transform.position = Vector3.LerpUnclamped(main_camera.transform.position, camera_starting_game_position, 2f * Time.deltaTime);
            yield return null;
        }
        gridSystem.Set_game_state_to_not_won();
        Debug.Log($"[{Time.frameCount}] Cam is in starting position");
    }
    public IEnumerator Camera_to_starting_rotation()
    {
        float first_rotation_angle = 180;
        Quaternion first_rotation_q = Quaternion.Euler(main_camera.transform.eulerAngles.x, first_rotation_angle, main_camera.transform.rotation.eulerAngles.z);
        Debug.Log($"[{Time.frameCount}] First rotation started");
        yield return new WaitForSeconds(.05f);
        while (Quaternion.Angle(main_camera.transform.rotation, first_rotation_q) > 1f)
        {
            main_camera.transform.rotation = Quaternion.RotateTowards(main_camera.transform.rotation, first_rotation_q, 90 * Time.deltaTime);
            yield return null;
        }
        Debug.Log($"[{Time.frameCount}] Second rotation started");
        float second_rotation_angle = 100;
        Quaternion second_rotation_q = Quaternion.Euler(main_camera.transform.eulerAngles.x, second_rotation_angle, main_camera.transform.rotation.eulerAngles.z);
        while (Quaternion.Angle(main_camera.transform.rotation, second_rotation_q) > 10f)
        {
            main_camera.transform.rotation = Quaternion.RotateTowards(main_camera.transform.rotation, second_rotation_q, 110 * Time.deltaTime);
            yield return null;
        }
        float third_rotation_angle = 0;
        Quaternion third_rotation_q = Quaternion.Euler(main_camera.transform.eulerAngles.x, third_rotation_angle, main_camera.transform.rotation.eulerAngles.z);
        Debug.Log($"[{Time.frameCount}] Third rotation started");
        while (Quaternion.Angle(main_camera.transform.rotation, third_rotation_q) > 1f)
        {
            main_camera.transform.rotation = Quaternion.RotateTowards(main_camera.transform.rotation, third_rotation_q, 90 * Time.deltaTime);
            yield return null;
        }
        Debug.Log($"[{Time.frameCount}] Cam rotation ended");
    }
}
