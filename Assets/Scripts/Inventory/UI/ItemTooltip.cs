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
    [SerializeField] private bool followMouse = true;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0, 1);

        if (tooltipText == null) tooltipText = GetComponentInChildren<TMP_Text>(true);
        if (tooltipIcon == null) tooltipIcon = GetComponentInChildren<Image>(true);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (followMouse && gameObject.activeSelf)
        {
            rectTransform.position = (Vector2)Input.mousePosition + offset;
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
        rectTransform.position = pos + offset;
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
