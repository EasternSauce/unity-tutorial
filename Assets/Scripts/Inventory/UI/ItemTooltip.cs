using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text tooltipText;
    [SerializeField] private Image tooltipIcon;

    [Header("Settings")]
    [SerializeField] private Vector2 offset = new Vector2(16f, -16f);

    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private bool followMouse;

    private Vector2 initialAnchoredPosition;
    private Vector2 initialPivot;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (tooltipText == null) tooltipText = GetComponentInChildren<TMP_Text>(true);
        if (tooltipIcon == null) tooltipIcon = GetComponentInChildren<Image>(true);

        parentCanvas = GetComponentInParent<Canvas>();

        // Store initial position/pivot for ground items
        initialAnchoredPosition = rectTransform.anchoredPosition;
        initialPivot = rectTransform.pivot;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (followMouse && gameObject.activeSelf)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                Input.mousePosition,
                parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera,
                out Vector2 localPos
            );
            rectTransform.localPosition = localPos + offset;
        }
    }

    public void Show(string text, Sprite icon, bool isWorldItem = false)
    {
        if (string.IsNullOrEmpty(text)) return;

        gameObject.SetActive(true);

        tooltipText.text = text;
        tooltipText.enableAutoSizing = false;

        if (tooltipIcon != null)
        {
            tooltipIcon.enabled = icon != null;
            tooltipIcon.sprite = icon;
        }

        followMouse = !isWorldItem;

        if (isWorldItem)
        {
            // restore original pivot and anchored position to fix the half-shift
            rectTransform.pivot = initialPivot;
            rectTransform.anchoredPosition = initialAnchoredPosition;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                Input.mousePosition,
                parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera,
                out Vector2 localPos
            );
            rectTransform.localPosition = localPos + offset;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
