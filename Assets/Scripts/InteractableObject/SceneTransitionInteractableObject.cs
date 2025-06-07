using UnityEngine;

public class SceneTransitionInteractableObject : MonoBehaviour
{
    [SerializeField] string sceneName;

    void Start()
    {
        GetComponent<InteractableObject>().Subscribe(Transition);
    }

    public void Transition(Inventory inventory)
    {
        GameSceneManager.instance.StartTransition(sceneName);
    }
}
