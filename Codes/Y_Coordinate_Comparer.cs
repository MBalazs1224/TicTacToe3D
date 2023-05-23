using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_Coordinate_Comparer : IComparer<Item>
{
    // Start is called before the first frame update
    public int Compare(Item x, Item y)
    {
        if (x.Y == y.Y)
        {
            return 0;
        }
        else if (x.Y > y.Y)
        {
            return 1;
        }
        return -1;
    }
}
