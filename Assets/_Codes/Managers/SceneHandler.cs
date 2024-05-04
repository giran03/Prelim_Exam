using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance;
    string currentScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        currentScene = SceneManager.GetActiveScene().name;
    }

    private void Update() => currentScene = SceneManager.GetActiveScene().name;

    public void GoToScene(string sceneName) => SceneManager.LoadSceneAsync(sceneName);

    public void Play()
    {
        GoToScene("_Level-1");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void FinishLevel()
    {
        if (CurrentScene() == "_Level-1")
            GoToScene("_Level-2");
        else if (CurrentScene() == "_Level-2")
            GoToScene("_Level-3");
        else if (CurrentScene() == "_Level-3")
            GoToScene("_End-Screen");
        else
            Debug.Log("No Scenes Available Next");
    }

    public string CurrentScene() => currentScene;

    public void RestartScene() => GoToScene(CurrentScene());

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
