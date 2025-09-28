using UnityEngine;

/*
Arrow.cs

Purpose:
- Represents a projectile fired from a bow.
- Handles movement, rotation, collision detection, and lifetime management.

Functional Requirements / Expected Behavior:

1. Movement:
   - Arrow moves in a straight line with constant velocity.
   - Gravity is not applied; Y remains constant relative to the initialized direction.
   - Arrow rotation continuously updates to match its velocity vector.

2. Lifetime:
   - Arrow self-destructs after a configurable time (`lifetime`).
   - Prevents arrows from persisting indefinitely if they never hit anything.

3. Collision Handling:
   - Ignores collision with the shooter who fired it.
   - If shooter is Player:
     - Damages enemy characters (`Character` with `!IsPlayer`) on hit.
     - Calls `OnAttacked` on enemy AI controllers if present.
     - Damages destructible objects implementing `IDamageable` (e.g., crates, doors).
   - If shooter is AI:
     - Damages the Player character on hit.
   - If arrow collides with anything on the **"Terrain" layer**, it is destroyed immediately.
   - On all valid collisions (characters or destructibles), the arrow applies damage then destroys itself.

4. Rotation Offset:
   - Allows visual correction of the arrow model so it points in the expected direction.

Notes:
- Arrows travel on a perfectly straight trajectory, unaffected by gravity or terrain height.
- Colliding with destructible or damageable objects ensures arrows can interact with the world (breaking items, damaging enemies).
- Terrain collisions ensure arrows don’t fly endlessly across the scene.
*/

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

        // ✅ destroy on terrain hit
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            Destroy(gameObject);
            return;
        }

        if (shooter.IsPlayer)
        {
            if (other.TryGetComponent<Character>(out var target) && !target.IsPlayer)
            {
                if (other.TryGetComponent<IDamageable>(out var damageable))
                    damageable.TakeDamage(shooter.GetDamage());

                other.GetComponent<AIController>()?.OnAttacked(shooter.gameObject);
                Destroy(gameObject);
                return;
            }

            if (other.TryGetComponent<IDamageable>(out var destructible))
            {
                destructible.TakeDamage(shooter.GetDamage());
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            if (other.TryGetComponent<Character>(out var target) && target.IsPlayer)
            {
                if (other.TryGetComponent<IDamageable>(out var damageable))
                    damageable.TakeDamage(shooter.GetDamage());

                Destroy(gameObject);
                return;
            }

            return;
        }
    }
}
