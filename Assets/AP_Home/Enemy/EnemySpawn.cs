using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    private void Start()
    {
        StartCoroutine(SpawnEnemyEverySeconds());
    }
    IEnumerator SpawnEnemyEverySeconds()
    {
        yield return new WaitForSeconds(Random.Range(1f, 5f));
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}
