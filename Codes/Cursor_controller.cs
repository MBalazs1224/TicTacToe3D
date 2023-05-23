using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor_controller
{
    Texture2D cursor_arrow = Resources.Load<Texture2D>("Cursors/red_arrow_cursor");
    Texture2D cursor_pointer = Resources.Load<Texture2D>("Cursors/red_pointer_cursor");
    public void Set_cursor_to_arrow()
    {
        Cursor.SetCursor(cursor_arrow, Vector2.zero, CursorMode.Auto);
    }
    public void Set_cursor_to_pointer()
    {
        Cursor.SetCursor(cursor_pointer, new Vector2(100f, 0), CursorMode.Auto);
    }
}
