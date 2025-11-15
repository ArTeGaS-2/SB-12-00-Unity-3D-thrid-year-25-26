using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemGrid : MonoBehaviour
{
    public static GemGrid Instance;

    public int width = 9; // ширина сітки
    public int length = 9; // довжина сітки
    public float gemStep = 1.1f; // відстань між гемами

    public GameObject gemPlaceholder; // заглушка для гемів що знищені
    public List<GameObject> gemPrefabs; // префаби гемів

    public List<List<GameObject>> grid; // сітка гемів на сцені

    private void Start()
    {
        Instance = this;

        grid = new List<List<GameObject>>();

        // Заповнюємо початкову сітку гемів
        for (int i = 0; i < length; i++)
        {
            List<GameObject> row = new List<GameObject>();
            for (int j = 0; j < width; j++)
            {
                Vector3 position = new Vector3(j * gemStep, i * gemStep, 0);
                GameObject gem = Instantiate(
                    gemPrefabs[Random.Range(0, gemPrefabs.Count)],
                    position, Quaternion.identity);
                row.Add(gem);
            }
            grid.Add(row);
        }
    }
}
