using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemSpawnManager : MonoBehaviour
{
    public static ItemSpawnManager instance;

    [SerializeField] private LayerMask terrainLayerMask;
    [SerializeField] private GameObject itemPrefab;

    private void Awake()
    {
        instance = this;
    }

    public void SpawnItem(Vector3 position, ItemData itemToSpawn, GameObject caller)
    {
        if (caller == null)
        {
            Debug.LogError("ItemSpawnManager: caller cannot be null when using Option 2.");
            return;
        }

        Scene targetScene = caller.scene;

        Transform groundParent = null;
        foreach (var root in targetScene.GetRootGameObjects())
        {
            if (root.name == "GroundItems")
            {
                groundParent = root.transform;
                break;
            }
        }

        if (groundParent == null)
        {
            GameObject newParent = new GameObject("GroundItems");
            SceneManager.MoveGameObjectToScene(newParent, targetScene);
            groundParent = newParent.transform;
        }

        position += Vector3.up * 50f;
        if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, Mathf.Infinity, terrainLayerMask))
        {
            float height = itemPrefab.GetComponent<Renderer>().bounds.size.y;

            GameObject newItem = GameObject.Instantiate(
                itemPrefab,
                hit.point + Vector3.up * (height / 2f),
                Quaternion.identity
            );

            newItem.GetComponent<PickUpInteractableObject>().SetItem(itemToSpawn);

            SceneManager.MoveGameObjectToScene(newItem, targetScene);
            newItem.transform.SetParent(groundParent, true);
        }
    }
}
