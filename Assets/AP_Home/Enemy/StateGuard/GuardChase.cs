using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class GuardChase : IGuardState
{
    public void Enter(GuardAgent guard)
    {
        guard.Agent.speed = 4f;
        guard.loseTimer = guard.LosePlayerTime;
    }
    public void Update(GuardAgent guard)
    {
        if (guard.Player != null)
        {
            guard.Agent.SetDestination(guard.Player.position);
        }
        if (guard.playerInTrigger)
        {
            guard.loseTimer = guard.LosePlayerTime;
        }
        else
        { 
            guard.loseTimer -= Time.deltaTime;
        }
        if (guard.loseTimer <= 0f)
        {
            guard.ChangeState(new GuardWalk());
        }
    }
    public void Exit(GuardAgent guard)
    {

    }
}
