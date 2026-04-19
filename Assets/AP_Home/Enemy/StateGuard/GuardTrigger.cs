using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardTrigger : MonoBehaviour
{
    [SerializeField] private GuardAgent guard;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            guard.playerInTrigger = true;
        }
    } 
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            guard.playerInTrigger = false;
        }
    }
}
