using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public GameObject placeholder { get; set; }
    public int value { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Item(GameObject obj, int value = 0,int x = 0, int y = 0)
    {
        placeholder = obj;
        this.value = value;
        X= x;
        Y= y;
    }
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (!(obj is Item)) return false;
        return this.X == ((Item)obj).X && this.Y == ((Item)obj).Y;
    }
    public override int GetHashCode()
    {
        return this.GetHashCode();
    }
}