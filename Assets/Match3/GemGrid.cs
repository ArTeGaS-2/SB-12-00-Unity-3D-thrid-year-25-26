using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemGrid : MonoBehaviour
{
    public static GemGrid Instance;

    public int width = 9;
    public int length = 9;
    public float gemStep = 1.1f;

    public GameObject gemPlaceholder;
    public List<GameObject> gemPrefabs;

    public List<List<GameObject>> grid;

    private void Start()
    {
        Instance = this;

        grid = new List<List<GameObject>>();
    }
}
