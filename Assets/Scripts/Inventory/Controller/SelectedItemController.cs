using UnityEngine;

public class SelectedItemController : MonoBehaviour
{
    public InventoryItem SelectedItem { get; private set; }
    public bool HasItem => IsValidItem(SelectedItem);

    [SerializeField] private RectTransform parentTransform;
    [SerializeField] private CharacterDefeatHandler defeatHandler;
    [SerializeField] private Canvas canvas;

    private RectTransform selectedItemRectTransform;

    private void Awake()
    {
        if (defeatHandler == null)
            defeatHandler = FindFirstObjectByType<CharacterDefeatHandler>();
    }

    private void Update()
    {
        if (!HasItem || selectedItemRectTransform == null) return;

        bool canShow = defeatHandler == null || !defeatHandler.IsDefeated;
        if (selectedItemRectTransform.gameObject.activeSelf != canShow)
            selectedItemRectTransform.gameObject.SetActive(canShow);

        Vector2 mousePos = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentTransform, mousePos, canvas.worldCamera, out Vector2 localPoint);
        selectedItemRectTransform.localPosition = localPoint;
    }

    public void SetSelectedItem(InventoryItem item)
    {
        if (!IsValidItem(item))
        {
            ClearSelectedItem();
            return;
        }

        SelectedItem = item;
        selectedItemRectTransform = item.GetComponent<RectTransform>();
        selectedItemRectTransform.SetParent(parentTransform, false);
        selectedItemRectTransform.localScale = Vector3.one;
        selectedItemRectTransform.pivot = new Vector2(0.5f, 0.5f);
        selectedItemRectTransform.SetAsLastSibling();
    }

    public void ClearSelectedItem()
    {
        SelectedItem = null;
        selectedItemRectTransform = null;
    }

    public InventoryItem PickUp(InventoryItem item)
    {
        if (!IsValidItem(item)) return null;
        SetSelectedItem(item);
        return SelectedItem;
    }

    public InventoryItem Drop()
    {
        if (!HasItem) return null;
        if (defeatHandler != null && defeatHandler.IsDefeated) return null;

        InventoryItem temp = SelectedItem;
        ClearSelectedItem();
        return temp;
    }

    private bool IsValidItem(InventoryItem item)
    {
        if (item == null) return false;
        if (!item.gameObject.activeInHierarchy) return false;
        if (item.IsEquipped) return false;
        return true;
    }
}
