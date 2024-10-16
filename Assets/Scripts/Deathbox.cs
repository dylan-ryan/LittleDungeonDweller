using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other == gameObject.CompareTag("Player"))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            playerHealth.TakeDamage(playerHealth.health);
        }
    }
}
