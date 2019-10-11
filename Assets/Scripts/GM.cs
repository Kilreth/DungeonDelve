using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
    public static GM Instance;

    public float BlockScale = 2;
    public int TotalKeys = 10;
    [HideInInspector]
    public Canvas Canvas { get; private set; }
    [HideInInspector]
    public GameObject Player { get; private set; }
    public System.Random Random { get; private set; }

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
        QualitySettings.vSyncCount = 1;
        Canvas = FindObjectOfType<Canvas>();
        Random = new System.Random(0);
        instantiateDungeon = GetComponent<InstantiateDungeon>();
        instantiateDungeon.CreateDungeon();
        Player = instantiateDungeon.Player;
    }
}
