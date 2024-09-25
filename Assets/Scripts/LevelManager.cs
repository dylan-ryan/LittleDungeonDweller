using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager manager;
    public UIManager UIManager;

    [SerializeField] private GameObject spawn;

    public Scene currentScene;

    [SerializeField] private string sceneName;

    private void Update()
    {
        spawn = GameObject.FindGameObjectWithTag("SpawnPoint");
        sceneName = currentScene.name;
    }

    public void ButtonOptions(string function)
    {
        if (function == "return")
        {
            UIManager.gameState = UIManager.GameState.Pause;
        }
    }

    public void ButtonLoad(string levelName)
    {
        SceneManager.LoadScene(levelName);
        if (levelName != null)
        {
            if (levelName == "MainMenu")
            {
                UIManager.character.transform.position = spawn.transform.position;
                UIManager.gameState = UIManager.GameState.MainMenu;
            }
            else
            {
                UIManager.character.transform.position = spawn.transform.position;
                UIManager.gameState = UIManager.GameState.Gameplay;
            }
        }
    }

    public void ButtonResume()
    {
        UIManager.gameState = UIManager.GameState.Gameplay;
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }

    void Awake()
    {
        //If GameManager doesnt exist set this as the manager and dont destruction on load
        if (manager == null)
        {
            DontDestroyOnLoad(gameObject);
            manager = this;
        }
        //If GameManager already exists destroy the dupe
        else if (manager != this)
        {
            Destroy(gameObject);
        }
    }
}
