using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public int speed = 150;

    public void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                collision.gameObject.transform.position,
                speed * Time.deltaTime);
        }
    }
    public void OnTriggerEnter(Collider collision)
    {
        StartCoroutine(DeathTime());
    }
    IEnumerator DeathTime()
    {
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);
    }
}
