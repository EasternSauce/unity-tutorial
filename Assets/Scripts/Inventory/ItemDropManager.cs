using UnityEngine;

public static class ItemDropUtility
{
    public static void ThrowItemOnGround(SelectedItemController selectedItemController)
    {
        InventoryItem itemToDrop = selectedItemController.Drop();
        if (itemToDrop != null)
            DropItem(GameManager.instance.playerObject.transform.position, itemToDrop);
    }

    public static void DropItem(Vector3 dropPosition, InventoryItem itemToDrop)
    {
        if (itemToDrop == null) return;
        ItemSpawnManager.instance.SpawnItem(dropPosition, itemToDrop.itemData);
        Object.Destroy(itemToDrop.gameObject);
    }
}
