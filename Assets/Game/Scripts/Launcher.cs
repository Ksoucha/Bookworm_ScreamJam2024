using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour
{
    private void Awake()
    {
        GameEvents.onExitClicked += HandleExitClicked;
        GameEvents.onPlayClicked += HandlePlayClicked;
        GameEvents.onHomeClicked += HandleHomeClicked;
    }

    private void OnDestroy()
    {
        GameEvents.onExitClicked -= HandleExitClicked;
        GameEvents.onPlayClicked -= HandlePlayClicked;
        GameEvents.onHomeClicked -= HandleHomeClicked;
    }

     private void Start()
     {
         SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
     }
     
    private void HandleHomeClicked()
    {
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("Game");
    }

    private void HandlePlayClicked()
    {
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("Menu");
    }

    public void HandleExitClicked()
    {
        Application.Quit();
    }

    public void StartFirstChapter()
    {

    }
}
