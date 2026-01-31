using UnityEngine;

public sealed class PlayerAttackState : MonoBehaviour
{
    public bool IsAttacking { get; private set; }

    public void SetAttacking(bool value)
    {
        IsAttacking = value;
    }
}

