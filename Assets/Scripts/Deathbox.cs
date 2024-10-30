using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered");
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            playerHealth.TakeDamage(playerHealth.health);
    }
}
