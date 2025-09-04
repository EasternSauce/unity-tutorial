using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI currencyText;
    [SerializeField] PlayerInventory playerInventory;
    int currency = -1;

    void Update()
    {
        if (currency != playerInventory.currency)
        {
            currencyText.text = playerInventory.currency.ToString();
            currency = playerInventory.currency;
        }

    }
}
