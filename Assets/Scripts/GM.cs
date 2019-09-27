using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
    public static GM Instance;

    public float BlockScale = 2;

    private InstantiateDungeon instantiateDungeon;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            InitializeGame();
        }
    }

    private void InitializeGame()
    {
        instantiateDungeon = GetComponent<InstantiateDungeon>();
        instantiateDungeon.CreateDungeon();
    }
}
