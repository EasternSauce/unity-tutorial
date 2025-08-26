using UnityEngine;

public class UiPanelManager : MonoBehaviour
{
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject statsPanel;
    [SerializeField] GameObject questPanel;

    private bool canOpenPanels = true;

    private void Start()
    {
        if (GameManager.instance != null && GameManager.instance.playerObject != null)
        {
            CharacterDefeatHandler defeatHandler = GameManager.instance.playerObject.GetComponent<CharacterDefeatHandler>();

            if (defeatHandler != null)
            {
                defeatHandler.onDefeated.AddListener(HandleDefeat);
                defeatHandler.onRespawned.AddListener(HandleRespawn);
            }
            else
            {
                Debug.LogWarning("UiPanelManager: No CharacterDefeatHandler found on playerObject!");
            }
        }
        else
        {
            Debug.LogWarning("UiPanelManager: GameManager or playerObject is not set in Start!");
        }
    }

    private void HandleDefeat()
    {
        canOpenPanels = false;
        CloseAllPanels();
    }

    private void HandleRespawn()
    {
        canOpenPanels = true;
    }

    public void OpenInventory()
    {
        if (!canOpenPanels) return;
        inventoryPanel.SetActive(!inventoryPanel.activeInHierarchy);
    }

    public void OpenStats()
    {
        if (!canOpenPanels) return;
        statsPanel.SetActive(!statsPanel.activeInHierarchy);
        questPanel.SetActive(false);
    }

    public void OpenQuests()
    {
        if (!canOpenPanels) return;
        questPanel.SetActive(!questPanel.activeInHierarchy);
        statsPanel.SetActive(false);
    }

    public void CloseAllPanels()
    {
        inventoryPanel.SetActive(false);
        statsPanel.SetActive(false);
        questPanel.SetActive(false);
    }
}