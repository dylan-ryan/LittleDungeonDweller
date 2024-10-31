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

    public enum GameState { MainMenu, Upgrade, Pause, Gameplay }
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
            case GameState.Upgrade:
                UpgradeUI();
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
        CharacterControllerScript characterControllerScript = character.GetComponent<CharacterControllerScript>();
        characterControllerScript.controlsEnabled = true;

        if (character != null)
        {
            characterArt.enabled = true;
            character.GetComponent<CharacterController>().enabled = true;
        }
    }


    public void UpgradeUI()
    {
        ManagerUpgradeUI();
        CharacterControllerScript characterControllerScript = character.GetComponent<CharacterControllerScript>();
        characterControllerScript.SetControlsEnabled(false);
        EventSystem.current.SetSelectedGameObject(null);

        // Check if the character exists
        if (character != null)
        {
            if (characterControllerScript != null)
            {

                float upgradeCurrency = gameManager.currency;

                float upgradedHealth = levelManager.starterHealth + gameManager.health;
                float upgradedSpeed = characterControllerScript.moveSpeed;
                float upgradedDamage = characterControllerScript.attackDamage;
                float upgradedRange = characterControllerScript.swordRadius;

                // Update UI elements
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
        upgrade.SetActive(true); //true
        Time.timeScale = 0f;
    }

    public void ManagerMainMenuUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        mainMenu.SetActive(true); //true
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
        pause.SetActive(true); //true
        gameplay.SetActive(false);
        Time.timeScale = 0f;
    }

    public void ManagerGameplayUI()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        mainMenu.SetActive(false);
        pause.SetActive(false);
        gameplay.SetActive(true); //true
        upgrade.SetActive(false);
        Time.timeScale = 1f;
    }
}
