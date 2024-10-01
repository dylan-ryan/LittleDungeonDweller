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
        ColorWait(2);
    }

    public void Heal(int hp)
    {
        health += hp;
    }

    private IEnumerable ColorWait(int seconds)
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 0.0f);
        yield return new WaitForSeconds(seconds);     
        gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f);
    }
}

