using UnityEngine;

public class DestructibleObject : MonoBehaviour, IDamageable
{
    public void TakeDamage(int damage)
    {
        Destroy(gameObject);
    }
}
