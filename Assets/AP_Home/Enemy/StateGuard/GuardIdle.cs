using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardIdle : IGuardState
{
    public void Enter(GuardAgent guard)
    {
        guard.idleCenter = guard.transform.position;
        guard.idleTimer = Random.Range(
            guard.IdleMinTime,
            guard.IdleMaxTime);
        guard.idleMoveTimer = 0f;
        guard.Agent.speed = 1.5f;
        guard.SetRandomIdlePoint();
    }
    public void Update(GuardAgent guard)
    {
        if (guard.playerInTrigger)
        {
            guard.ChangeState(new GuardChase());
            return;
        }
        guard.idleTimer -= Time.deltaTime;
        guard.idleMoveTimer -= Time.deltaTime;

        if (guard.idleMoveTimer <= 0)
        {
            guard.SetRandomIdlePoint();
            guard.idleMoveTimer = Random.Range(2f, 4f);
        }
        guard.Agent.SetDestination(guard.idleCenter);
        if (guard.idleTimer <= 0)
        {
            guard.ChangeState(new GuardWalk());
        }

    }
    public void Exit(GuardAgent guard)
    {

    }
}


