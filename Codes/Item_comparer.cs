using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_comparer : IEqualityComparer<Item>
{
    public bool Equals(Item one, Item two)
    {
        return one.X == two.X && one.Y == two.Y;
    }

    public int GetHashCode(Item one)
    {
        return one.GetHashCode();
    }
}