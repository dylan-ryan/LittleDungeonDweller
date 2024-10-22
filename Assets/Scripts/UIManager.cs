using TMPro;
using UnityEngine;

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
    [SerializeField] private GameManager gameManager;


    public TextMeshProUGUI healthText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rangeText;

    public enum GameState { MainMenu, Upgrade, Pause, Gameplay }
    public GameState gameState;

    void Awake()
    {
        if (manager == null)
        {
            DontDestroyOnLoad(gameObject);
            manager = this;
        }
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

        if (character.GetComponent<HealthSystem>().health <= 0)
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

        if (character != null)
        {
            CharacterControllerScript characterControllerScript = character.GetComponent<CharacterControllerScript>();

            if (characterControllerScript != null)
            {
                characterControllerScript.SetControlsEnabled(true);
            }

            characterArt.enabled = true;
            character.GetComponent<CharacterController>().enabled = true;
        }
    }


    public void UpgradeUI()
    {
        ManagerUpgradeUI();

        // Check if the character exists
        if (character != null)
        {
            CharacterControllerScript characterControllerScript = character.GetComponent<CharacterControllerScript>();

            if (characterControllerScript != null)
            {
                characterControllerScript.SetControlsEnabled(false);

                float upgradedHealth = levelManager.starterHealth + gameManager.health;
                float upgradedSpeed = characterControllerScript.moveSpeed;
                float upgradedDamage = characterControllerScript.attackDamage;
                float upgradedRange = characterControllerScript.swordRadius;

                // Update UI elements
                if (healthText != null) healthText.text = "Health: " + upgradedHealth.ToString();
                if (speedText != null) speedText.text = "Speed: " + upgradedSpeed.ToString();
                if (damageText != null) damageText.text = "Damage: " + upgradedDamage.ToString();
                if (rangeText != null) rangeText.text = "Range: " + upgradedRange.ToString();
            }
            else
            {
                Debug.LogWarning("CharacterControllerScript is missing on the Player.");
            }

            characterArt.enabled = false;
            character.GetComponent<CharacterController>().enabled = false;
        }
        else
        {
            Debug.LogWarning("Player character is missing.");
        }
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
