using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager manager;

    public GameObject mainMenu;
    public GameObject pause;
    public GameObject gameplay;
    public GameObject upgrade;
    public GameObject options;
    public GameObject results;
    public GameObject introPanel;

    private bool hasShownIntro = false;

    private GameState previousGameState;

    [HideInInspector] public GameObject character;
    private SpriteRenderer characterArt;

    public LevelManager levelManager;
    [SerializeField] private GameManager gameManager;

    [Header("Gameplay Stats")]
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI healthPriceText;
    public TextMeshProUGUI speedPriceText;
    public TextMeshProUGUI damagePriceText;
    public TextMeshProUGUI rangePriceText;

    public enum GameState { MainMenu, Upgrade, Pause, Gameplay, Options, Results }
    [Header("GameStates")]
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
        characterArt = character.GetComponent<SpriteRenderer>();

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
            case GameState.Upgrade:
                UpgradeUI();
                break;
            case GameState.Options:
                OptionsUI();
                break;
            case GameState.Results:
                ResultsUI();
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
            gameState = GameState.Results;
            character.GetComponent<HealthSystem>().health = levelManager.starterHealth + gameManager.health;
        }
    }

    public void OptionsUI()
    {
        ManagerOptionsUI();
    }

    public void ResultsUI()
    {
        ManagerResultsUI();
        characterArt.enabled = false;
        character.GetComponent<CharacterController>().enabled = false;
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
        CharacterControllerScript characterControllerScript = character.GetComponent<CharacterControllerScript>();
        characterControllerScript.controlsEnabled = true;

        if (character != null)
        {
            characterArt.enabled = true;
            character.GetComponent<CharacterController>().enabled = true;
        }

        if (!hasShownIntro && introPanel != null)
        {
            introPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void CloseIntroPanel()
    {
        if (introPanel != null)
        {
            introPanel.SetActive(false);
            hasShownIntro = true;
            Time.timeScale = 1f;
        }
    }

    public void UpgradeUI()
    {
        ManagerUpgradeUI();
        CharacterControllerScript characterControllerScript = character.GetComponent<CharacterControllerScript>();
        characterControllerScript.SetControlsEnabled(false);
        EventSystem.current.SetSelectedGameObject(null);

        if (character != null)
        {
            if (characterControllerScript != null)
            {
                float upgradeCurrency = gameManager.currency;

                float upgradedHealth = levelManager.starterHealth + gameManager.health;
                float upgradedSpeed = characterControllerScript.moveSpeed;
                float upgradedDamage = characterControllerScript.attackDamage;
                float upgradedRange = characterControllerScript.swordRadius;

                if (currencyText != null) currencyText.text = "Currency: " + upgradeCurrency.ToString();
                if (healthText != null) healthText.text = "Health: " + upgradedHealth.ToString();
                if (speedText != null) speedText.text = "Speed: " + upgradedSpeed.ToString();
                if (damageText != null) damageText.text = "Damage: " + upgradedDamage.ToString();
                if (rangeText != null) rangeText.text = "Range: " + upgradedRange.ToString();
                if (healthPriceText != null) healthPriceText.text = "Price: " + gameManager.healthPrice.ToString();
                if (speedPriceText != null) speedPriceText.text = "Price: " + gameManager.speedPrice.ToString();
                if (damagePriceText != null) damagePriceText.text = "Price: " + gameManager.damagePrice.ToString();
                if (rangePriceText != null) rangePriceText.text = "Price: " + gameManager.rangePrice.ToString();
            }

            characterArt.enabled = false;
            character.GetComponent<CharacterController>().enabled = false;
        }
    }

    public void ManagerOptionsUI()
    {
        options.SetActive(true);
        pause.SetActive(false);
        upgrade.SetActive(false);
        gameplay.SetActive(false);
        mainMenu.SetActive(false);
        results.SetActive(false);
    }

    public void ManagerResultsUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        mainMenu.SetActive(false);
        upgrade.SetActive(false);
        pause.SetActive(false);
        gameplay.SetActive(false);
        results.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ManagerUpgradeUI()
    {
        results.SetActive(false);
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
        results.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        mainMenu.SetActive(true);
        upgrade.SetActive(false);
        pause.SetActive(false);
        gameplay.SetActive(false);
    }

    public void ManagerPauseUI()
    {
        results.SetActive(false);
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
        results.SetActive(false);
        mainMenu.SetActive(false);
        pause.SetActive(false);
        gameplay.SetActive(true);
        upgrade.SetActive(false);
        if(hasShownIntro == true)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
        }
    }

    public void ButtonSwitchScreen(string screenName)
    {
        previousGameState = gameState;

        mainMenu.SetActive(false);
        pause.SetActive(false);
        gameplay.SetActive(false);
        upgrade.SetActive(false);
        options.SetActive(false);
        results.SetActive(false);

        switch (screenName)
        {
            case "MainMenu":
                mainMenu.SetActive(true);
                gameState = GameState.MainMenu;
                ManagerMainMenuUI();
                break;
            case "Pause":
                pause.SetActive(true);
                gameState = GameState.Pause;
                ManagerPauseUI();
                break;
            case "Gameplay":
                gameplay.SetActive(true);
                gameState = GameState.Gameplay;
                ManagerGameplayUI();
                break;
            case "Upgrade":
                upgrade.SetActive(true);
                gameState = GameState.Upgrade;
                ManagerUpgradeUI();
                break;
            case "Options":
                options.SetActive(true);
                gameState = GameState.Options;
                ManagerOptionsUI();
                break;
            case "Results":
                results.SetActive(true);
                gameState = GameState.Results;
                ManagerResultsUI();
                break;
            default:
                Debug.LogWarning("Screen name not recognized: " + screenName);
                break;
        }
    }
    public void ButtonBackToPreviousScreen()
    {
        ButtonSwitchScreen(previousGameState.ToString());
    }
}
