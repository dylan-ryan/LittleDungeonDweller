using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    void Awake()
    {
        //If GameManager doesnt exist set this as the manager and dont destruction on load
        if (manager == null)
        {
            DontDestroyOnLoad(gameObject);
            manager = this;
        }
        //If GameManager already exists destroy the dupe
        else if (manager != this)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
