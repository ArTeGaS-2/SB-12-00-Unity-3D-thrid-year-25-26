using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [Header("Сітка")]
    public int width = 9; // ширина сітки
    public int lenght = 9; // довжина сітки
    public float gemStep = 1.1f; // відстань між гемами

    [Header("Геми")]
    public List<GameObject> gemPrefabs; // префаби гемів

    public GameObject[,] Grid;

    private void Awake()
    {
        Grid = new GameObject[width, lenght];
    }
    private void Start()
    {
        for (int y = 0; y < lenght; y++)
        {
            for (int x = 0; x < width; x++)
            {
                SpawnGem(x,y);
            }
        }
    }
    private GameObject SpawnGem(int x, int y)
    {
        int type = Random.Range(0, gemPrefabs.Count);
        Vector3 position = new Vector3(
            x * gemStep, y * gemStep, 0);
        GameObject obj = Instantiate(gemPrefabs[type], position,
            Quaternion.identity, transform);
        Grid[x, y] = obj;
        return obj;
    }
}
