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
        var grid = inventoryController.SelectedItemGrid;
        var gridHandler = inventoryController.GridHandler;

        if (grid != null && gridHandler != null)
        {
            HandleGridClick(grid, gridHandler);
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

    private void HandleGridClick(ItemGrid grid, InventoryGridHandler gridHandler)
    {
        InventoryItem selectedItem = inventoryController.SelectedItemController.HasItem
            ? inventoryController.SelectedItemController.SelectedItem
            : null;

        Vector2 mousePos = UnityEngine.Input.mousePosition;
        Vector2Int tilePos = gridHandler.GetTileGridPosition(mousePos, selectedItem);

        if (selectedItem != null)
        {
            gridHandler.PlaceItemInput(grid, selectedItem, tilePos);
        }
        else
        {
            InventoryItem item = grid.PickUpItem(tilePos);
            if (item != null)
            {
                inventoryController.SelectedItemController.PickUp(item);
                inventoryController.ItemHighlightController?.SetSelectedItem(item);
            }
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
