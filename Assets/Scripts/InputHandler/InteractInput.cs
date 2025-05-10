using System;
using UnityEngine;

public class InteractInput : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI textOnScreen;
    [SerializeField] UIPoolBar hpBar;

    GameObject currentHoverOverObject;

    [HideInInspector]
    public InteractableObject hoveringOverObject;
    [HideInInspector]
    public Character hoveringOverCharacter;
    
    InteractHandler interactHandler;

    void Awake()
    {
        interactHandler = GetComponent<InteractHandler>();
    }

    void Update()
    {
        CheckInteractObject();
    }

    private void CheckInteractObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (currentHoverOverObject != hit.transform.gameObject)
            {
                currentHoverOverObject = hit.transform.gameObject;
                UpdateInteractableObject(hit);
            }
        }
    }

    
    internal void Interact()
    {
        interactHandler.interactedObject = hoveringOverObject;
    }

    private void UpdateInteractableObject(RaycastHit hit)
    {
        InteractableObject interactableObject = hit.transform.GetComponent<InteractableObject>();
        if (interactableObject != null)
        {
            hoveringOverObject = interactableObject;
            hoveringOverCharacter = interactableObject.GetComponent<Character>();
            textOnScreen.text = hoveringOverObject.objectName;

        }
        else
        {
            hoveringOverObject = null;
            hoveringOverCharacter = null;
            textOnScreen.text = "";
        }
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        if (hoveringOverCharacter != null)
        {
            hpBar.Show(hoveringOverCharacter.lifePool);
        }
        else {
            hpBar.Clear();
        }
    }

    internal bool InteractCheck()
    {
        return hoveringOverObject != null;
    }

    internal void ResetState()
    {
        interactHandler.ResetState();
    }
}
