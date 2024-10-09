using System.Collections;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int health = 3;
    private Renderer objRenderer;

    private void Start()
    {
        objRenderer = gameObject.GetComponent<Renderer>();
    }

    public HealthSystem(int health)
    {
        this.health = health;
    }

    public void TakeDamage(int hp)
    {
        health -= hp;
        StartCoroutine(ColorWait(0.5f)); // Start the coroutine to change the color
    }

    public void Heal(int hp)
    {
        health += hp;
    }

    private IEnumerator ColorWait(float seconds)
    {
        // Change color to red
        objRenderer.material.color = Color.red;
        // Wait for the specified time (1 second)
        yield return new WaitForSeconds(seconds);
        // Change back to the original color
        objRenderer.material.color = Color.white;
    }
}
