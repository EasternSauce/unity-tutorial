using System;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public Action<Character> interact;
    public string objectName;

    private void Start()
    {
        objectName = gameObject.name;
    }

    public void Subscribe(Action<Character> action)
    {
        interact += action;
    }

    public void Interact(Character interactor)
    {
        interact?.Invoke(interactor);
    }

}
