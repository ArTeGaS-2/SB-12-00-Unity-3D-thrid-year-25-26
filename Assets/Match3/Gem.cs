using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Type { get; set; }

    public static event Action<Gem> Clicked;

    private void OnMouseDown()
    {
        Clicked?.Invoke(this);
    }
}
