using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    public int currency;
    public LevelManager levelManager;

    //Refrences
    private GameObject player;
    private CharacterControllerScript characterControllerScript;
    private HealthSystem healthSystem;
    public int health = 0;

    public int damagePrice;
    public int rangePrice;
    public int speedPrice;
    public int healthPrice;

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

    public void AddCurrency(int gained)
    {
        currency = currency + gained;
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        characterControllerScript = player.GetComponent<CharacterControllerScript>();
        healthSystem = player.GetComponent<HealthSystem>();
    }

    public void ButtonDamage()
    {
        if(currency >= damagePrice)
        {
            characterControllerScript.attackDamage += 1;
            currency -= damagePrice;
            damagePrice += 1;
        }
    }

    public void ButtonRange()
    {
        if (currency >= rangePrice)
        {
            characterControllerScript.swordRadius += 1f;
            currency -= rangePrice;
            rangePrice += 1; 
        }
    }

    public void ButtonSpeed()
    {
        if (currency >= speedPrice)
        {
            characterControllerScript.moveSpeed += 1;
            currency -= speedPrice;
            speedPrice += 1;
        }
    }

    public void ButtonHealth()
    {
        if (currency >= healthPrice)
        {
            health += 1;
            currency -= healthPrice;
            healthPrice += 1;
        }
    }
}
