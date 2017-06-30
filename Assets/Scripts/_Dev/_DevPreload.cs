using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _DevPreload : MonoBehaviour {
    void Awake()
    {
        GameObject preloadedApp = GameObject.Find("__app");

        // Load the preload scene if the app hasn't already been preloaded 
        if (preloadedApp == null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("_PreloadScene");
        }
    }

    void Start()
    {
        GameObject preloadedApp = GameObject.Find("__app");
        LevelGenerator l = preloadedApp.GetComponent<LevelGenerator>();

        l.GenerateLevel();
    }
}
