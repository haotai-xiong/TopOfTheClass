using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangePlayersTurn : MonoBehaviour
{
    public static ChangePlayersTurn Instance { get; private set; }

    public GameObject CurrentPlayersTurn;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Warning: multiple ChangePlayersTurn instances exist!");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

}
