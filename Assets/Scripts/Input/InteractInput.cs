using UnityEngine;
using UnityEngine.InputSystem;

public class InteractInput : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI textOnScreen;
    [SerializeField] UIPoolBar hpBar;
    [SerializeField] private ItemTooltip itemTooltip;

    GameObject currentHoverOverObject;

    [HideInInspector] public InteractableObject hoveringOverObject;
    [HideInInspector] public IDamageable attackTarget;
    Character hoveringCharacter;

    Vector2 mousePosition;

    void Update()
    {
        CheckInteractObject();
    }

    public void MousePositionInput(InputAction.CallbackContext callbackContext)
    {
        mousePosition = callbackContext.ReadValue<Vector2>();
    }

    private void CheckInteractObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        float hoverRadius = 0.5f;
        int interactMask = ~LayerMask.GetMask("Player", "Terrain");

        if (Physics.SphereCast(ray, hoverRadius, out hit, float.MaxValue, interactMask))
        {
            GameObject hitObject = hit.transform.gameObject;

            if (currentHoverOverObject != hitObject)
            {
                HoverUtils.SetOutline(currentHoverOverObject, false);
                HoverUtils.HideTooltip(itemTooltip);
                currentHoverOverObject = hitObject;
            }

            HoverUtils.SetOutline(currentHoverOverObject, true);

            hoveringOverObject = hitObject.GetComponent<InteractableObject>();
            attackTarget = hitObject.GetComponent<IDamageable>();
            hoveringCharacter = hitObject.GetComponent<Character>();

            textOnScreen.text = hoveringCharacter != null ? hoveringOverObject?.objectName ?? "" : "";
            var pickupItem = hitObject.GetComponent<PickUpInteractableObject>();

            if (pickupItem != null && pickupItem.ItemData != null)
            {
                itemTooltip.Show(ItemTooltipBuilder.BuildTooltip(pickupItem.ItemData), pickupItem.ItemData.icon);
            }
            else
            {
                itemTooltip.Hide();
            }


            UpdateHPBar();
        }
        else
        {
            HoverUtils.SetOutline(currentHoverOverObject, false);
            HoverUtils.HideTooltip(itemTooltip);

            currentHoverOverObject = null;
            hoveringOverObject = null;
            attackTarget = null;
            hoveringCharacter = null;
            textOnScreen.text = "";
            hpBar.Clear();
        }
    }

    public bool TryGetTerrainPoint(out Vector3 point)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        int terrainMask = LayerMask.GetMask("Terrain");

        if (Physics.Raycast(ray, out hit, float.MaxValue, terrainMask))
        {
            point = hit.point;
            return true;
        }

        point = Vector3.zero;
        return false;
    }

    private void UpdateHPBar()
    {
        if (attackTarget != null)
        {
            hpBar.Show(attackTarget.GetLifePool());
        }
        else
        {
            hpBar.Clear();
        }
    }

    public bool InteractCheck()
    {
        return hoveringOverObject != null;
    }
}
