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
    public List<GameObject> levels = new List<GameObject>();
    [Header("Інше")]
    public GameObject shadowObj; // Об'єкт тіні/скрімера
    [Header("Рандомні рівні")]
    public List<GameObject> anotherLevels
        = new List<GameObject>(); // Список інакших рівнів
    public int firstAnotherLevelIndex = 35; // поріг інакших рівнів
    private List<int> anotherLevelIndexes
        = new List<int>(); // Список індексів випадкових рівнів
    public bool genRandomLevels = true;

    [HideInInspector] public GameObject player;
    [HideInInspector] public int currentLvlNum;
    private void Start()
    {
        instance = this;

        player = GameObject.FindWithTag("Player");

        for (int i = 1; i < anotherLevels.Count; i++)
        {
            int randomLevelIndex = Random.Range(36, levelsNumToGenerate);
            foreach (int j in anotherLevelIndexes)
            {
                if (j == randomLevelIndex)
                {
                    randomLevelIndex =
                        Random.Range(36, levelsNumToGenerate);
                    
                }
                anotherLevelIndexes.Add(randomLevelIndex);
            }
        }

        for (int i = 0; i < levelsNumToGenerate + 1; i++)
        {
            levelNum--;
            GameObject level = null;

            if (anotherLevelIndexes[0] == i && genRandomLevels)
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
