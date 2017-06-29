using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
    public string initialSceneName = null;

    void Awake()
    {
        // Check if instance already exists
        if (instance == null)
        {
            instance = this;
        }

        // If it already exists and it's not this, then destroy this (to enforce our Singleton pattern)
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // This won't be destroyed when reloading scenes
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (initialSceneName != null)
        {
            SceneManager.LoadScene(initialSceneName);
        }
    }
}
