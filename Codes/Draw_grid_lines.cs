using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class Draw_grid_lines : MonoBehaviour
{
    GameObject line_parent;
    GridSystem grid_system;
    [SerializeField] Material white_material;
    void Start()
    {
        line_parent = GameObject.Find("Lines");
        grid_system = GameObject.Find("GridRunner").GetComponent<GridSystem>();
        Draw_lines();
    }
    private void Draw_lines()
    {
        Draw_horizontal_lines();
        Draw_vertical_lines();
    }

    private void Draw_vertical_lines()
    {
        int height = grid_system.height;
        for (int i = -1; i < grid_system.width; i++)
        {
            GameObject go = new GameObject();
            go.name = $"Line Vertical({i})";
            go.layer = 3;
            LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
            go.transform.SetParent(line_parent.transform);
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
            lineRenderer.numCapVertices = 90;
            lineRenderer.material = white_material;
            lineRenderer.startWidth = .1f;
            lineRenderer.endWidth = .1f;
            lineRenderer.alignment = LineAlignment.TransformZ;
            lineRenderer.SetPosition(0, new Vector3(i + 1, -1f, -.51f));
            lineRenderer.SetPosition(1, new Vector3(i + 1, height - 1, -.51f));
            lineRenderer.enabled = true;
        }
    }
    private void Draw_horizontal_lines()
    {
        int width = grid_system.width;
        for (int i = 0; i < grid_system.height + 1; i++)
        {
            GameObject go = new GameObject();
            go.name = $"Line Horizontal({i})";
            go.layer = 3;
            LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
            go.transform.SetParent(line_parent.transform);
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
            lineRenderer.numCapVertices = 90;
            lineRenderer.material = white_material;
            lineRenderer.startWidth = .1f;
            lineRenderer.endWidth = .1f;
            lineRenderer.alignment = LineAlignment.TransformZ;
            lineRenderer.SetPosition(0, new Vector3(0, i - 1, -.51f));
            lineRenderer.SetPosition(1, new Vector3(width, i - 1, -.51f));
            lineRenderer.enabled = true;
        }
    }
}
