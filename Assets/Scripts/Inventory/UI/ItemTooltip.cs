using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] private TMP_Text tooltipText;
    [SerializeField] private Image tooltipIcon;
    [SerializeField] private Vector2 offset = new Vector2(16f, -16f);
    [SerializeField] private bool followMouse = true;

    private RectTransform rectTransform;
    private Canvas parentCanvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0, 1);

        if (tooltipText == null) tooltipText = GetComponentInChildren<TMP_Text>(true);
        if (tooltipIcon == null) tooltipIcon = GetComponentInChildren<Image>(true);

        parentCanvas = GetComponentInParent<Canvas>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (followMouse && gameObject.activeSelf)
        {
            SetPosition(Input.mousePosition);
        }
    }

    public void Show(string text, Sprite icon, Vector2? screenPosition = null)
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

        Vector2 pos = screenPosition ?? (Vector2)Input.mousePosition;
        SetPosition(pos);
    }

    private void SetPosition(Vector2 screenPosition)
    {
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rectTransform.position = screenPosition + offset;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                screenPosition,
                parentCanvas.worldCamera,
                out Vector2 localPoint);
            rectTransform.localPosition = localPoint + offset;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetFollowMouse(bool value)
    {
        followMouse = value;
    }
}
