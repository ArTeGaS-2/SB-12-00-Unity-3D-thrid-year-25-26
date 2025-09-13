using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Level_Generator : MonoBehaviour
{
    [Header("Налаштування рівня")]
    public GameObject zeroLevel; // Перша платформа - нульовий поверх
    public GameObject levelPrefab; // Шаблон рівня
    public int levelNum; // Номер поточного рівня (для нумерації тексту на стіні)
    public float levelHeightStep = 7f; // Крок між поверхами
    [Header("Налаштування Стін")]
    public GameObject zeroLevelWalls; // Перший шар стін
    public GameObject levelWallsPrefab; // Шаблон стін
    public int wallNum; // Кількість стін
    public float wallsHeightStep = 7f; // Крок між стінами
    [Header("Інше")]
    public int levelsNumToGenerate = 10; // Кількість рівнів
    private void Start()
    {
        for (int i = 0; i < levelsNumToGenerate + 1; i++)
        {
            levelNum--;
            Instantiate(levelPrefab,
                new Vector3(0,7,0) + new Vector3(
                    0, levelNum * levelHeightStep,0),
                Quaternion.identity);
        }
        for (int i = 0; i < (levelsNumToGenerate + 1) * 2; i++)
        {
            wallNum--;
            Instantiate(levelWallsPrefab,
              new Vector3(0, 7, 0) + new Vector3(
                  0, wallNum * wallsHeightStep, 0),
              Quaternion.identity);
            
        }
    }
}
