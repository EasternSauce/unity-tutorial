using UnityEngine;
using UnityEngine.InputSystem;

public class InteractInput : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI textOnScreen;
    [SerializeField] UIPoolBar hpBar;

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
                SetOutlineEnabled(currentHoverOverObject, false);

                currentHoverOverObject = hitObject;
                SetOutlineEnabled(currentHoverOverObject, true);

                hoveringOverObject = hitObject.GetComponent<InteractableObject>();
                attackTarget = hitObject.GetComponent<IDamageable>();
                hoveringCharacter = hitObject.GetComponent<Character>();

                textOnScreen.text = hoveringCharacter != null ? hoveringOverObject?.objectName ?? "" : "";

                UpdateHPBar();
            }
        }
        else
        {
            SetOutlineEnabled(currentHoverOverObject, false);
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
