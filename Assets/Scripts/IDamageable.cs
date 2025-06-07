using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int damage);

    public ValuePool GetLifePool();
}
