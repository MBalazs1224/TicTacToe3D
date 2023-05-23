using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Text : MonoBehaviour
{
    // Start is called before the first frame update
    TextMesh textMesh;
    GameObject Grid_Object;
    GridSystem gridSystem;
    Cursor_controller cursor_controller;
    Audio_manager audio_manager;
    void Start()
    {
        //textMesh = GetComponent<TextMesh>();
        //textMesh.text = "0";
        Grid_Object = GameObject.FindGameObjectWithTag("GridRunner");
        gridSystem = Grid_Object.GetComponent<GridSystem>();
        transform.SetParent(Grid_Object.transform, false);
        cursor_controller = new Cursor_controller();
        audio_manager = Camera.main.GetComponent<Audio_manager>();

    }
    private void OnMouseEnter()
    {
        if (!gridSystem.Is_game_in_menu()) cursor_controller.Set_cursor_to_pointer();
    }
    private void OnMouseExit()
    {
        cursor_controller.Set_cursor_to_arrow();
    }
    private void OnMouseDown()
    {
       
        Debug.Log($"Kiválasztott helyek:{Convert.ToInt32(transform.localPosition.x)};{Convert.ToInt32(transform.localPosition.y+1)}");
        gridSystem.Grid_Value_Set(Convert.ToInt32(transform.localPosition.x), Convert.ToInt32(transform.localPosition.y + 1));
        //if (!gridSystem.Is_game_in_menu()) audio_manager.Play_button_click();
    }
}
