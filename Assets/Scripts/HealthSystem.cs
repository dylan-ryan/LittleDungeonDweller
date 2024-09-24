using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
   
        public int health;

    public HealthSystem(int health)
    {
        this.health = health;
    }
    public void TakeDamage(int hp)
    {
        health = health - hp;
    }

    public void Heal(int hp)
    {
        health += hp;
    }
}

