using UnityEngine;

public class WeaponVisibilityController : MonoBehaviour
{
    [Header("Weapon References")]
    [SerializeField] GameObject axeInHand;   // child of RIGHT_HAND_COMBAT
    [SerializeField] GameObject axeOnBack;   // child of 2H_REST
    [SerializeField] GameObject bowInHand;   // child of LEFT_HAND_COMBAT
    [SerializeField] GameObject bowOnBack;   // child of 2H_REST

    PlayerInventory inventory;
    CanMoveState canMoveState;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        canMoveState = GetComponent<CanMoveState>();
    }

    private void Update()
    {
        UpdateWeaponVisibility();
    }

    private void UpdateWeaponVisibility()
    {
        // Default: disable everything
        axeInHand.SetActive(false);
        axeOnBack.SetActive(false);
        bowInHand.SetActive(false);
        bowOnBack.SetActive(false);

        if (inventory == null || inventory.CurrentWeapon == null)
            return;

        var weaponType = inventory.CurrentWeapon.itemData.weaponType;
        bool isAttacking = canMoveState != null && canMoveState.isAttacking;

        switch (weaponType)
        {
            case WeaponType.None:
                // Nothing to show
                break;

            case WeaponType.Bow:
                if (isAttacking)
                {
                    bowInHand.SetActive(true);
                }
                else
                {
                    bowOnBack.SetActive(true);
                }
                break;

            case WeaponType.OneHandedAxe:
                if (isAttacking)
                {
                    axeInHand.SetActive(true);
                }
                else
                {
                    axeOnBack.SetActive(true);
                }
                break;
            case WeaponType.TwoHandedAxe:
                if (isAttacking)
                {
                    axeInHand.SetActive(true);
                }
                else
                {
                    axeOnBack.SetActive(true);
                }
                break;
        }
    }
}
