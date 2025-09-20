using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;

    [SerializeField] string currentScene;

    public string CurrentScene => currentScene;

    AsyncOperation load;
    AsyncOperation unload;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentScene = GetFirstNonEssentialScene();
    }

    private string GetFirstNonEssentialScene()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != "Essential")
            {
                return scene.name;
            }
        }
        return null;
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
