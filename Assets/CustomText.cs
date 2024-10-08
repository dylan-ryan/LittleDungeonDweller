using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomText : MonoBehaviour
{
    public TextMeshProUGUI text;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        gameManager = FindObjectOfType<GameManager>();
        text.SetText("Currency: " + gameManager.currency);
    }
}
