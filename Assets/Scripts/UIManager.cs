using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class UIManager : MonoBehaviour
{
    public static UIManager manager;

    public GameObject mainMenu;
    public GameObject pause;
    public GameObject gameplay;

    [HideInInspector] public GameObject character;
    private SpriteRenderer characterArt;

    public LevelManager levelManager;

    public enum GameState {MainMenu, Pause, Gameplay}
    public GameState gameState;
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

    public void Update()
    {
        character = GameObject.FindGameObjectWithTag("Player");
        characterArt = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();

        switch (gameState)
        {
            case GameState.MainMenu:
                MainMenuUI();
                break;
            case GameState.Pause:
                PauseUI();
                break;
            case GameState.Gameplay:
                GameplayUI();
                break;
        }

        if (Input.GetKeyUp(KeyCode.Escape) && gameState == GameState.Gameplay)
        {
            gameState = GameState.Pause;
        }
        else if (Input.GetKeyUp(KeyCode.Escape) && gameState == GameState.Pause)
        {
            gameState = GameState.Gameplay;
        }
    }

    public void MainMenuUI()
    {
        ManagerMainMenuUI();
        characterArt.enabled = false;
        character.GetComponent<CharacterContoller>().enabled = false;

    }

    public void PauseUI()
    {
        ManagerPauseUI();
        character.GetComponent<CharacterContoller>().enabled = false;
    }

    public void GameplayUI()
    {
        ManagerGameplayUI();
        characterArt.enabled = true;
        character.GetComponent<CharacterContoller>().enabled = true;

    }

    public void ManagerMainMenuUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        mainMenu.SetActive(true);
        pause.SetActive(false);
        gameplay.SetActive(false);
    }

    public void ManagerPauseUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        mainMenu.SetActive(false);
        pause.SetActive(true);
        gameplay.SetActive(false);
        Time.timeScale = 0f;
    }

    public void ManagerGameplayUI()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        mainMenu.SetActive(false);
        pause.SetActive(false);
        gameplay.SetActive(true);
        Time.timeScale = 1f;
    }
}

