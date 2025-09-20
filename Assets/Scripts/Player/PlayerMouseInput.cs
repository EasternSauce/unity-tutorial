using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouseInput : MonoBehaviour
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

        int layerMask = LayerMask.GetMask("Terrain");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            rayToWorldIntersectionPoint = hit.point;
        }
    }
}
