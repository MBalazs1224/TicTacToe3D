using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Canvas_Script : MonoBehaviour
{
    Canvas canvas;
    GameObject panel;
    GridSystem gridsystem;
    TextMeshPro restart_text;
    TextMeshPro exit_text;
    Audio_manager am;
    void Start()
    {
        Set_variables();
        Set_language();
        panel.SetActive(false);
    }

    private void Set_language()
    {
        Languages langs = GameObject.Find("Language_holder").GetComponent<Languages>();
        restart_text.text = langs.Return_language_string("Restart");
        exit_text.text = langs.Return_language_string("Exit");
    }

    private void Set_variables()
    {
        GameObject obj = GameObject.Find("Restart_button_text");
        restart_text = obj.GetComponent<TextMeshPro>();
        exit_text = GameObject.Find("Exit_button_text").GetComponent<TextMeshPro>();
        canvas = GetComponent<Canvas>();
        panel = GameObject.Find("Panel");
        gridsystem = GameObject.Find("GridRunner").GetComponent<GridSystem>();
        panel.transform.localPosition = new Vector3(0, 0, -1.1f);
        am = Camera.main.GetComponent<Audio_manager>();
    }

    public void Panel_Set(Vector3 position, int[] scores, bool draw = false)
    {
        panel.transform.position = new Vector3(position.x, position.y, -5f);
        StartCoroutine(Panel_Drop_Animation(position, scores, draw));
    }
    IEnumerator Panel_Drop_Animation(Vector3 Position, int[] scores, bool draw)
    {
        panel.SetActive(true);
        Vector3 target_Position = new Vector3(Position.x, Position.y, -.5f);
        //yield return new WaitForSeconds(3);
        Languages langs = GameObject.Find("Language_holder").GetComponent<Languages>();
        string text = "";
        if (draw)
        {
            text = langs.Return_language_string("Draw");
        }
        else
        {
            //int winner = gridsystem.one_player ? 3 - gridsystem.current_Player : gridsystem.current_Player;
            //text = $"{langs.Return_language_string("Player")} {winner} {langs.Return_language_string("Wins")}";
            if (gridsystem.one_player)
            {
                text = gridsystem.current_Player == 1 ? langs.Return_language_string("Player_1_wins") : langs.Return_language_string("Player_2_wins");
            }
            else
            {
                text = gridsystem.current_Player == 1 ? langs.Return_language_string("Player_1_wins") : langs.Return_language_string("Player_2_wins");
            }
        }
        GameObject.Find("Win_Text").GetComponent<TextMesh>().text = text;
        string score_text = $"{langs.Return_language_string("Player_1")}  :\t{scores[0]}-\t{scores[1]}: {langs.Return_language_string("Player_2")}";
        GameObject.Find("Scores_Text").GetComponent<TextMesh>().text = score_text;
        am.Play_cheering();
        while (Vector3.Distance(panel.transform.position, target_Position) > 1f)
        {
            panel.transform.position = Vector3.Lerp(panel.transform.position, target_Position, 1.5f * Time.deltaTime);
            yield return null;
        }
        Debug.Log($"[Frame:{Time.frameCount}] Panel finished dropping");
    }
    public IEnumerator Panel_Reset()
    {
        Vector3 target = new Vector3(panel.transform.position.x, panel.transform.position.y, -30);
        Debug.Log($"[Frame:{Time.frameCount}] Panel started moving backwards");
        while (Vector3.Distance(panel.transform.position, target) > 1f)
        {
            panel.transform.position = Vector3.MoveTowards(panel.transform.position, target, 20f * Time.deltaTime);
            yield return null;
        }
        panel.SetActive(false);
        Debug.Log($"[Frame:{Time.frameCount}] Panel disappeared");
    }
}
