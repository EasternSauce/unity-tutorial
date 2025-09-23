using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Character shooter;
    private Vector3 velocity;

    [SerializeField] private float lifetime = 10f;
    [SerializeField] private Vector3 rotationOffset = new Vector3(0f, 90f, 0f);
    [SerializeField] private float damage = 20f;
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private float prefabDiameter = 2f;

    private float explosionRadius = 1f;

    public void Initialize(Character shooter, Vector3 direction, float speed, float heightOffset)
    {
        this.shooter = shooter;
        velocity = direction.normalized * speed;
        transform.rotation = Quaternion.LookRotation(velocity) * Quaternion.Euler(rotationOffset);
        Destroy(gameObject, lifetime);
        if (shooter.IsPlayer)
            gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");
        else
            gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
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

        if (shooter.IsPlayer)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage((int)damage);
                other.GetComponent<AIController>()?.OnAttacked(shooter.gameObject);
            }
            Explode();
            Destroy(gameObject);
        }
        else
        {
            if (other.TryGetComponent<Character>(out var target) && target.IsPlayer)
            {
                if (other.TryGetComponent<IDamageable>(out var damageable))
                    damageable.TakeDamage((int)damage);
                Explode();
                Destroy(gameObject);
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                Explode();
                Destroy(gameObject);
            }
        }
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
    }
}
