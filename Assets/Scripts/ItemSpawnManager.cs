using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemSpawnManager : MonoBehaviour
{
    public static ItemSpawnManager instance;

    [SerializeField] LayerMask terrainLayerMask;
    [SerializeField] GameObject itemPrefab;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    /// <summary>
    /// Spawns an item on the ground in the current area scene.
    /// </summary>
    public void SpawnItem(Vector3 position, ItemData itemToSpawn)
    {
        if (itemToSpawn == null || itemPrefab == null) return;

        position += Vector3.up * 50f; // lift above terrain

        if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, Mathf.Infinity, terrainLayerMask))
        {
            // Find the current scene by name
            Scene currentScene = SceneManager.GetSceneByName(GameSceneManager.instance.CurrentScene);

            // Look for "GroundItems" in root objects
            GameObject groundItemsParent = null;
            foreach (var root in currentScene.GetRootGameObjects())
            {
                if (root.name == "GroundItems")
                {
                    groundItemsParent = root;
                    break;
                }
            }

            // Create "GroundItems" if missing
            if (groundItemsParent == null)
            {
                groundItemsParent = new GameObject("GroundItems");
                SceneManager.MoveGameObjectToScene(groundItemsParent, currentScene);
            }

            // Instantiate item
            GameObject newItemGO = Instantiate(
                itemPrefab,
                hit.point + Vector3.up * (itemPrefab.GetComponent<Renderer>().bounds.size.y / 2f),
                Quaternion.identity
            );

            newItemGO.GetComponent<PickUpInteractableObject>().SetItem(itemToSpawn);

            // Parent under GroundItems
            newItemGO.transform.SetParent(groundItemsParent.transform);
        }
        else
        {
            Debug.LogWarning("ItemSpawnManager: Could not find valid ground to spawn item.");
        }
    }
}
