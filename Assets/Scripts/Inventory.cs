using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int currency;
    [SerializeField] ItemGrid mainInventoryItemGrid;
    [SerializeField] InventoryController inventoryController;

    private void Start()
    {
        mainInventoryItemGrid.Init();
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
        Debug.Log("Currency = " + currency.ToString());
    }

    public bool AddItem(ItemData itemData)
    {
        Vector2Int? positionToPlace = mainInventoryItemGrid.FindSpaceForObject(itemData);

        if (positionToPlace == null) { return false; }

        InventoryItem newItem = inventoryController.CreateNewInventoryItem(itemData);
        mainInventoryItemGrid.PlaceItem(newItem, positionToPlace.Value.x, positionToPlace.Value.y);

        return true;
    }
}
