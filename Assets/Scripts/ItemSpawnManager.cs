using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    public static ItemSpawnManager instance;

    [SerializeField] LayerMask terrainLayerMask;
    [SerializeField] GameObject itemPrefab;

    private Transform groundItemsParent;

    private void Awake()
    {
        instance = this;
        InitializeGroundItemsParent();
    }

    private void InitializeGroundItemsParent()
    {
        GameObject existingParent = GameObject.Find("GroundItems");
        if (existingParent != null)
        {
            groundItemsParent = existingParent.transform;
        }
        else
        {
            GameObject newParent = new GameObject("GroundItems");
            groundItemsParent = newParent.transform;
        }
    }

    /// <summary>
    /// Spawns an item on the ground. Parent is always "GroundItems".
    /// </summary>
    public void SpawnItem(Vector3 position, ItemData itemToSpawn)
    {
        // Force parent to GroundItems
        Transform parent = groundItemsParent;

        position += Vector3.up * 50;

        Ray findSurfaceRay = new Ray(position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(findSurfaceRay, out hit, Mathf.Infinity, terrainLayerMask))
        {
            float height = itemPrefab.GetComponent<Renderer>().bounds.size.y;

            // Instantiate under GroundItems
            GameObject newItemOnGround = GameObject.Instantiate(
                itemPrefab,
                hit.point + Vector3.up * (height / 2f),
                Quaternion.identity,
                parent
            );

            newItemOnGround.GetComponent<PickUpInteractableObject>().SetItem(itemToSpawn);
        }
    }
}
