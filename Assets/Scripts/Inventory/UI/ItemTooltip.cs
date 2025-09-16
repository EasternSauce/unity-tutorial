using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] private TMP_Text tooltipText;
    [SerializeField] private Image tooltipIcon;
    [SerializeField] private Vector2 offset = new Vector2(16f, -16f);

    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private bool followMouse;
    private Vector2 initialAnchoredPosition;
    private Vector2 initialPivot;

    public GameObject CurrentTarget { get; private set; }
    public Vector2 InitialPivot => initialPivot;
    public Vector2 InitialAnchoredPosition => initialAnchoredPosition;

    public void SetCurrentTarget(GameObject target)
    {
        CurrentTarget = target;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (tooltipText == null) tooltipText = GetComponentInChildren<TMP_Text>(true);
        if (tooltipIcon == null) tooltipIcon = GetComponentInChildren<Image>(true);
        parentCanvas = GetComponentInParent<Canvas>();

        initialAnchoredPosition = rectTransform.anchoredPosition;
        initialPivot = rectTransform.pivot;

        var cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;

        if (tooltipText != null) tooltipText.raycastTarget = false;
        if (tooltipIcon != null) tooltipIcon.raycastTarget = false;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameObject.activeSelf && followMouse)
            FollowMouse();
    }

    private void FollowMouse()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition,
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera,
            out Vector2 localPos
        );
        rectTransform.localPosition = localPos + offset;
    }

    public void ShowForItem(InventoryItem item, bool followMouseCursor, GameObject target)
    {
        if (item == null || item.itemData == null) return;
        string tooltipTextValue = ItemTooltipBuilder.BuildTooltip(item.itemData);
        ShowInternal(tooltipTextValue, item.itemData.icon, followMouseCursor, target);
    }

    public void ShowForItemData(ItemData data, bool followMouseCursor, GameObject target)
    {
        if (data == null) return;
        string tooltipTextValue = ItemTooltipBuilder.BuildTooltip(data);
        ShowInternal(tooltipTextValue, data.icon, followMouseCursor, target);
    }

    private void ShowInternal(string text, Sprite icon, bool followMouseCursor, GameObject target)
    {
        CurrentTarget = target;
        gameObject.SetActive(true);

        tooltipText.text = text;
        tooltipText.enableAutoSizing = false;

        if (tooltipIcon != null)
        {
            tooltipIcon.enabled = icon != null;
            tooltipIcon.sprite = icon;
        }

        followMouse = followMouseCursor;
        if (followMouse)
        {
            rectTransform.pivot = new Vector2(0f, 1f);
            FollowMouse();
        }
        else
        {
            rectTransform.pivot = initialPivot;
            rectTransform.anchoredPosition = initialAnchoredPosition;
        }

        Canvas.ForceUpdateCanvases();
    }

    public void HideIfTarget(GameObject target)
    {
        if (CurrentTarget == target)
        {
            CurrentTarget = null;
            gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        if (CurrentTarget != null)
        {
            CurrentTarget = null;
            gameObject.SetActive(false);
        }
    }
}
