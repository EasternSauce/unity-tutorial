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
    private bool followMouse = true;

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
            Vector2 pos = Input.mousePosition;
            if (parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentCanvas.transform as RectTransform, pos, parentCanvas.worldCamera, out Vector2 localPoint);
                rectTransform.localPosition = localPoint + offset;
            }
            else
            {
                rectTransform.position = pos + offset;
            }
        }
    }

    public void Show(string text, Sprite icon, bool staticPosition = false)
    {
        followMouse = !staticPosition;

        if (string.IsNullOrEmpty(text)) return;
        gameObject.SetActive(true);
        tooltipText.text = text;
        tooltipText.enableAutoSizing = false;

        if (tooltipIcon != null)
        {
            tooltipIcon.enabled = icon != null;
            tooltipIcon.sprite = icon;
        }

        if (staticPosition)
        {
            rectTransform.localPosition = rectTransform.localPosition; // stays in original position
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
