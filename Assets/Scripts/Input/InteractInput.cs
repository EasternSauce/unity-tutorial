using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InteractInput : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI textOnScreen;
    [SerializeField] private UIPoolBar hpBar;
    [SerializeField] private TooltipController tooltipController;

    private GameObject currentHoverOverObject;
    [HideInInspector] public InteractableObject hoveringOverObject;
    [HideInInspector] public IDamageable attackTarget;
    private Character hoveringCharacter;
    private Vector2 mousePosition;

    private void Update() => CheckInteractObject();

    public void MousePositionInput(InputAction.CallbackContext callbackContext)
    {
        mousePosition = callbackContext.ReadValue<Vector2>();
    }

    private void CheckInteractObject()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            ClearCurrentHover();
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        float hoverRadius = 0.05f;
        int interactMask = ~LayerMask.GetMask("Player", "Terrain");

        if (Physics.SphereCast(ray, hoverRadius, out hit, float.MaxValue, interactMask))
        {
            GameObject hitObject = hit.transform.gameObject;

            if (currentHoverOverObject != hitObject)
            {
                SetOutline(currentHoverOverObject, false);
                currentHoverOverObject = hitObject;
                SetOutline(currentHoverOverObject, true);

                hoveringOverObject = hitObject.GetComponent<InteractableObject>();
                attackTarget = hitObject.GetComponent<IDamageable>();
                hoveringCharacter = hitObject.GetComponent<Character>();

                if (textOnScreen != null)
                    textOnScreen.text = hoveringCharacter != null ? hoveringOverObject?.objectName ?? "" : "";

                var pickupItem = hitObject.GetComponent<PickUpInteractableObject>();
                if (pickupItem != null && pickupItem.ItemData != null && tooltipController != null)
                {
                    tooltipController.ShowTooltipForGroundItem(pickupItem.ItemData, pickupItem.gameObject);
                }

                UpdateHPBar();
            }
        }
        else
        {
            ClearCurrentHover();
        }
    }

    private void ClearCurrentHover()
    {
        if (currentHoverOverObject != null)
        {
            SetOutline(currentHoverOverObject, false);
            tooltipController?.HideTooltipForGroundItem(currentHoverOverObject);
            currentHoverOverObject = null;
            hoveringOverObject = null;
            attackTarget = null;
            hoveringCharacter = null;
            if (textOnScreen != null) textOnScreen.text = "";
            if (hpBar != null) hpBar.Clear();
        }
    }

    private void UpdateHPBar()
    {
        if (attackTarget != null && hpBar != null)
            hpBar.Show(attackTarget.GetLifePool());
        else if (hpBar != null)
            hpBar.Clear();
    }

    public bool InteractCheck() => hoveringOverObject != null;

    public bool TryGetTerrainPoint(out Vector3 point)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        int terrainMask = LayerMask.GetMask("Terrain");

        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, terrainMask))
        {
            point = hit.point;
            return true;
        }

        point = Vector3.zero;
        return false;
    }

    private void SetOutline(GameObject obj, bool enabled)
    {
        if (obj == null) return;
        var outline = obj.GetComponent<Outline>();
        if (outline != null) outline.enabled = enabled;
    }
}
