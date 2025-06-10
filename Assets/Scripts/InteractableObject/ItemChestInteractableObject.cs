using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class ItemChestInteractableObject : MonoBehaviour
{
    Animator animator;
    [SerializeField] ItemDropList dropList;

    [SerializeField] float ringInnerRadius = 1f;
    [SerializeField] float ringOuterRadius = 2f;

    bool isOpened = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        GetComponent<InteractableObject>().Subscribe(OpenChest);
    }

    public void OpenChest(Character character)
    {
        if (isOpened == true) { return; }

        GetComponent<Collider>().enabled = false;
        animator.SetBool("Open", true);
        for (int i = 0; i < 50; i++)
        {
            ItemSpawnManager.instance.SpawnItem(SelectRandomPosition(), dropList.GetDrop(), transform);
        }
        isOpened = true;
    }

    private Vector3 SelectRandomPosition()
    {
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);
        float radius = UnityEngine.Random.Range(ringInnerRadius, ringOuterRadius);

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        Vector3 offset = new Vector3(x, 0f, z);

        return transform.position + offset;
    }
}
