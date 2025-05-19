using UnityEngine;

public class UiPanelManager : MonoBehaviour
{
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject statsPanel;
    [SerializeField] GameObject questPanel;

    public void OpenInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeInHierarchy);
    }

    public void OpenStats()
    {
        statsPanel.SetActive(!statsPanel.activeInHierarchy);
        questPanel.SetActive(false);

    }

    public void OpenQuests()
    {
        questPanel.SetActive(!questPanel.activeInHierarchy);
        statsPanel.SetActive(false);

    }
}
