using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CursorTargetingHandler : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI textOnScreen;
    [SerializeField] private UIPoolBar hpBar;
    [SerializeField] private ItemTooltipController tooltipController;

    private GameObject currentHoverOverObject;
    [HideInInspector] public InteractableObject hoveringOverObject;
    [HideInInspector] public IDamageable attackTarget;
    private Character hoveringCharacter;
    private AIEnemy hoveringEnemy;
    private Vector2 mousePosition;

    private void Update()
    {
        CheckInteractObject();
    }

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

                hoveringOverObject = hitObject.GetComponentInParent<InteractableObject>();
                attackTarget = hitObject.GetComponentInParent<IDamageable>();
                hoveringCharacter = hitObject.GetComponentInParent<Character>();
                hoveringEnemy = hitObject.GetComponentInParent<AIEnemy>();

                // Only show name for enemies
                if (textOnScreen != null)
                {
                    textOnScreen.text = hoveringEnemy != null ? hoveringEnemy.name : "";
                }

                var pickupItem = hitObject.GetComponentInParent<PickUpInteractableObject>();
                if (pickupItem != null && pickupItem.ItemData != null && tooltipController != null)
                {
                    tooltipController.ShowTooltip(pickupItem.ItemData, pickupItem.gameObject);
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
            tooltipController?.HideTooltip();
            currentHoverOverObject = null;
        }

        hoveringOverObject = null;
        attackTarget = null;
        hoveringCharacter = null;
        hoveringEnemy = null;

        if (textOnScreen != null) textOnScreen.text = "";
        if (hpBar != null) hpBar.Clear();
    }

    private void UpdateHPBar()
    {
        if (hoveringEnemy != null && hpBar != null)
        {
            attackTarget = hoveringEnemy.GetComponent<IDamageable>();
            if (attackTarget != null)
                hpBar.Show(attackTarget.GetLifePool());
            else
                hpBar.Clear();
        }
        else if (hpBar != null)
        {
            hpBar.Clear();
        }
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
