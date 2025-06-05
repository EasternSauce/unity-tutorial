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

    public void Subscribe(Action<Inventory> action)
    {
        interact += action;
    }

    public void Interact(Inventory inventory)
    {
        Debug.Log("here2");
        interact?.Invoke(inventory);
    }

}
