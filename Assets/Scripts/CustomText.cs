using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomText : MonoBehaviour
{
    public TextMeshProUGUI text;
    private GameManager gameManager;
    private GameObject player;
    private HealthSystem health;
    private CharacterControllerScript characterStats;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        health = player.GetComponent<HealthSystem>();
        characterStats = player.GetComponent<CharacterControllerScript>();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        gameManager = FindObjectOfType<GameManager>();
        text.SetText(
            "Currency: " + gameManager.currency + "        " +
            "Health: " + health.health + "      " +
            "Speed: " + characterStats.moveSpeed + "      " +
            "Damage: " + characterStats.attackDamage + "     " +
            "Range: " + characterStats.swordRange);
    }
}
