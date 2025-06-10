using UnityEngine;
using UnityEngine.InputSystem;

public class InteractInput : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI textOnScreen;
    [SerializeField] UIPoolBar hpBar;

    GameObject currentHoverOverObject;

    [HideInInspector]
    public InteractableObject hoveringOverObject;
    [HideInInspector]
    public IDamageable attackTarget;

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

        float hoverRadius = 0.5f; // Increase for bigger hover area
        int layerMask = ~LayerMask.GetMask("RaycastIgnore");

        if (Physics.SphereCast(ray, hoverRadius, out hit, float.MaxValue, layerMask))
        {
            GameObject hitObject = hit.transform.gameObject;

            if (currentHoverOverObject != hitObject)
            {
                SetOutlineEnabled(currentHoverOverObject, false);

                currentHoverOverObject = hitObject;
                SetOutlineEnabled(currentHoverOverObject, true);

                UpdateInteractableObject(hit);
            }
        }
        else
        {
            SetOutlineEnabled(currentHoverOverObject, false);
            currentHoverOverObject = null;
            hoveringOverObject = null;
            attackTarget = null;
            textOnScreen.text = "";
            hpBar.Clear();
        }
    }

    private void UpdateInteractableObject(RaycastHit hit)
    {
        InteractableObject interactableObject = hit.transform.GetComponent<InteractableObject>();
        if (interactableObject != null)
        {
            hoveringOverObject = interactableObject;
            attackTarget = interactableObject.GetComponent<IDamageable>();
            textOnScreen.text = hoveringOverObject.objectName;
        }
        else
        {
            hoveringOverObject = null;
            attackTarget = null;
            textOnScreen.text = "";
        }

        UpdateHPBar();
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

    private void SetOutlineEnabled(GameObject obj, bool enabled)
    {
        if (obj == null) return;

        var outline = obj.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = enabled;
        }
    }
}