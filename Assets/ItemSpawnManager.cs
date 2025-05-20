using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    public static ItemSpawnManager instance;

    [SerializeField] LayerMask terrainLayerMask;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] GameObject itemPrefab;

    public void SpawnItem(Vector3 position, ItemData itemToSpawn)
    {
        position += Vector3.up * 20;

        Ray findSurfaceRay = new Ray(position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(findSurfaceRay, out hit, Mathf.Infinity, terrainLayerMask))
        {
            GameObject newItemOnGround = GameObject.Instantiate(itemPrefab, hit.point, Quaternion.identity);
            newItemOnGround.GetComponent<PickUpInteractableObject>().SetItem(itemToSpawn);
        }
    }
}
