using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int currency;
    [SerializeField] ItemGrid mainInventoryItemGrid;
    [SerializeField] public InventoryController inventoryController;

    [SerializeField] List<EquipmentItemSlot> slots;

    Character character;

    [SerializeField] List<ItemData> itemsOnStart;

    private void Start()
    {
        mainInventoryItemGrid.Init();

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Init(this);
        }

        character = GetComponent<Character>();

        if (itemsOnStart == null) { return; }

        for (int i = 0; i < itemsOnStart.Count; i++)
        {
            AddItem(itemsOnStart[i]);
        }
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
    }

    public bool AddItem(ItemData itemData)
    {
        Vector2Int? positionToPlace = mainInventoryItemGrid.FindSpaceForObject(itemData);

        if (positionToPlace == null) { return false; }

        InventoryItem newItem = inventoryController.CreateNewInventoryItem(itemData);
        mainInventoryItemGrid.PlaceItem(newItem, positionToPlace.Value.x, positionToPlace.Value.y);

        return true;
    }

    public bool TryAddItemOrDrop(ItemData itemData, InventoryController inventoryController)
    {
        bool added = AddItem(itemData);
        if (!added)
        {
            InventoryItem tempItem = inventoryController.CreateNewInventoryItem(itemData);

            inventoryController.DropItem(GameManager.instance.playerObject.transform.position, tempItem);

            return false;
        }

        return true;
    }

    public void AddStats(List<StatsValue> statsValues)
    {
        character.AddStats(statsValues);
    }

    public void SubtractStats(List<StatsValue> stats)
    {
        character.SubstractStats(stats);
    }
}
