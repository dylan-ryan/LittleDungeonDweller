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
        characterControllerScript.attackDamage += 1;
    }

    public void ButtonRange()
    {
        characterControllerScript.swordRadius += 1f;
    }

    public void ButtonSpeed()
    {
        characterControllerScript.moveSpeed += 1;
    }

    public void ButtonHealth()
    {
        health += 1;
    }
}
