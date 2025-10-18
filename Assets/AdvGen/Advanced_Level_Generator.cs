using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Advanced_Level_Generator : MonoBehaviour
{
    public static Advanced_Level_Generator instance;
    [Header("Елементи рівня")]
    public GameObject levelPrefab; // Шаблон рівня
    public int levelNum;
    public float levelHeightStep = 14f; // Крок між поверхами
    public int levelsNumToGenerate = 10; // Кількість рівнів
    [Header("Інше")]
    public GameObject shadowObj; // Об'єкт тіні/скрімера
    public List<GameObject> anotherLevels
        = new List<GameObject>(); // Список інакших рівнів

    [HideInInspector] public GameObject player;
    [HideInInspector] public int currentLvlNum;
    public List<GameObject> levels = new List<GameObject>();
    public bool genRandomLevels = true;

    private void Start()
    {
        instance = this;

        player = GameObject.FindWithTag("Player");

        for (int i = 0; i < levelsNumToGenerate + 1; i++)
        {
            levelNum--;

            GameObject level = null;

            
            if (levels.Count > 35 && genRandomLevels)
            {
                level = Instantiate(anotherLevels[
                    Random.Range(0, anotherLevels.Count)],
                    new Vector3(0, 7, 0) + new Vector3(
                    0, levelNum * levelHeightStep, 0),
                    Quaternion.identity);
            }
            else if (levelNum == -2)
            {
                level = Instantiate(anotherLevels[
                    Random.Range(0, anotherLevels.Count)],
                    new Vector3(0, 7, 0) + new Vector3(
                    0, levelNum * levelHeightStep, 0),
                    Quaternion.identity);
            }
            else
            {
                level = Instantiate(levelPrefab,
                    new Vector3(0, 7, 0) + new Vector3(
                    0, levelNum * levelHeightStep, 0),
                    Quaternion.identity);
            }
            if (levels.Count > 10)
            {
                level.SetActive(false);
            }
            levels.Add(level);
        }
    }
}
