using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemGrid))]
public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventoryController inventoryController;
    ItemGrid itemGrid;

#pragma warning disable CS0618
    private void Start()
    {
        inventoryController = FindObjectOfType<InventoryController>();
        itemGrid = GetComponent<ItemGrid>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.SelectedItemGrid = itemGrid;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedItemGrid = null;
    }
}
