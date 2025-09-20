using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int currency;

    [SerializeField] private ItemGrid mainInventoryItemGrid;
    [SerializeField] private InventoryController inventoryController;
    public InventoryController InventoryController => inventoryController;
    [SerializeField] private List<EquipmentItemSlot> slots;
    [SerializeField] private List<ItemData> itemsOnStart;

    [Header("Weapon Slots")]
    [SerializeField] private Transform handSlot;
    [SerializeField] private Transform backSlot;

    private Character character;
    public InventoryItem CurrentWeapon { get; private set; }
    private GameObject currentWeaponObject;

    private void Start()
    {
        mainInventoryItemGrid.Init();

        foreach (var slot in slots)
            slot.Init(this);

        character = GetComponent<Character>();
        UpdateCurrentWeapon();

        if (itemsOnStart == null) return;

        foreach (var item in itemsOnStart)
            AddItem(item);
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
    }

    public bool AddItem(ItemData itemData)
    {
        Vector2Int? position = mainInventoryItemGrid.FindSpaceForObject(itemData);
        if (position == null) return false;

        InventoryItem newItem = inventoryController.CreateNewInventoryItem(itemData);
        mainInventoryItemGrid.PlaceItem(newItem, position.Value.x, position.Value.y);
        return true;
    }

    public bool TryAddItemOrDrop(ItemData itemData, InventoryController controller)
    {
        if (AddItem(itemData)) return true;

        InventoryItem tempItem = controller.CreateNewInventoryItem(itemData);
        controller.DropItem(GameManager.instance.playerObject.transform.position, tempItem);
        return false;
    }

    public void AddStats(List<RegularStatValue> statsValues)
    {
        character.AddStats(statsValues);
    }

    public void SubtractStats(List<RegularStatValue> stats)
    {
        character.SubtractStats(stats);
    }

    public void UpdateCurrentWeapon()
    {
        CurrentWeapon = null;
        foreach (var slot in slots)
        {
            var item = slot.GetItem();
            if (item != null && item.itemData.equipmentSlot == EquipmentSlot.Weapon)
            {
                CurrentWeapon = item;
                currentWeaponObject = item.GetComponentInChildren<MeshRenderer>()?.gameObject;
                break;
            }
        }
    }

    public void EquipToHandSlot()
    {
        if (currentWeaponObject != null && handSlot != null)
        {
            currentWeaponObject.transform.SetParent(handSlot);
            currentWeaponObject.transform.localPosition = Vector3.zero;
            currentWeaponObject.transform.localRotation = Quaternion.identity;
        }
    }

    public void EquipToRestSlot()
    {
        if (currentWeaponObject != null && backSlot != null)
        {
            currentWeaponObject.transform.SetParent(backSlot);
            currentWeaponObject.transform.localPosition = Vector3.zero;
            currentWeaponObject.transform.localRotation = Quaternion.identity;
        }
    }
}
