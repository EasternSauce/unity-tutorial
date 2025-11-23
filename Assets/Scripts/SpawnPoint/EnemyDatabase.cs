using UnityEngine;
using System.Collections.Generic;

public class EnemyDatabase : MonoBehaviour
{
    public static EnemyDatabase Instance { get; private set; }

    [SerializeField] private List<GameObject> enemyPrefabs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public GameObject GetRandomEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError("EnemyDatabase has no enemy prefabs assigned!");
            return null;
        }

        int index = Random.Range(0, enemyPrefabs.Count);
        return enemyPrefabs[index];
    }
}