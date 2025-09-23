using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Character shooter;
    private Vector3 velocity;

    [SerializeField] private float lifetime = 10f;
    [SerializeField] private Vector3 rotationOffset = new Vector3(0f, 90f, 0f);

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

        if (shooter.IsPlayer)
        {
            if (other.TryGetComponent<Character>(out var target) && !target.IsPlayer)
            {
                if (other.TryGetComponent<IDamageable>(out var damageable))
                    damageable.TakeDamage(shooter.GetDamage());

                other.GetComponent<AIController>()?.OnAttacked(shooter.gameObject);
            }
        }
        else
        {
            if (other.TryGetComponent<Character>(out var target) && target.IsPlayer)
            {
                if (other.TryGetComponent<IDamageable>(out var damageable))
                    damageable.TakeDamage(shooter.GetDamage());
            }
        }

        Destroy(gameObject);
    }
}
