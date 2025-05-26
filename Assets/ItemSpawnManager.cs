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
        position += Vector3.up * 50;

        Ray findSurfaceRay = new Ray(position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(findSurfaceRay, out hit, Mathf.Infinity, terrainLayerMask))
        {
            float height = itemPrefab.GetComponent<Renderer>().bounds.size.y;
            GameObject newItemOnGround = GameObject.Instantiate(itemPrefab, hit.point + Vector3.up * (height / 2f), Quaternion.identity);
            newItemOnGround.GetComponent<PickUpInteractableObject>().SetItem(itemToSpawn);
        }
    }
}
