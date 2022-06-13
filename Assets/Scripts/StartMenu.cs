using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class StartMenu : MonoBehaviour
{
    public string SceneName;
    
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Called");
        Application.Quit();
    }
}
