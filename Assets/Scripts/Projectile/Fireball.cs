using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Character shooter;
    private Vector3 velocity;

    [Header("Fireball Settings")]
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private Vector3 rotationOffset = new Vector3(0f, 90f, 0f);
    [SerializeField] private float damage = 20f;

    [Header("Explosion Settings")]
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private float prefabDiameter = 2f;

    private float explosionRadius = 1f;

    public void Initialize(Character shooter, Vector3 direction, float speed, float heightOffset)
    {
        this.shooter = shooter;
        velocity = direction.normalized * speed;
        transform.rotation = Quaternion.LookRotation(velocity) * Quaternion.Euler(rotationOffset);
        Destroy(gameObject, lifetime);
    }

    public void SetExplosionRadius(float radius)
    {
        explosionRadius = radius;
    }

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity) * Quaternion.Euler(rotationOffset);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == shooter.gameObject) return;

        Explode();
        Destroy(gameObject);
    }

    private void Explode()
    {
        if (explosionEffectPrefab != null)
        {
            GameObject vfx = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

            float scaleFactor = (explosionRadius * 2f) / prefabDiameter;
            vfx.transform.localScale = Vector3.one * scaleFactor;

            Destroy(vfx, 3f);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hitColliders)
        {
            if (hit.gameObject == shooter.gameObject) continue;

            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage((int)damage);

                AIController aiEnemy = hit.GetComponent<AIController>();
                if (aiEnemy != null)
                    aiEnemy.OnAttacked(shooter.gameObject);
            }
        }
    }
}
