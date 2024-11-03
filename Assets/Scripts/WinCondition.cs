using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        UpdateEnemyList();
    }

    void Update()
    {
        UpdateEnemyList();

        if (enemies.Count == 0)
        {
            WinGame();
        }
    }

    private void UpdateEnemyList()
    {
        enemies.Clear();
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    private void WinGame()
    {
        Debug.Log("You win!");
SceneManager.LoadScene("WinScene");
    }
}
