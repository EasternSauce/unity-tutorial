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

    public void SpawnItem(Vector3 position, ItemData itemToSpawn)
    {
        if (itemToSpawn == null || itemPrefab == null) return;

        position += Vector3.up * 50f;

        if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, Mathf.Infinity, terrainLayerMask))
        {
            Scene currentScene = SceneManager.GetSceneByName(GameSceneManager.instance.CurrentScene);

            GameObject groundItemsParent = null;
            foreach (var root in currentScene.GetRootGameObjects())
            {
                if (root.name == "GroundItems")
                {
                    groundItemsParent = root;
                    break;
                }
            }

            if (groundItemsParent == null)
            {
                groundItemsParent = new GameObject("GroundItems");
                SceneManager.MoveGameObjectToScene(groundItemsParent, currentScene);
            }

            GameObject newItemGameObject = Instantiate(
                itemPrefab,
                hit.point + Vector3.up * (itemPrefab.GetComponent<Renderer>().bounds.size.y / 2f),
                Quaternion.identity
            );

            newItemGameObject.GetComponent<PickUpInteractableObject>().SetItem(itemToSpawn);

            newItemGameObject.transform.SetParent(groundItemsParent.transform);
        }
        else
        {
            Debug.LogWarning("ItemSpawnManager: Could not find valid ground to spawn item.");
        }
    }
}
