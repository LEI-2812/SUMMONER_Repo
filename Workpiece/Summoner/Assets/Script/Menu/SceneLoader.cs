using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static bool isHUDLoaded = false;

    void Awake()
    {
        if (!isHUDLoaded)
        {
            SceneManager.LoadScene("Screen/HUD", LoadSceneMode.Additive);
            isHUDLoaded = true;
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
