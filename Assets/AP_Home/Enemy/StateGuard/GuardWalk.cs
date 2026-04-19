using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardWalk : IGuardState
{
    public void Enter(GuardAgent guard)
    {
        guard.Agent.speed = 2.5f;
        guard.Agent.SetDestination(guard.GetCurrentPoint());
    }
    public void Update(GuardAgent guard)
    {
        if (guard.playerInTrigger)
        {
            guard.ChangeState(new GuardChase());
            return;
        }

        guard.Agent.SetDestination(guard.GetCurrentPoint());

        if (guard.ReachedPoint())
        {
            guard.NextPoint();
            guard.ChangeState(new GuardIdle());
        }
    }
    public void Exit(GuardAgent guard)
    {

    }
}
