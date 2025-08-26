using UnityEngine;
using UnityEngine.InputSystem;

public class SelectedItemController : MonoBehaviour
{
    public InventoryItem SelectedItem { get; private set; }
    public bool HasItem => SelectedItem != null;

    [SerializeField] private RectTransform parentTransform;
    [SerializeField] private MouseInput mouseInput;

    private RectTransform selectedItemRectTransform;
    private CanvasGroup canvasGroup;

    private void Update()
    {
        if (HasItem)
        {
            Vector2 mousePosition = mouseInput.mouseInputPosition;
            selectedItemRectTransform.position = mousePosition;
        }
    }

    public void SetSelectedItem(InventoryItem item)
    {
        if (item == null)
        {
            ClearSelectedItem();
            return;
        }

        SelectedItem = item;
        selectedItemRectTransform = item.GetComponent<RectTransform>();

        selectedItemRectTransform.SetParent(parentTransform);
        selectedItemRectTransform.localScale = Vector3.one;
        selectedItemRectTransform.pivot = new Vector2(0.5f, 0.5f);
        selectedItemRectTransform.SetAsLastSibling();

        canvasGroup = item.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = item.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
    }

    public void ClearSelectedItem()
    {
        if (canvasGroup != null) canvasGroup.blocksRaycasts = true;
        SelectedItem = null;
        selectedItemRectTransform = null;
        canvasGroup = null;
    }

    public InventoryItem PickUp(InventoryItem item)
    {
        SetSelectedItem(item);
        return SelectedItem;
    }

    public InventoryItem Drop()
    {
        InventoryItem temp = SelectedItem;
        ClearSelectedItem();
        return temp;
    }
}
