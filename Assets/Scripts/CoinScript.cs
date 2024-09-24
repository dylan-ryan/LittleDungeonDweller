using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
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
        if(other == gameObject.CompareTag("Player"))
        {
            gameManager.AddCurrency(worth);
            Destroy(gameObject);
        }
    }
}
