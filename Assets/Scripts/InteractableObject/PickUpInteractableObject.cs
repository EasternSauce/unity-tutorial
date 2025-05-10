using UnityEngine;

public class PickUpInteractableObject : MonoBehaviour
{
    [SerializeField] int cointCount;

    private void Start()
    {
        GetComponent<InteractableObject>().interact += PickUp;
    }

    public void PickUp(Inventory inventory)
    {
        inventory.AddCurrency(cointCount);
        Debug.Log("You are trying to pick up me!");
        Destroy(gameObject);
    }
}
