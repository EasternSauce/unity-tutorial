using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Character shooter;
    private Vector3 velocity;

    [SerializeField] private float lifetime = 10f;

    public void Initialize(Character shooter, Vector3 direction, float speed, float heightOffset)
    {
        this.shooter = shooter;
        velocity = direction.normalized * speed;

        Quaternion rotation = Quaternion.LookRotation(velocity);
        rotation *= Quaternion.Euler(0f, 90f, 0f);
        transform.rotation = rotation;

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == shooter.gameObject) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(shooter.GetDamage());
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
