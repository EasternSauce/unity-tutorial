using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] string currentScene;

    AsyncOperation load;
    AsyncOperation unload;

    private void Start()
    {
        // set currentScene to first non-essential scene
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name != "Essential")
            {
                currentScene = SceneManager.GetSceneAt(i).name;
                break;
            }
        }
    }

    public void StartTransition(string toSceneName)
    {
        StartCoroutine(Transition(toSceneName));
    }

    public IEnumerator Transition(string toSceneName)
    {
        SwitchScenes(toSceneName);

        while (load.isDone == false && unload.isDone == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        load = null;
        unload = null;
    }

    public void SwitchScenes(string toSceneName)
    {
        load = SceneManager.LoadSceneAsync(toSceneName, LoadSceneMode.Additive);
        unload = SceneManager.UnloadSceneAsync(currentScene);
        currentScene = toSceneName;
    }
}
