using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Advanced_Level_Generator : MonoBehaviour
{
    public static Advanced_Level_Generator instance;

    public GameObject levelPrefab; // Шаблон рівня
    public int levelNum;
    public float levelHeightStep = 14f; // Крок між поверхами
    public int levelsNumToGenerate = 10; // Кількість рівнів

    [HideInInspector]public GameObject player;

    public List<GameObject> levels = new List<GameObject>();

    private void Start()
    {
        instance = this;

        player = GameObject.FindWithTag("Player");

        for (int i = 0; i < levelsNumToGenerate + 1; i++)
        {
            levelNum--;
            GameObject level = Instantiate(levelPrefab,
                new Vector3(0, 7, 0) + new Vector3(
                    0, levelNum * levelHeightStep, 0),
                Quaternion.identity);
            if (levels.Count > 10)
            {
                level.SetActive(false);
            }
            levels.Add(level);
        }
    }
}
