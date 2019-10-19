using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The Game Manager (GM) orchestrates dungeon creation, and makes
/// dungeon parameters and scene objects accessible by static reference.
/// </summary>
public class GM : MonoBehaviour
{
    public static GM Instance;

    // Parameters of the dungeon, set by LoadDungeonScene.cs before loading the scene
    public DungeonParameters DungeonParameters;

    public float BlockScale = 2;
    public GameState GameState;
    public System.Random Random { get; private set; }
    [HideInInspector]
    public Canvas Canvas { get; private set; }
    [HideInInspector]
    public GameObject Player { get; private set; }

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
            DontDestroyOnLoad(this);
            InitializeGameManager();
        }
    }

    // Setup that needs to happen when starting game from any scene
    private void InitializeGameManager()
    {
        QualitySettings.vSyncCount = 1;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Dungeon")
        {
            InitializeGame();
        }
        else
        {
            // Free the mouse cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void InitializeGame()
    {
        if (DungeonParameters == null)
        {
            throw new InvalidOperationException("Dungeon parameters object in Game Manager is null");
        }

        Canvas = FindObjectOfType<Canvas>();
        Random = new System.Random();
        instantiateDungeon = GetComponent<InstantiateDungeon>();
        instantiateDungeon.CreateDungeon();
        Player = instantiateDungeon.InstantiatePlayer();
        GameState = GameState.Active;
    }
}

/// <summary>
/// GameState "Won" prevents the player from moving and from dropping breadcrumbs.
/// It also makes the cursor visible.
/// </summary>
public enum GameState { Active, Won }