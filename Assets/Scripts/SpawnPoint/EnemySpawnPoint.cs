using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private int packSize = 5;
    [SerializeField] private float spawnRadius = 2f;

    private Transform enemiesParent;

    private void Start()
    {
        // Find the existing "Enemies" parent in the scene
        var parentObj = GameObject.Find("Enemies");
        if (parentObj == null)
        {
            Debug.LogError("No GameObject named 'Enemies' found in the scene!");
            return;
        }

        enemiesParent = parentObj.transform;

        var db = EnemyDatabase.Instance;
        if (db == null)
        {
            Debug.LogError("EnemyDatabase missing in scene!");
            return;
        }

        for (int i = 0; i < packSize; i++)
        {
            var prefab = db.GetRandomEnemy();

            Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
            Vector3 offset3D = new Vector3(offset2D.x, 0f, offset2D.y);

            Instantiate(prefab, transform.position + offset3D, Quaternion.identity, enemiesParent);
        }
    }
}
