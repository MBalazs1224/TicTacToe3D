using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public enum Game_state
    {
        In_Menu,
        Not_Won,
        Won_horizontal,
        Won_vertical,
        Won_diagonal_L_R,
        Won_diagonal_R_L,
        Paused,
        Draw
    }
    Camera Main_Camera;
    public Game_state current_game_state = Game_state.In_Menu;
    Camera_animations cam_anims;
    public List<Item> winning_items = new List<Item>();
    [SerializeField] TextMesh text;
    [SerializeField] public int height = 10, width = 10;
    Item[,] grid;
    public int step_counter = 99;
    public int current_Player = 1;
    [SerializeField] float Rotation_Angle = 60f;
    List<Item>[] placed_Items = new List<Item>[2];
    Canvas_Script canvas_script;
    public int[] playerscores = new int[2];
    Light sky_light;
    Light fire_light;
    Pause pause;
    Game_state before_pause;
    public bool one_player = true;
    Audio_manager audio_manager;
    [SerializeField] GameObject item_placeholder;
    GameObject[] Item_animations = new GameObject[2];
    GameObject X_parent;
    GameObject O_parent;
    void Start()
    {
        Set_camera();
        Set_variables();
        Grid_Fill();
    }
    public void Awake()
    {
        grid = new Item[width, height];
        placed_Items[0] = new List<Item>();
        placed_Items[1] = new List<Item>();
        playerscores[0] = 0;
        playerscores[1] = 0;
        text.text = "";

    }
    public void Reset_scores()
    {
        playerscores[0] = 0;
        playerscores[1] = 0;
    }
    public void Set_play_mode(bool value)
    {
        one_player = value;
    }
    public void Set_game_state_to_before_pause()
    {
        current_game_state = before_pause;
    }
    public void Set_game_state_to_not_won()
    {
        current_game_state = Game_state.Not_Won;
    }
    public void Set_game_state_to_paused()
    {
        before_pause = current_game_state;
        current_game_state = Game_state.Paused;
    }
    public void Set_game_state_to_none()
    {
        current_game_state = Game_state.Not_Won;
    }
    private void Set_camera()
    {
        Main_Camera = Camera.main;
        Main_Camera.fieldOfView = 60;
        Main_Camera.transform.rotation = Quaternion.Euler(0, -70, 0);
    }
    public bool Is_game_in_menu()
    {
        return current_game_state == Game_state.In_Menu;
    }
    public void Set_game_state_to_in_menu()
    {
        current_game_state = Game_state.In_Menu;
    }

    private void Set_variables()
    {
        GameObject obj = GameObject.Find("Canvas");
        canvas_script = obj.GetComponent<Canvas_Script>();
        Canvas canvas = obj.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        cam_anims = GetComponent<Camera_animations>();
        pause = GameObject.Find("Camera_canvas").GetComponent<Pause>();
        Item_animations[0] = GameObject.Find("X_Item");
        Item_animations[1] = GameObject.Find("O_Item");
        X_parent = GameObject.Find("X_Items");
        O_parent = GameObject.Find("O_Items");
        audio_manager = Camera.main.GetComponent<Audio_manager>();
    }

    public void Check_For_Win()
    {

        if (step_counter > 8)
        {
            if (current_game_state == Game_state.Not_Won)
            {
                int counter = 0;
                while (Item_didnt_win(counter))
                {
                    counter++;
                }
                if (counter < placed_Items[current_Player - 1].Count)
                {
                    playerscores[current_Player - 1]++;
                    Game_Won();
                    return;
                }
            }
            if (step_counter == width * height) Game_draw();
        }
        if (!one_player) Change_player();
    }

    private void CPU_turn()
    {
        Debug.Log("-----------------------------------------------------------------");
        Debug.Log("Player calculations:");
        List<Item> player_steps = Calculate_min_steps(placed_Items[0], 1);
        StringBuilder sb = new StringBuilder($"Steps to win for Player 1: ");
        foreach (Item item in player_steps)
        {
            sb.Append($"[{item.X},{item.Y}];");
        }
        Debug.Log(sb.ToString());
        List<Item> cpu_steps = null;
        if (step_counter > 1)
        {
            Debug.Log("CPU calculations:");
            cpu_steps = Calculate_min_steps(placed_Items[1], 2);
        }
        if (cpu_steps != null)
        {
            StringBuilder sb2 = new StringBuilder($"Steps to win for Player 2: ");
            foreach (Item item in cpu_steps)
            {
                sb2.Append($"[{item.X},{item.Y}];");
            }
            Debug.Log(sb2.ToString());
        }
        Change_player();
        if (cpu_steps == null)
        {
            int x = 0;
            int y = 0;
            do
            {
                x = UnityEngine.Random.Range(0, width - 1);
                y = UnityEngine.Random.Range(0, height - 1);
            } while (grid[x, y].value != 0);
            Grid_Value_Set(x, y);
        }
        else
        {
            if (cpu_steps.Count == 1 || player_steps.Count == 1)
            {
                if (cpu_steps.Count == 1) Grid_Value_Set(cpu_steps[0].X, cpu_steps[0].Y);
                else Grid_Value_Set(player_steps[0].X, player_steps[0].Y);
            }
            else if (player_steps.Count < cpu_steps.Count) Grid_Value_Set(player_steps[0].X, player_steps[0].Y);
            else Grid_Value_Set(cpu_steps[0].X, cpu_steps[0].Y);
        }
        if (current_game_state == Game_state.Not_Won)
        {
            Change_player();
        }
    }

    private void Change_player()
    {
        current_Player = 3 - current_Player;
    }

    private List<Item> Calculate_min_steps(List<Item> items, int value)
    {
        List<Item> temp = new List<Item>();
        for (int i = 0; i < 5; i++)
        {
            temp.Add(new Item(item_placeholder));
        }
        foreach (Item item in items)
        {
            Debug.Log($"Current item: [{item.X};{item.Y}]");
            Calculate_horizontal_steps(item, value, ref temp);
            Calculate_vertical_steps(item, value, ref temp);
            Calculate_diagonal_l_steps(item, value, ref temp);
            Calculate_diagonal_r_steps(item, value, ref temp);
        }
        return temp;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            current_game_state = Game_state.Not_Won;
        }
        else if (Input.GetKeyDown(KeyCode.F11))
        {
            GridReset();
            winning_items.Clear();
            placed_Items[0].Clear();
            placed_Items[1].Clear();
            step_counter = 0;
            current_Player = 1;
        }
    }
    private void Calculate_horizontal_steps(Item item, int value, ref List<Item> main)
    {
        int x = item.X;
        int y = item.Y;
        if (x + 4 < width)
        {
            if (grid[x + 1, y].value != 3 - value)
            {
                x++;
                int counter = 1;
                List<Item> temp = new List<Item>();
                List<Item> same = new List<Item>();
                while (counter < 5 && grid[x, y].value != 3 - value)
                {
                    if (grid[x, y].value == value) same.Add(grid[x, y]);
                    temp.Add(new Item(item_placeholder, value, x, y));
                    counter++;
                    x++;
                }
                if (temp.Count >= 4)
                {
                    foreach (Item s in same)
                    {
                        temp.Remove(s);
                    }
                    Debug_steps_to_console("Horizontal", temp, item);
                    if (temp.Count < main.Count)
                    {
                        main = temp;
                    }
                }
            }
        }
        x = item.X;
        if (x - 4 >= 0)
        {
            if (grid[x - 1, y].value != 3 - value)
            {
                x--;
                int counter = 1;
                List<Item> temp = new List<Item>();
                List<Item> same = new List<Item>();
                while (counter < 5 && grid[x, y].value != 3 - value)
                {
                    if (grid[x, y].value == value) same.Add(grid[x, y]);
                    temp.Add(new Item(item_placeholder, value, x, y));
                    counter++;
                    x--;
                }
                if (temp.Count >= 4)
                {
                    foreach (Item s in same)
                    {
                        temp.Remove(s);
                    }
                    Debug_steps_to_console("Horizontal", temp, item);
                    if (temp.Count < main.Count)
                    {
                        main = temp;
                    }
                }
            }

        }
    }
    private void Calculate_vertical_steps(Item item, int value, ref List<Item> main)
    {
        int x = item.X;
        int y = item.Y;
        if (y - 4 >= 0)
        {
            if (grid[x, y - 1].value != 3 - value)
            {
                y--;
                List<Item> temp = new List<Item>();
                List<Item> same = new List<Item>();
                int counter = 1;
                while (counter < 5 && grid[x, y].value != 3 - value)
                {
                    if (grid[x, y].value == value) same.Add(grid[x, y]);
                    temp.Add(new Item(item_placeholder, value, x, y));
                    counter++;
                    y--;
                }
                if (temp.Count < 4) return;
                foreach (Item s in same)
                {
                    temp.Remove(s);
                }
                Debug_steps_to_console("Vertical", temp, item);
                if (temp.Count < main.Count)
                {
                    main = temp;
                }
            }
        }
        y = item.Y;
        if (y + 4 < height)
        {
            if (grid[x, y + 1].value != 3 - value)
            {
                y++;
                List<Item> temp = new List<Item>();
                List<Item> same = new List<Item>();
                int counter = 1;
                while (counter < 5 && grid[x, y].value != 3 - value)
                {
                    if (grid[x, y].value == value) same.Add(grid[x, y]);
                    temp.Add(new Item(item_placeholder, value, x, y));
                    counter++;
                    y++;
                }
                if (temp.Count < 4) return;
                foreach (Item s in same)
                {
                    temp.Remove(s);
                }
                Debug_steps_to_console("Vertical", temp, item);
                if (temp.Count < main.Count)
                {
                    main = temp;
                }
            }
        }
    }
    private void Calculate_diagonal_l_steps(Item item, int value, ref List<Item> main)
    {
        int x = item.X;
        int y = item.Y;
        if (x - 4 >= 0 && y + 4 < height)
        {
            if (grid[x - 1, y + 1].value != 3 - value)
            {
                x--;
                y++;
                List<Item> temp = new List<Item>();
                List<Item> same = new List<Item>();
                int counter = 1;
                while (counter < 5 && grid[x, y].value != 3 - value)
                {
                    if (grid[x, y].value == value) same.Add(grid[x, y]);
                    counter++;
                    temp.Add(new Item(item_placeholder, value, x, y));
                    x--;
                    y++;
                }
                if (temp.Count >= 4)
                {
                    foreach (Item s in same)
                    {
                        temp.Remove(s);
                    }
                    Debug_steps_to_console("DiagonalLR", temp, item);
                    if (temp.Count < main.Count)
                    {
                        main = temp;
                    }
                }
            }
        }
        x = item.X;
        y = item.Y;
        if (x - 4 >= 0 && y - 4 >= 0)
        {
            if (grid[x - 1, y - 1].value != 3 - value)
            {
                x--;
                y--;
                List<Item> temp = new List<Item>();
                List<Item> same = new List<Item>();
                int counter = 1;
                while (counter < 5 && grid[x, y].value != 3 - value)
                {
                    if (grid[x, y].value == value) same.Add(grid[x, y]);
                    counter++;
                    temp.Add(new Item(item_placeholder, value, x, y));
                    x--;
                    y--;
                }
                if (temp.Count >= 4)
                {
                    foreach (Item s in same)
                    {
                        temp.Remove(s);
                    }
                    Debug_steps_to_console("DiagonalLR", temp, item);
                    if (temp.Count < main.Count)
                    {
                        main = temp;
                    }
                }
            }

        }
    }
    private void Calculate_diagonal_r_steps(Item item, int value, ref List<Item> main)
    {
        int x = item.X;
        int y = item.Y;
        if (x + 4 < width && y + 4 < height)
        {
            if (grid[x + 1, y + 1].value != 3 - value)
            {
                x++;
                y++;

                List<Item> temp = new List<Item>();
                List<Item> same = new List<Item>();
                int counter = 1;
                while (counter < 5 && grid[x, y].value != 3 - value)
                {
                    if (grid[x, y].value == value) same.Add(grid[x, y]);
                    temp.Add(new Item(item_placeholder, value, x, y));
                    counter++;
                    x++;
                    y++;
                }
                if (temp.Count >= 4)
                {
                    foreach (Item s in same)
                    {
                        temp.Remove(s);
                    }
                    Debug_steps_to_console("DiagonalRL", temp, item);
                    if (temp.Count < main.Count)
                    {
                        main = temp;
                    }
                }
            }
        }
        x = item.X;
        y = item.Y;
        if (x + 4 < width && y - 4 >= 0)
        {
            if (grid[x + 1, y - 1].value != 3 - value)
            {
                x++;
                y--;
                List<Item> temp = new List<Item>();
                List<Item> same = new List<Item>();
                int counter = 1;
                while (counter < 5 && grid[x, y].value != 3 - value)
                {
                    if (grid[x, y].value == value) same.Add(grid[x, y]);
                    counter++;
                    temp.Add(new Item(item_placeholder, value, x, y));
                    x++;
                    y--;
                }
                if (temp.Count >= 4)
                {
                    foreach (Item s in same)
                    {
                        temp.Remove(s);
                    }
                    Debug_steps_to_console("DiagonalRL", temp, item);
                    if (temp.Count < main.Count)
                    {
                        main = temp;
                    }
                }
            }
        }
    }

    void Debug_steps_to_console(string name, List<Item> items, Item starting)
    {
        StringBuilder sb = new StringBuilder($"{name}:");
        items.ForEach(x => sb.Append($"[{x.X};{x.Y}]"));
        sb.Append($" starting item: [{starting.X};{starting.Y}]");
        Debug.Log(sb.ToString());
    }





    //private void Calculate_horizontal_steps(List<Item> items, int value, ref List<Item> main)
    //{
    //    List<Item> horizontal = new List<Item>();
    //    bool first = true;
    //    foreach (Item item in items)
    //    {
    //        if (item.X + 4 <= width)
    //        {
    //            List<Item> temp = new List<Item>();
    //            int counter = 1;
    //            int x = item.X;
    //            int y = item.Y;
    //            while (counter < 5 && (grid[x, y].value == 0 || grid[x,y].value == value))
    //            {
    //                temp.Add(new Item(text, value, x, y));
    //                counter++;
    //                x++;
    //            }
    //            if (first && temp.Count != 0)
    //            {
    //                horizontal = temp;
    //                first = false;
    //            }
    //            else if (horizontal.Count != 0 && temp.Count <= horizontal.Count)
    //            {
    //                horizontal = temp;
    //            }
    //        }
    //    }
    //    if ( horizontal.Count != 0 && horizontal.Count < main.Count)
    //    {
    //        main = horizontal;
    //    }
    //}


    private bool Item_didnt_win(int counter)
    {
        return counter < placed_Items[current_Player - 1].Count && (!Horizontal_Check(placed_Items[current_Player - 1][counter]) && !Vertical_Check(placed_Items[current_Player - 1][counter]) && !Diagonal_L_R_Check(placed_Items[current_Player - 1][counter]) && !Diagonal_R_L_Check(placed_Items[current_Player - 1][counter]));
    }

    private void Game_draw()
    {
        current_game_state = Game_state.Draw;
        playerscores[0]++;
        playerscores[1]++;
        canvas_script.Panel_Set(grid[3, 4].placeholder.transform.position, playerscores, true);
        StartCoroutine(cam_anims.Camera_Draw_Animation(grid[5, 5].placeholder.transform.position + new Vector3(0, 0, -3)));
    }
    private void Game_Won()
    {
        Debug.Log($"[Frame:{Time.frameCount}] Player {current_Player} won - Win method: {current_game_state}");
        pause.Set_can_pause(false);
        Camera_Movement();
    }

    void Camera_Movement()
    {
        Quaternion desired_Rotation_Q = new Quaternion();
        Quaternion default_Rotation_Pos = new Quaternion();
        Vector3 First_Target = new Vector3();
        Vector3 Second_Target = new Vector3();
        Vector3 Last_Target = new Vector3();
        Vector3 Panel_Target = new Vector3();

        Deciding_Camera_Targets(ref desired_Rotation_Q, ref default_Rotation_Pos, ref First_Target, ref Second_Target, ref Last_Target, ref Panel_Target);

        StartCoroutine(cam_anims.Camera_win_anim(First_Target, Second_Target, Last_Target, Panel_Target, playerscores));
        StartCoroutine(cam_anims.Camera_win_rotate(desired_Rotation_Q, default_Rotation_Pos, Second_Target, Last_Target));
    }

    private void Deciding_Camera_Targets(ref Quaternion desired_Rotation_Q, ref Quaternion default_Rotation_Pos, ref Vector3 First_Target, ref Vector3 Second_Target, ref Vector3 Last_Target, ref Vector3 Panel_Target)
    {
        desired_Rotation_Q = Quaternion.Euler(Main_Camera.transform.eulerAngles.x, Rotation_Angle, Main_Camera.transform.eulerAngles.z);
        default_Rotation_Pos = Main_Camera.transform.rotation;
        switch (current_game_state)
        {

            case Game_state.Not_Won:
                break;
            case Game_state.Won_horizontal:
                First_Target = new Vector3(winning_items[0].X - 1f, winning_items[0].Y - .8f, -2.5f);
                Second_Target = new Vector3(winning_items[4].X + 1.5f, winning_items[4].Y - .8f, -2.5f);
                if (winning_items[1].Y < 3)
                {
                    Last_Target = new Vector3(winning_items[2].X, winning_items[2].Y + .8f, -7);
                    Panel_Target = new Vector3(winning_items[2].X - 1.7f, winning_items[2].Y, -1);
                }
                else
                {
                    Last_Target = new Vector3(winning_items[2].X, winning_items[2].Y - 2.5f, -7);
                    Panel_Target = new Vector3(winning_items[2].X - 1.7f, winning_items[2].Y - 4f, -1);
                }

                break;
            case Game_state.Won_vertical:

                First_Target = new Vector3(winning_items[0].X - .8f, winning_items[0].Y, -2);
                Second_Target = new Vector3(winning_items[4].X - .8f, winning_items[4].Y - 2f, -2);
                if (winning_items[1].X >= width - 3)
                {
                    Last_Target = new Vector3(winning_items[2].X - 3f, winning_items[2].Y, -7);
                    Panel_Target = new Vector3(winning_items[2].X - 5f, winning_items[2].Y - 2f, -1f);
                }
                else
                {
                    Last_Target = new Vector3(winning_items[2].X + 3f, winning_items[2].Y, -7);
                    Panel_Target = new Vector3(winning_items[2].X + 2f, winning_items[2].Y - 2f, -1f);
                }

                break;
            case Game_state.Won_diagonal_L_R:
                First_Target = new Vector3(winning_items[0].X - 1f, winning_items[0].Y, -2);
                Second_Target = new Vector3(winning_items[4].X + 1f, winning_items[4].Y - 2f, -2);
                if (winning_items[0].X <= 3)
                {
                    Last_Target = new Vector3(winning_items[2].X + 3f, winning_items[2].Y, -7);
                    Panel_Target = new Vector3(winning_items[2].X + 1.5f, winning_items[2].Y - 1f, -1);
                }
                else
                {
                    Last_Target = new Vector3(winning_items[2].X - 1f, winning_items[2].Y - 1f, -7);
                    Panel_Target = new Vector3(winning_items[2].X - 4f, winning_items[2].Y - 3f, -1);
                }
                break;
            case Game_state.Won_diagonal_R_L:

                First_Target = new Vector3(winning_items[0].X, winning_items[0].Y, -1);
                Second_Target = new Vector3(winning_items[4].X - 1, winning_items[4].Y - 1.5f, -2);
                if (winning_items[0].X >= width - 3)
                {
                    Last_Target = new Vector3(winning_items[2].X - 1f, winning_items[2].Y, -7);
                    Panel_Target = new Vector3(winning_items[2].X - 4f, winning_items[2].Y - 1f, -1);

                }
                else
                {
                    Last_Target = new Vector3(winning_items[2].X + 1f, winning_items[2].Y, -7);
                    Panel_Target = new Vector3(winning_items[2].X + 1f, winning_items[2].Y - 2f, -1);
                }
                break;
            default:
                break;
        }
    }
    private bool Diagonal_R_L_Check(Item item)
    {
        if (item.X - 4 >= 0 && item.Y - 4 >= 0)
        {
            int count = 0;
            int y_Counter = item.Y;
            int x_Counter = item.X;
            while (count < 5 && grid[x_Counter, y_Counter].value == current_Player)
            {
                winning_items.Add(new Item(item_placeholder, current_Player, x_Counter, y_Counter));
                y_Counter--;
                x_Counter--;
                count++;
            }
            if (count == 5)
            {
                current_game_state = Game_state.Won_diagonal_R_L;
            }
            else
            {
                winning_items.Clear();
            }
            return count == 5;
        }
        else return false;
    }
    private bool Diagonal_L_R_Check(Item item)
    {
        if (item.X + 4 < width && item.Y - 4 >= 0)
        {
            int count = 0;
            int y_Counter = item.Y;
            int x_Counter = item.X;
            while (count < 5 && grid[x_Counter, y_Counter].value == current_Player)
            {
                winning_items.Add(new Item(item_placeholder, current_Player, x_Counter, y_Counter));
                y_Counter--;
                x_Counter++;
                count++;
            }
            if (count == 5)
            {
                current_game_state = Game_state.Won_diagonal_L_R;
            }
            else
            {
                winning_items.Clear();
            }
            return count == 5;
        }
        else return false;

    }

    private bool Vertical_Check(Item item)
    {
        if (item.Y - 4 >= 0)
        {
            int count = 0;
            int y_Counter = item.Y;
            while (count < 5 && grid[item.X, y_Counter].value == current_Player)
            {
                winning_items.Add(new Item(item_placeholder, current_Player, item.X, y_Counter));
                y_Counter--;
                count++;
            }
            if (count == 5)
            {
                current_game_state = Game_state.Won_vertical;
            }
            else
            {
                winning_items.Clear();
            }
            return count == 5;
        }
        else return false;
    }

    private bool Horizontal_Check(Item item)
    {
        if (item.X + 4 < width)
        {
            int count = 0;
            int x_Counter = item.X;
            while (count < 5 && grid[x_Counter, item.Y].value == current_Player)
            {
                winning_items.Add(new Item(item_placeholder, current_Player, x_Counter, item.Y));
                x_Counter++;
                count++;
            }
            if (count == 5)
            {
                current_game_state = Game_state.Won_horizontal;
            }
            else
            {
                winning_items.Clear();
            }
            return count == 5;
        }
        else return false;
    }
    private void Grid_Fill()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject parent = GameObject.Find("Items");
                grid[x, y] = new Item(Instantiate(item_placeholder, new Vector3(x + 2.6f, y - .3f, -.2f), Quaternion.identity, parent.transform), 0, x, y);
            }
        }
    }
    public void Grid_Value_Set(int x, int y)
    {
        if (grid[x, y].value == 0 && current_game_state == Game_state.Not_Won)
        {
            grid[x, y].X = x;
            grid[x, y].Y = y;
            grid[x, y].value = current_Player;
            placed_Items[current_Player - 1].Add(grid[x, y]);
            step_counter++;
            Transform parent = current_Player == 1 ? X_parent.transform : O_parent.transform;
            Debug.Log($"[Frame:{Time.frameCount}] Placed item for Player {current_Player} at [{x},{y}]");
            GameObject new_item = Instantiate(Item_animations[current_Player - 1], new Vector3(x - .5f + 1, y - .5f, -.5f), Quaternion.identity, parent);
            audio_manager.Play_drawing();
            string item_name = current_Player == 1 ? $"X_{placed_Items[0].Count}" : $"O_{placed_Items[1].Count}";
            new_item.name = item_name;
            Check_For_Win();
            if (one_player && current_Player == 1 && current_game_state == Game_state.Not_Won)
            {
                CPU_turn();
            }
        }
    }
    public void Restart()
    {
        audio_manager.Stop_playing_effect();
        Game_reset();
        StartCoroutine(cam_anims.Camera_Reset());
    }

    public void GridReset()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y].value = 0;
                grid[x, y].X = 0;
                grid[x, y].Y = 0;
            }
        }
        for (int i = 0; i < X_parent.transform.childCount; i++)
        {
            Destroy(X_parent.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < O_parent.transform.childCount; i++)
        {
            Destroy(O_parent.transform.GetChild(i).gameObject);
        }
    }
    public void Back_to_main_menu()
    {
        audio_manager.Stop_playing_effect();
        Game_reset();
        StartCoroutine(cam_anims.Back_to_main_menu(true));
        Main_menu_Script mms = GameObject.Find("Main_menu_canvas").GetComponent<Main_menu_Script>();
        mms.Activate_main_menu();
        current_game_state = Game_state.In_Menu;
        current_Player = 1;
        Reset_scores();
    }

    private void Game_reset()
    {
        Debug.Log($"[Frame:{Time.frameCount}] Back to main menu");
        GridReset();
        StartCoroutine(canvas_script.Panel_Reset());
        Draw_line draw_Line = GameObject.Find("GridRunner").GetComponent<Draw_line>();
        StartCoroutine(draw_Line.Draw_Line_BackWards());
        winning_items.Clear();
        placed_Items[0].Clear();
        placed_Items[1].Clear();
        step_counter = 0;
        current_Player = 1;
    }
}
