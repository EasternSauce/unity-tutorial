using UnityEngine;

public class PickUpInteractableObject : MonoBehaviour
{
    [SerializeField] int cointCount;
    [SerializeField] ItemData itemData;

    private void Start()
    {
        GetComponent<InteractableObject>().Subscribe(PickUp);
    }

    public void PickUp(Inventory inventory)
    {
        inventory.AddCurrency(cointCount);

        if (itemData != null)
        {
            inventory.AddItem(itemData);
        }

        Debug.Log("You are trying to pick up me!");
        Destroy(gameObject);
    }
}
