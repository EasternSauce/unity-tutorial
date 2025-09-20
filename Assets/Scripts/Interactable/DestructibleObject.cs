using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class DestructibleObject : MonoBehaviour, IDamageable
{
    public ResourcePool GetLifePool()
    {
        return null;
    }

    public void TakeDamage(int damage)
    {
        Destroy(gameObject);
    }
}
