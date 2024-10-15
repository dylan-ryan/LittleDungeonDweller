using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class UIManager : MonoBehaviour
{
    public static UIManager manager;

    public GameObject mainMenu;
    public GameObject pause;
    public GameObject gameplay;
    public GameObject upgrade;

    [HideInInspector] public GameObject character;
    private SpriteRenderer characterArt;

    public LevelManager levelManager;

    [SerializeField]
    private GameManager gameManager;
    public enum GameState {MainMenu, Upgrade, Pause, Gameplay}
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
            case GameState.Upgrade:
                UpgradeUI();
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

        if(character.GetComponent<HealthSystem>().health <= 0)
        {
            gameState = GameState.Upgrade;
            character.GetComponent<HealthSystem>().health = levelManager.starterHealth + gameManager.health;

        }
    }

    public void MainMenuUI()
    {
        ManagerMainMenuUI();
        characterArt.enabled = false;
        character.GetComponent<CharacterController>().enabled = false;

    }

    public void PauseUI()
    {
        ManagerPauseUI();
        character.GetComponent<CharacterController>().enabled = false;
    }

    public void GameplayUI()
    {
        ManagerGameplayUI();
        characterArt.enabled = true;
        character.GetComponent<CharacterController>().enabled = true;

    }

    public void UpgradeUI()
    {
        ManagerUpgradeUI();
        characterArt.enabled = false;
        character.GetComponent<CharacterController>().enabled = false;
    }

    public void ManagerUpgradeUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pause.SetActive(false);
        mainMenu.SetActive(false);
        gameplay.SetActive(false);
        upgrade.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ManagerMainMenuUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        mainMenu.SetActive(true);
        upgrade.SetActive(false);
        pause.SetActive(false);
        gameplay.SetActive(false);
    }

    public void ManagerPauseUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        mainMenu.SetActive(false);
        upgrade.SetActive(false);
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
        upgrade.SetActive(false);
        Time.timeScale = 1f;
    }
}

