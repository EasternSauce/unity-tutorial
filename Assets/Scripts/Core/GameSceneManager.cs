using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;

    [SerializeField] string currentScene;

    public string CurrentScene => currentScene; // public getter for current scene

    AsyncOperation load;
    AsyncOperation unload;

    private void Awake()
    {
        instance = this;
    }

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

        while (!load.isDone || !unload.isDone)
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
