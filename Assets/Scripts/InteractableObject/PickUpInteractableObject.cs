using UnityEngine;

public class PickUpInteractableObject : MonoBehaviour
{
    [SerializeField] int coinCount;
    [SerializeField] ItemData itemData;

    private void Start()
    {
        GetComponent<InteractableObject>().Subscribe(PickUp);
    }

    public void PickUp(Character character)
    {
        PlayerInventory inventory = character.GetComponent<PlayerInventory>();
        if (inventory == null) return;

        if (coinCount >= 0)
        {
            inventory.AddCurrency(coinCount);
        }

        if (itemData != null)
        {
            inventory.TryAddItemOrDrop(itemData, inventory.InventoryController);
        }

        Destroy(gameObject);
    }

    public void SetItem(ItemData itemToSpawn)
    {
        itemData = itemToSpawn;
    }

    public ItemData ItemData => itemData;
}
