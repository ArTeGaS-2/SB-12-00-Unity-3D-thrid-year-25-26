using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private IGuardState currentState;

    [SerializeField] private string playerTag = "Player";
}
