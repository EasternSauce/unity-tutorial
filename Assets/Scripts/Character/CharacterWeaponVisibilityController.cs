using UnityEngine;

public class CharacterWeaponVisibilityController : MonoBehaviour
{
    [Header("Weapon References")]
    [SerializeField] GameObject axeInHand;
    [SerializeField] GameObject axeOnBack;
    [SerializeField] GameObject bowInHand;
    [SerializeField] GameObject bowOnBack;

    [Header("Settings")]
    [SerializeField] float lingerTime = 2f;

    PlayerInventory inventory;
    CanMoveState canMoveState;

    float lingerTimer;

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
        axeInHand.SetActive(false);
        axeOnBack.SetActive(false);
        bowInHand.SetActive(false);
        bowOnBack.SetActive(false);

        if (inventory == null || inventory.CurrentWeapon == null)
            return;

        var weaponType = inventory.CurrentWeapon.itemData.weaponType;
        bool isAttacking = canMoveState != null && canMoveState.isAttacking;

        if (isAttacking)
            ResetLingerTimer();

        if (lingerTimer > 0f)
            lingerTimer -= Time.deltaTime;

        bool keepInHand = isAttacking || lingerTimer > 0f;

        switch (weaponType)
        {
            case WeaponType.None:
                break;

            case WeaponType.Bow:
                if (keepInHand)
                    bowInHand.SetActive(true);
                else
                    bowOnBack.SetActive(true);
                break;

            case WeaponType.OneHandedAxe:
            case WeaponType.TwoHandedAxe:
                if (keepInHand)
                    axeInHand.SetActive(true);
                else
                    axeOnBack.SetActive(true);
                break;
        }
    }

    public void ResetLingerTimer()
    {
        lingerTimer = lingerTime;
    }
}
