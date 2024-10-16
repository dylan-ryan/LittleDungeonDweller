using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathbox : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other == player)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            playerHealth.TakeDamage(playerHealth.health);
        }
    }
}
