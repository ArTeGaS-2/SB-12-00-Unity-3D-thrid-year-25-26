using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP_Bar : MonoBehaviour
{
    [Header("Налаштування бару")]
    [SerializeField] GameObject hpBarObj;
    [SerializeField] float max_hp = 100f;
    [SerializeField] float min_hp = 0f;

    [Header("Налаштування об'єкту")]
    [SerializeField] GameObject targetObj;

    private float current_hp;
    private Slider hp_bar;

    private void Start()
    {
        current_hp = max_hp;

        hp_bar = hpBarObj.GetComponent<Slider>();

        hp_bar.maxValue = max_hp;
        hp_bar.minValue = min_hp;
        hp_bar.value = current_hp;
    }
    public void TakeDamage(float damage)
    {
        current_hp -= damage;
        hp_bar.value = current_hp;
        if (current_hp <= 0)
        {
            Destroy(targetObj);
        }
    }

}
