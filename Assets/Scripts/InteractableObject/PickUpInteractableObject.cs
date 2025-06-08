using System;
using UnityEngine;

public class PickUpInteractableObject : MonoBehaviour
{
    [SerializeField] int cointCount;
    [SerializeField] ItemData itemData;

    private void Start()
    {
        GetComponent<InteractableObject>().Subscribe(PickUp);
    }

    public void PickUp(Character character)
    {
        Inventory inventory = character.GetComponent<Inventory>();

        if (inventory == null)
        {
            Debug.LogWarning("To interact with this object, this character needs an Inventory");
            return;
        }

        inventory.AddCurrency(cointCount);

        if (itemData != null)
        {
            inventory.AddItem(itemData);
        }

        Destroy(gameObject);
    }

    public void SetItem(ItemData itemToSpawn)
    {
        itemData = itemToSpawn;
    }
}
