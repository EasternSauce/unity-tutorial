using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryInputHandler : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;

    private void Awake()
    {
        if (inventoryController == null)
            inventoryController = FindFirstObjectByType<InventoryController>();

        if (inventoryController == null)
            Debug.LogError("InventoryInputHandler: InventoryController is missing.");
    }

    public void LMB_InputHandle()
    {
        inventoryController.HandlePrimaryClick(UnityEngine.Input.mousePosition);
    }
}
