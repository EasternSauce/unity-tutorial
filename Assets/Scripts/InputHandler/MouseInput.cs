using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInput : MonoBehaviour
{
    public Vector3 mouseInputPosition;
    [HideInInspector]
    public Vector3 rayToWorldIntersectionPoint;

    public void MousePositionUpdate(InputAction.CallbackContext callbackContext)
    {
        mouseInputPosition = callbackContext.ReadValue<Vector2>();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInputPosition);
        RaycastHit hit;

        int layerMask = ~LayerMask.GetMask("RaycastIgnore");

        if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
        {
            rayToWorldIntersectionPoint = hit.point;
        }
    }
}
