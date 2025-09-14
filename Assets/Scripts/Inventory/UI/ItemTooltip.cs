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

    private GameObject currentTarget;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (tooltipText == null) tooltipText = GetComponentInChildren<TMP_Text>(true);
        if (tooltipIcon == null) tooltipIcon = GetComponentInChildren<Image>(true);

        parentCanvas = GetComponentInParent<Canvas>();

        initialAnchoredPosition = rectTransform.anchoredPosition;
        initialPivot = rectTransform.pivot;

        // Make tooltip ignore raycasts so it doesn't block clicks
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;

        gameObject.SetActive(false);
    }


    private void Update()
    {
        if (!gameObject.activeSelf) return;

        if (followMouse)
        {
            FollowMouse();
        }
        else if (currentTarget != null)
        {
            // If the target is destroyed or invalid, hide tooltip automatically
            if (!currentTarget.activeInHierarchy)
            {
                Hide();
            }
        }
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

    public void Show(string text, Sprite icon, bool followMouseCursor = true, GameObject target = null)
    {
        if (string.IsNullOrEmpty(text))
        {
            Hide();
            return;
        }

        gameObject.SetActive(true);
        tooltipText.text = text;
        tooltipText.enableAutoSizing = false;

        if (tooltipIcon != null)
        {
            tooltipIcon.enabled = icon != null;
            tooltipIcon.sprite = icon;
        }

        followMouse = followMouseCursor;
        currentTarget = target;

        if (!followMouse)
        {
            // Reset pivot and anchored position for static tooltip
            rectTransform.pivot = initialPivot;
            rectTransform.anchoredPosition = initialAnchoredPosition;
        }
        else
        {
            FollowMouse();
        }
    }

    public void UpdateTooltip(string text, Sprite icon = null)
    {
        if (string.IsNullOrEmpty(text))
        {
            Hide();
            return;
        }

        tooltipText.text = text;

        if (tooltipIcon != null && icon != null)
        {
            tooltipIcon.enabled = true;
            tooltipIcon.sprite = icon;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentTarget = null;
    }
}
