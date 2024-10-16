using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager manager;
    public UIManager UIManager;

    [SerializeField] private GameObject spawn;
    public int starterHealth = 3;

    public Scene currentScene;

    [SerializeField] private string sceneName;
     private GameManager gameManager;

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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        CharacterController characterContoller = player.GetComponent<CharacterController>();
        SceneManager.LoadScene(levelName);
        if (levelName != null)
        {
            if (levelName == "MainMenu")
            {
                UIManager.character.transform.position = spawn.transform.position;
                UIManager.gameState = UIManager.GameState.MainMenu;
                player.GetComponent<CharacterControllerScript>().objRenderer.material.color = Color.white;
            }
            else
            {
                UIManager.character.transform.position = spawn.transform.position;
                UIManager.gameState = UIManager.GameState.Gameplay;
                player.GetComponent<CharacterControllerScript>().objRenderer.material.color = Color.white;
            }
        }
        player.GetComponent<HealthSystem>().health = starterHealth + gameManager.health;
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
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
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
