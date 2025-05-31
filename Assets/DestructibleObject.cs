using UnityEngine;

public class DestructibleObject : MonoBehaviour, IDamageable
{
    public ValuePool getLifePool()
    {
        return null;
    }

    public void TakeDamage(int damage)
    {
        Destroy(gameObject);
    }
}
