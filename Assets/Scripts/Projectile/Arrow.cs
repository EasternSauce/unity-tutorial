using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Character owner;
    private Vector3 direction;
    private float speed;
    private float heightOffset;

    public void Initialize(Character owner, Vector3 dir, float speed, float heightOffset)
    {
        this.owner = owner;
        this.direction = dir;
        this.speed = speed;
        this.heightOffset = heightOffset;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner.gameObject) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            Character targetChar = other.GetComponent<Character>();

            if (targetChar != null)
            {
                bool isOwnerPlayer = owner.IsPlayer;
                bool isTargetPlayer = targetChar.IsPlayer;

                if (isOwnerPlayer != isTargetPlayer)
                {
                    damageable.TakeDamage(owner.GetDamage());
                    Destroy(gameObject);
                }
            }
        }
    }
}
