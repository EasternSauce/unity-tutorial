using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Character shooter;
    private Rigidbody rb;

    public void Initialize(Character shooter, Vector3 direction, float speed, float heightOffset)
    {
        this.shooter = shooter;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.linearVelocity = direction.normalized * speed;

        Quaternion rotation = Quaternion.LookRotation(rb.linearVelocity);
        rotation *= Quaternion.Euler(0f, 90f, 0f); // adjust this axis offset to match your prefab
        transform.rotation = rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable) && other.gameObject != shooter.gameObject)
        {
            damageable.TakeDamage(shooter.GetDamage());
            Destroy(gameObject);
        }
    }
}
