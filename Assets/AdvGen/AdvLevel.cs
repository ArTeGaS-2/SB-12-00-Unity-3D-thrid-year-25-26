using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvLevel : MonoBehaviour
{
    private Advanced_Level_Generator levelGenerator; // ��������� �� ���������
    private GameObject player; // ��������� �� ��'��� ������
    private List<GameObject> levels; // ������ ������������ ����

    public void Start()
    {
        levelGenerator = Advanced_Level_Generator.instance;
        player = Advanced_Level_Generator.instance.player;
        levels = Advanced_Level_Generator.instance.levels;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (Mathf.Abs(levels[i].
                    transform.position.y - player.transform.position.y) > 70f)
                {
                    levels[i].SetActive(false);
                }
                else
                {
                    levels[i].SetActive(true);
                }
            }
        } 
    }
}
