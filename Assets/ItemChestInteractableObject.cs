using UnityEngine;

public class ItemChestInteractableObject : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        GetComponent<InteractableObject>().Subscribe(OpenChest);
    }

    public void OpenChest(Inventory inventory)
    {
        animator.SetBool("Open", true);
    }
}
