using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Resolution_full { get; set; }
    public int Res_width { get; set; }
    public int Res_height { get; set; }
    public string resolution_full { get; }
    public int Quality_index { get; set; }
    public bool Fullscreen { get; set; }
    public float Music_volume { get; set; }
    public float Effect_volume { get; set; }

    public User(string data)
    {
        CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.NumberFormat.NumberDecimalSeparator = ".";
        culture.NumberFormat.NumberGroupSeparator = ".";
        CultureInfo.CurrentCulture = culture;
        string[] temp = data.Split('/');
        Id = temp[0];
        Username = temp[1];
        string[] res = temp[2].Split('x');
        Res_width = int.Parse(res[0]);
        Res_height = int.Parse(res[1]);
        Resolution_full = $"{Res_width} x {Res_height}";
        Quality_index = int.Parse(temp[3]);
        Fullscreen = temp[4] == "1" ? true : false;
        Music_volume = float.Parse((temp[5]));
        Effect_volume = float.Parse((temp[6]));
    }
}