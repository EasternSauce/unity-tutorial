using UnityEngine;
using UnityEngine.AI;

public class CharacterVisualsController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private Character character;

    [Header("Weapon References")]
    [SerializeField] private GameObject axeInHand;
    [SerializeField] private GameObject bowInHand;

    private WeaponType currentWeaponType = WeaponType.None;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<Character>();
    }

    private void Update()
    {
        if (animator != null && agent != null)
        {
            animator.SetFloat("motion", agent.velocity.magnitude);

            if (character != null)
                animator.SetBool("defeated", character.IsDead);
        }

        // Weapon visibility
        UpdateWeaponVisual();
    }

    private void UpdateWeaponVisual()
    {
        if (character == null) return;

        WeaponType weaponType = WeaponType.None;

        var inventory = GetComponent<PlayerInventory>();
        if (inventory != null && inventory.CurrentWeapon != null)
            weaponType = inventory.CurrentWeapon.itemData.weaponType;
        else
        {
            var aiCombat = GetComponent<AICombat>();
            if (aiCombat != null)
                weaponType = aiCombat.WeaponType == AIWeaponType.Bow ? WeaponType.Bow : WeaponType.OneHandedAxe;
        }

        if (weaponType == currentWeaponType) return;
        currentWeaponType = weaponType;

        if (axeInHand) axeInHand.SetActive(false);
        if (bowInHand) bowInHand.SetActive(false);

        switch (weaponType)
        {
            case WeaponType.Bow:
                if (bowInHand) bowInHand.SetActive(true);
                break;
            case WeaponType.OneHandedAxe:
            case WeaponType.TwoHandedAxe:
                if (axeInHand) axeInHand.SetActive(true);
                break;
        }
    }
}
