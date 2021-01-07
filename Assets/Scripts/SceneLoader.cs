using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private const string MainSceneName = "MainScene";
    private const string SlotsSceneName = "SlotsScene";
    
    public void LoadMainScene()
    {
        SceneManager.LoadScene(MainSceneName, LoadSceneMode.Single);
    }
    public void LoadSlotsScene()
    {
        SceneManager.LoadScene(SlotsSceneName, LoadSceneMode.Single);
    }
}
