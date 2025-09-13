using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create_Steps : MonoBehaviour
{
    public GameObject firstStep; // Положення першої сходинки
    public GameObject stepPrefab; // Шаблон сходинки
    public int stepNum; // Кількість сходинок яку треба створити
    public float stepHeight = 0.5f; // Висота сходинки
    public float stepLenght = 1f; // Довжина сходинки
    private void Start()
    {

        for (int i = 0; i < stepNum + 1; i++)
        {
            Instantiate(
                stepPrefab,
                firstStep.transform.position + new Vector3(
                    0, i * stepHeight, i * stepLenght),
                Quaternion.identity);
        }
    }
}
