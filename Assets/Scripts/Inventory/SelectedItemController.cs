using UnityEngine;

public class SelectedItemController : MonoBehaviour
{
    public InventoryItem SelectedItem { get; private set; }
    public bool HasItem => SelectedItem != null;

    [SerializeField] private RectTransform parentTransform;
    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private CharacterDefeatHandler defeatHandler;

    private RectTransform selectedItemRectTransform;

    private void Awake()
    {
        if (defeatHandler == null)
            defeatHandler = FindFirstObjectByType<CharacterDefeatHandler>();
    }

    private void Update()
    {
        if (HasItem && selectedItemRectTransform != null)
        {
            Vector2 mousePosition = mouseInput.mouseInputPosition;
            selectedItemRectTransform.position = mousePosition;

            bool canShow = defeatHandler == null || !defeatHandler.IsDefeated;
            if (selectedItemRectTransform.gameObject.activeSelf != canShow)
                selectedItemRectTransform.gameObject.SetActive(canShow);
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
        selectedItemRectTransform.SetParent(parentTransform, true);
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
        SetSelectedItem(item);
        return SelectedItem;
    }

    public InventoryItem Drop()
    {
        if (defeatHandler != null && defeatHandler.IsDefeated)
            return null;

        InventoryItem temp = SelectedItem;
        ClearSelectedItem();
        return temp;
    }
}
