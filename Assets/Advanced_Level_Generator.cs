using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Advanced_Level_Generator : MonoBehaviour
{
    public GameObject levelPrefab; // Шаблон рівня
    public int levelNum;
    public float levelHeightStep = 14f; // Крок між поверхами
    public int levelsNumToGenerate = 10; // Кількість рівнів

    private void Start()
    {
        for (int i = 0; i < levelsNumToGenerate + 1; i++)
        {
            levelNum--;
            Instantiate(levelPrefab,
                new Vector3(0, 7, 0) + new Vector3(
                    0, levelNum * levelHeightStep, 0),
                Quaternion.identity);
        }
    }
}
