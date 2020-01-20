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

    // Saved game data: player, item and breadcrumb positions in addition to dungeon parameters
    // Will be null if generating a new dungeon
    public SaveGame SaveGame;

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
            if (SaveGame == null)
            {
                InitializeNewGame();
            }
            else
            {
                InitializeLoadedGame();
            }
            GameState = GameState.Active;
        }
        else
        {
            // Free the mouse cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Common behavior between InitializeNewGame() and InitializeLoadedGame()
    private void InitializeGameCommon()
    {
        if (DungeonParameters == null)
            throw new InvalidOperationException("Dungeon parameters object in Game Manager is null");

        Random = new System.Random(DungeonParameters.Seed);
        Canvas = FindObjectOfType<Canvas>();
        instantiateDungeon = GetComponent<InstantiateDungeon>();
        instantiateDungeon.CreateDungeon();
    }

    private void InitializeNewGame()
    {
        // Initialize the dungeon seed *before* generating the dungeon
        DungeonParameters.Seed = UnityEngine.Random.Range(Int32.MinValue, Int32.MaxValue);
        InitializeGameCommon();
        instantiateDungeon.InstantiateNewItems();
        Player = instantiateDungeon.InstantiateNewPlayer();
    }

    private void InitializeLoadedGame()
    {
        InitializeGameCommon();
        instantiateDungeon.InstantiateLoadedItems(SaveGame.Items);
        Player = instantiateDungeon.InstantiateLoadedPlayer(SaveGame.PlayerPosition, SaveGame.PlayerRotation);

        // Instantiate breadcrumbs using prefab and parent gameobject from other script
        Breadcrumbs breadcrumbsScript = Player.GetComponent<Breadcrumbs>();
        foreach (GameObjectSave b in SaveGame.Breadcrumbs)
        {
            Instantiate(breadcrumbsScript.Breadcrumb, b.Position, b.Rotation,
                            breadcrumbsScript.BreadcrumbsParent.transform);
        }
    }

    public void SaveGameToFile()
    {
        GameObject itemsParent = instantiateDungeon.ItemsParent;
        GameObject breadcrumbsParent = Player.GetComponent<Breadcrumbs>().BreadcrumbsParent;
        SaveGameSystem.SaveGameToFile(new SaveGame(DungeonParameters, Player, itemsParent, breadcrumbsParent));
    }
}

/// <summary>
/// GameState "Won" prevents the player from moving and from dropping breadcrumbs.
/// It also makes the cursor visible.
/// </summary>
public enum GameState { Active, Won }