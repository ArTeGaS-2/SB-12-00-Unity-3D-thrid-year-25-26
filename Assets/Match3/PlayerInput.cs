using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] BoardController board;

    Gem firstSelection;

    private void OnEnable()
    {
        Gem.Clicked += OnGemClicked;
    }
    private void OnDisable()
    {
        Gem.Clicked -= OnGemClicked;
    }
    void OnGemClicked(Gem gem)
    {
        if (board == null) return;

        if (firstSelection == null)
        {
            firstSelection = gem;
            Debug.Log($"First selected: ({gem.X}, {gem.Y})");
        }
        else if (firstSelection == gem)
        {
            Debug.Log("Selection canceled");
            firstSelection = null;
        }
        else
        {
            Debug.Log($"First selected: ({gem.X}, {gem.Y})");
            firstSelection = null;
        }
    }
}
