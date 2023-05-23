using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw_line : MonoBehaviour
{
    LineRenderer lineRenderer;
    List<Vector3> line_points = new List<Vector3>();
    [SerializeField] float line_Draw_Speed = 6;
   
    void Start()
    {
        lineRenderer = GameObject.Find("GridRunner").GetComponent<LineRenderer>();

    }
    public IEnumerator Draw_Line_BackWards()
    {
        int counter = line_points.Count - 2;
        Debug.Log($"[Frame:{Time.frameCount}] Draw line backwards started");
        while (counter >= 0)
        {
            lineRenderer.SetPosition(1, line_points[counter]);
            counter -= 2;
            yield return null;
        }
        lineRenderer.enabled = false;
        Debug.Log($"[Frame:{Time.frameCount}] Draw line backwards finished");
        line_points.Clear();
    }
    public IEnumerator Draw_Line()
    {
        GridSystem gridSystem = GameObject.Find("GridRunner").GetComponent<GridSystem>();
        GridSystem.Game_state winning_method = gridSystem.current_game_state;
        List<Item> winning_items = gridSystem.winning_items;
        Vector3 First_Target = new Vector3();
        Vector3 Second_Target = new Vector3();
        switch (winning_method)
        {
            case GridSystem.Game_state.Not_Won:
                break;
            case GridSystem.Game_state.Won_horizontal:
                First_Target = new Vector3(winning_items[0].X, winning_items[0].Y - .5f, -1.1f);
                Second_Target = new Vector3(winning_items[4].X + .7f, winning_items[4].Y - .5f, -1.1f);
                break;
            case GridSystem.Game_state.Won_vertical:
                First_Target = new Vector3(winning_items[0].X + .5f, winning_items[0].Y, -.9f);
                Second_Target = new Vector3(winning_items[4].X + .5f, winning_items[4].Y - 1.5f, -.9f);
                break;
            case GridSystem.Game_state.Won_diagonal_L_R:
                First_Target = new Vector3(winning_items[0].X +.4f, winning_items[0].Y - .2f, -.9f);
                Second_Target = new Vector3(winning_items[4].X + .8f, winning_items[4].Y - 1f, -.9f);
                break;
            case GridSystem.Game_state.Won_diagonal_R_L:
                First_Target = new Vector3(winning_items[0].X + 1f, winning_items[0].Y, -.9f);
                Second_Target = new Vector3(winning_items[4].X, winning_items[4].Y - 1f, -.9f);
                break;

        }
        float counter = 0;
        float Distance = Vector3.Distance(First_Target, Second_Target);
        lineRenderer.SetPosition(0, First_Target);
        line_points.Add(First_Target);
        lineRenderer.enabled = true;
        lineRenderer.material = Resources.Load<Material>("Material/White");
        Debug.Log($"[Frame:{Time.frameCount}] Draw line started");
        while (counter < Distance)
        {
            counter += .3f / line_Draw_Speed;
            float x = Mathf.Lerp(0, Distance, counter);
            Vector3 where_draw_line = x * Vector3.Normalize(Second_Target - First_Target) + First_Target;
            lineRenderer.SetPosition(1, where_draw_line);
            if (!line_points.Contains(where_draw_line))
            {
                line_points.Add(where_draw_line);
            }
            yield return null;
        }
        Debug.Log($"[Frame:{Time.frameCount}] Draw line finished");
    }
}
