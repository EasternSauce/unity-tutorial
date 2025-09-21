using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Character shooter;
    private Vector3 velocity;

    [SerializeField] private float lifetime = 10f;
    [SerializeField] private Vector3 rotationOffset = new Vector3(0f, 90f, 0f);
    [SerializeField] private float damage = 20f;

    public void Initialize(Character shooter, Vector3 direction, float speed, float heightOffset)
    {
        this.shooter = shooter;
        velocity = direction.normalized * speed;
        transform.rotation = Quaternion.LookRotation(velocity) * Quaternion.Euler(rotationOffset);
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity) * Quaternion.Euler(rotationOffset);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == shooter.gameObject) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage((int)damage);
        }

        Destroy(gameObject);
    }
}
