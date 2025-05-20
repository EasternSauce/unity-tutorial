using UnityEngine;

public class DropSystemTest : MonoBehaviour
{
    [SerializeField] ItemDropList dropList;

    private void Update()
    {
        if (dropList == null) { return; }

        if (Input.GetKey(KeyCode.Z))
        {
            Debug.Log(dropList.GetDropName());
        }
    }
}
