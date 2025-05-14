using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int currency;
    [SerializeField] ItemGrid mainInventoryItemGrid;
    [SerializeField] InventoryController inventoryController;

    [SerializeField] List<EquipmentItemSlot> slots;

    Character character;

    private void Start()
    {
        mainInventoryItemGrid.Init();

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Init(this);
        }

        character = GetComponent<Character>();
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

    public void AddStats(List<StatsValue> statsValues)
    {
        character.AddStats(statsValues);
    }
}
