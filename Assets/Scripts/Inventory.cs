using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int currency;

    public void AddCurrency(int amount)
    {
        currency += amount;
        Debug.Log("Currency = " + currency.ToString());
    }
}
