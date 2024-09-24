using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Coin Value")]
    [SerializeField] private int worth;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(other == player.CompareTag("Player"))
        {
            gameManager.AddCurrency(worth);
            Destroy(gameObject);
        }
    }
}
