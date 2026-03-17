using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetHP : MonoBehaviour
{
    [SerializeField] Slider hpBarObj;
    [SerializeField] float damage = 20f;

    private HP_Bar hp_bar;
    private void Start()
    {
        hp_bar = hpBarObj.GetComponent<HP_Bar>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            hp_bar.TakeDamage(damage);
            Destroy(other.gameObject);
        }
    }
}
