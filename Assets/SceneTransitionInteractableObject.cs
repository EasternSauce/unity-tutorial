using UnityEngine;

public class SceneTransitionInteractableObject : MonoBehaviour
{
    [SerializeField] string sceneName;

    void Start()
    {
        Debug.Log("here1");
        GetComponent<InteractableObject>().Subscribe(Transition);
    }

    public void Transition(Inventory inventory)
    {
        GameSceneManager.instance.StartTransition(sceneName);
    }
}
