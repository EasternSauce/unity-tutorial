using UnityEngine;

public class CanMoveState : MonoBehaviour
{
    public bool isAttacking;

    public bool Check()
    {
        return isAttacking == false;
    }
}
