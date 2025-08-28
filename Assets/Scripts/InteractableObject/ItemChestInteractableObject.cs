using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class ItemChestInteractableObject : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private ItemDropList dropList;

    [SerializeField] private float ringInnerRadius = 1f;
    [SerializeField] private float ringOuterRadius = 2f;

    private bool isOpened = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        GetComponent<InteractableObject>().Subscribe(OpenChest);
    }

    public void OpenChest(Character character)
    {
        if (isOpened) return;

        GetComponent<Collider>().enabled = false;
        animator.SetBool("Open", true);

        // Spawn 50 items under "GroundItems"
        for (int i = 0; i < 50; i++)
        {
            ItemSpawnManager.instance.SpawnItem(SelectRandomPosition(), dropList.GetDrop());
        }

        isOpened = true;
    }

    private Vector3 SelectRandomPosition()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(ringInnerRadius, ringOuterRadius);

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        Vector3 offset = new Vector3(x, 0f, z);
        return transform.position + offset;
    }
}
