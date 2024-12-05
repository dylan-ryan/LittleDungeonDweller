using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    public int currency;
    public LevelManager levelManager;

    // References
    private GameObject player;
    private CharacterControllerScript characterControllerScript;
    private HealthSystem healthSystem;
    public int health = 0;

    public int damagePrice;
    public int attackSpeedPrice;
    public int moveSpeedPrice;
    public int healthPrice;

    [HideInInspector] public int currencyGainedLastRun = 0;
    [HideInInspector] public int enemiesKilledLastRun = 0;

    // Upgrade counters
    [HideInInspector] public int damageUpgradeCount = 0;
    [HideInInspector] public int attackSpeedUpgradeCount = 0;
    [HideInInspector] public int moveSpeedUpgradeCount = 0;
    [HideInInspector] public int healthUpgradeCount = 0;
    [HideInInspector] public const int maxUpgrades = 5;

    [Header("Enabled Buttons")]
    public Button damageButton;
    public Button attackSpeedButton;
    public Button moveSpeedButton;
    public Button healthButton;

    [Header("Disabled Buttons")]
    public Sprite disabledDamageSprite;
    public Sprite disabledAttackSpeedSprite;
    public Sprite disabledMoveSpeedSprite;
    public Sprite disabledHealthSprite;

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

    public void StartNewRun()
    {
        currencyGainedLastRun = 0;
        enemiesKilledLastRun = 0;
    }

    public void AddCurrency(int gained)
    {
        currency += gained;
        currencyGainedLastRun += gained;
        Debug.Log("Currency added: " + gained + ", Total currency gained this run: " + currencyGainedLastRun);
    }

    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        characterControllerScript = player.GetComponent<CharacterControllerScript>();
        healthSystem = player.GetComponent<HealthSystem>();
    }

    public void ButtonDamage()
    {
        if (currency >= damagePrice && damageUpgradeCount < maxUpgrades)
        {
            characterControllerScript.attackDamage += 1;
            currency -= damagePrice;
            damagePrice += damagePrice * 2;
            damageUpgradeCount++;

            if (damageUpgradeCount >= maxUpgrades)
            {
                DisableButton(damageButton, disabledDamageSprite);
                Debug.Log("Max damage upgrades reached.");
            }
        }
    }

    public void ButtonAttackSpeed()
    {
        if (currency >= attackSpeedPrice && attackSpeedUpgradeCount < maxUpgrades)
        {
            characterControllerScript.attackCooldown -= 0.1f;
            currency -= attackSpeedPrice;
            attackSpeedPrice += attackSpeedPrice * 2;
            attackSpeedUpgradeCount++;

            if (attackSpeedUpgradeCount >= maxUpgrades)
            {
                DisableButton(attackSpeedButton, disabledAttackSpeedSprite);
                Debug.Log("Max attack speed upgrades reached.");
            }
        }
    }

    public void ButtonMoveSpeed()
    {
        if (currency >= moveSpeedPrice && moveSpeedUpgradeCount < maxUpgrades)
        {
            characterControllerScript.moveSpeed += 1;
            currency -= moveSpeedPrice;
            moveSpeedPrice += moveSpeedPrice * 2;
            moveSpeedUpgradeCount++;

            if (moveSpeedUpgradeCount >= maxUpgrades)
            {
                DisableButton(moveSpeedButton, disabledMoveSpeedSprite);
                Debug.Log("Max move speed upgrades reached.");
            }
        }
    }

    public void ButtonHealth()
    {
        if (currency >= healthPrice && healthUpgradeCount < maxUpgrades)
        {
            health += 2;
            currency -= healthPrice;
            healthPrice += healthPrice * 2;
            healthUpgradeCount++;

            if (healthUpgradeCount >= maxUpgrades)
            {
                DisableButton(healthButton, disabledHealthSprite);
                Debug.Log("Max health upgrades reached.");
            }
        }
    }

    private void DisableButton(Button button, Sprite disabledSprite)
    {
        if (button != null)
        {
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = disabledSprite;
            }
            button.interactable = false;
        }
    }
}
