using System;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public int X { get; set; }     // координата в сітці по X
    public int Y { get; set; }     // координата в сітці по Y
    public int Type { get; set; }  // тип / колір гема (індекс префаба)

    public static event Action<Gem> Clicked;

    private void OnMouseDown()
    {
        Clicked?.Invoke(this);
    }
}
