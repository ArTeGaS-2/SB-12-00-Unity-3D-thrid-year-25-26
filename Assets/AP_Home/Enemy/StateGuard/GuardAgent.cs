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
    [SerializeField] private Transform[] points;

    [SerializeField] private float idleRadius = 0.5f;
    [SerializeField] private float idleMinTime = 25f;
    [SerializeField] private float idleMaxTime = 35f;
    [SerializeField] private float losePlayerTime = 10f;

    [HideInInspector] public bool playerInTrigger;
    [HideInInspector] public float idleTimer;
    [HideInInspector] public float loseTimer;
    [HideInInspector] public float idleMoveTimer;
    [HideInInspector] public int pointIndex;
    [HideInInspector] public Vector3 idleCenter;
    [HideInInspector] public Vector3 idlePoint;

    public NavMeshAgent Agent => agent;
    public Transform Player => player;
    public Transform[] Points => points;
    public float IdleRadius => idleRadius;
    public float IdleMinTime => idleMinTime;
    public float IdleMaxTime => idleMaxTime;
    public float LosePlayerTime => losePlayerTime;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag(
            playerTag);
        if (playerObj != null) player = playerObj.transform;
        else 
        {
            Debug.LogError($"Гравець з тегом '" +
                $"{playerTag}' не знайдений!");
        }
        ChangeState(new GuardIdle());
    }
    private void Update()
    {
        if (currentState != null) currentState.Update(this);
    }
    public void ChangeState(IGuardState newState)
    {
        if (currentState != null) currentState.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }
    public bool ReachedPoint()
    {
        if (agent.pathPending) return false;
        return agent.remainingDistance <= agent.stoppingDistance;
    }
    public Vector3 GetCurrentPoint()
    {
        if (points.Length == 0) return transform.position;
        return points[pointIndex].position;
    }
    public void NextPoint()
    {
        if (points.Length == 0) return;
        pointIndex++;
        if (pointIndex >= points.Length) pointIndex = 0;
    }
    public void SetRandomIdlePoint()
    {
        Vector2 randomPos = Random.insideUnitCircle * idleRadius;
        idlePoint = idleCenter + new Vector3(
            randomPos.x, 0f, randomPos.y);
    }
}
