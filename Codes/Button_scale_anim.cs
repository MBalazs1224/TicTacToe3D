using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Button_scale_anim : MonoBehaviour
{
    Main_menu_Script script;
    bool scaling = false;
    float end_x;
    float end_y;
    float start_x;
    float start_y;
    Button button;
    Audio_manager audio_Manager;
    float scaling_speed;
    Pause pause;
    Cursor_controller cursor_controller;
    void Start()
    {
        Set_variables();
    }

    private void Set_variables() 
    {
        GameObject go = GameObject.Find("Main_menu_canvas");
        pause = GameObject.Find("Camera_canvas").GetComponent<Pause>();
        cursor_controller = new Cursor_controller();
        cursor_controller.Set_cursor_to_arrow();
        if (go != null)
        {
            start_x = this.gameObject.transform.localScale.x;
            end_x = this.gameObject.transform.localScale.x + .2f;
            start_y = this.gameObject.transform.localScale.y;
            end_y = this.gameObject.transform.localScale.y + .2f;
            button = this.gameObject.GetComponent<Button>();
            scaling_speed = .03f;
        }
        else
        {
            start_x = this.gameObject.transform.localScale.x;
            end_x = this.gameObject.transform.localScale.x + .05f;
            start_y = this.gameObject.transform.localScale.y;
            end_y = this.gameObject.transform.localScale.y + .05f;
            button = this.gameObject.GetComponent<Button>();
            scaling_speed = .001f;
        }
        //script = GameObject.Find("Main_menu_canvas").GetComponent<Main_menu_Script>();
       
        audio_Manager = Camera.main.GetComponent<Audio_manager>();
    }
    private void OnLevelWasLoaded(int level)
    {
        Set_variables();
    }
    private void OnMouseEnter()
    {
        if (GetComponent<Button>().interactable && !pause.menu_shown)
        {
            audio_Manager.Play_button_hover();
            cursor_controller.Set_cursor_to_pointer();
        }
    }
    private void OnMouseOver()
    {
        Button_scale_up();
    }
    private void OnMouseDown()
    {
        if (GetComponent<Button>().interactable && !pause.menu_shown)
        {
            audio_Manager.Play_button_click();
        }
    }
    private void OnMouseExit()
    {
        StartCoroutine(Button_scale_down());
        cursor_controller.Set_cursor_to_arrow();
    }
    IEnumerator Button_scale_down()
    {
        scaling = true;
        while (start_x < button.transform.localScale.x && start_y < button.transform.localScale.y)
        {
            button.transform.localScale -= new Vector3(scaling_speed, scaling_speed, 0);
            yield return null;
        }
        scaling = false;
    }
    void Button_scale_up()
    {
        if (!pause.menu_shown && !scaling && button.interactable)
        {
            if (end_x > button.transform.localScale.x && end_y > button.transform.localScale.y)
            {
                button.transform.localScale += new Vector3(scaling_speed + 1 * Time.deltaTime, scaling_speed + 1 * Time.deltaTime, 0);
            }
        }
    }
}