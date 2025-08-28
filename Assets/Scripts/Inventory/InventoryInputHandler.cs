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

    private void Update()
    {
        if (inventoryController == null) return;

        if (UnityEngine.Input.GetMouseButtonDown(0))
            ProcessLMBPress();
    }

    private void ProcessLMBPress()
    {
        if (inventoryController.SelectedItemGrid != null && inventoryController.GridHandler != null)
        {
            inventoryController.GridHandler.ItemGridInput();
            return;
        }

        if (inventoryController.SelectedItemSlot != null)
        {
            inventoryController.ItemSlotInput();
            return;
        }

        if (inventoryController.HasItemOnCursor)
        {
            bool overUI = IsPointerOverUI(UnityEngine.Input.mousePosition);
            if (!overUI)
                inventoryController.ThrowItemOnGround();
        }
    }

    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null) return false;

        var eventData = new PointerEventData(EventSystem.current) { position = screenPosition };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
