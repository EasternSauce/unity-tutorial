using UnityEngine;

public class ItemChestInteractableObject : MonoBehaviour
{
    Animator animator;
    [SerializeField] ItemDropList dropList;

    [SerializeField] float itemDropRange = 2f;

    bool isOpened = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        GetComponent<InteractableObject>().Subscribe(OpenChest);
    }

    public void OpenChest(Inventory inventory)
    {
        if (isOpened == true) { return; }

        GetComponent<Collider>().enabled = false;
        animator.SetBool("Open", true);
        ItemSpawnManager.instance.SpawnItem(SelectRandomPosition(), dropList.GetDrop());
        isOpened = true;
    }

    private Vector3 SelectRandomPosition()
    {
        Vector3 pos = transform.position;

        pos += Vector3.right * UnityEngine.Random.Range(-itemDropRange, itemDropRange);
        pos += Vector3.forward * UnityEngine.Random.Range(-itemDropRange, itemDropRange);

        return pos;
    }
}
