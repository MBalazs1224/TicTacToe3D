using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class X_Coordinate_Comparer : IComparer<Item>
{
    public int Compare(Item x, Item y)
    {
        if (x.X == y.X)
        {
            return 0;
        }
        else if (x.X > y.X)
        {
            return 1;
        } 
        return -1;
    }
}
