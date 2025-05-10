using System;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public Action<Inventory> interact;
    public string objectName;

    private void Start()
    {
        objectName = gameObject.name;
    }

    public void Interact(Inventory inventory)
    {
        interact?.Invoke(inventory);
    }

}
