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

    private float timeElapsed;
    private float timeAtDungeonLoad;

    [HideInInspector]
    public Canvas Canvas { get; private set; }
    [HideInInspector]
    public GameObject Player { get; private set; }
    [HideInInspector]
    public GameObject BlocksParent { get; private set; }
    [HideInInspector]
    public GameObject UnreachableBlocksParent { get; private set; }
    [HideInInspector]
    public GameObject ItemsParent { get; private set; }

    public bool MapActive;
    public bool UseRandomSeed;

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
        SaveGame = null;
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
            try
            {
                if (SaveGame == null)
                    InitializeNewGame();
                else
                    InitializeLoadedGame();

                GameState = GameState.Active;
            }
            catch (Exception)
            {
                SceneManager.LoadScene("MainMenu");
                GameState = GameState.MainMenu;
            }
        }
        else
        {
            // Free the mouse cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GameState = GameState.MainMenu;
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
        BlocksParent = instantiateDungeon.BlocksParent;
        UnreachableBlocksParent = instantiateDungeon.UnreachableBlocksParent;
        timeAtDungeonLoad = Time.time;
        MapActive = false;
    }

    private void InitializeNewGame()
    {
        if (UseRandomSeed)
            DungeonParameters.Seed = UnityEngine.Random.Range(Int32.MinValue, Int32.MaxValue);

        InitializeGameCommon();
        instantiateDungeon.InstantiateNewItems();
        ItemsParent = instantiateDungeon.ItemsParent;
        Player = instantiateDungeon.InstantiateNewPlayer();
        Player.GetComponent<PickUpKey>().InitializeKeyCounter();
        timeElapsed = 0;
    }

    private void InitializeLoadedGame()
    {
        InitializeGameCommon();
        instantiateDungeon.InstantiateLoadedItems(SaveGame.Items);
        ItemsParent = instantiateDungeon.ItemsParent;
        Player = instantiateDungeon.InstantiateLoadedPlayer(SaveGame.PlayerPosition, SaveGame.PlayerRotation);
        Player.GetComponent<Breadcrumbs>().LoadBreadcrumbs(SaveGame.Breadcrumbs);
        Player.GetComponent<PickUpKey>().InitializeKeyCounter(SaveGame.KeysCollected);
        timeElapsed = SaveGame.TimeElapsed;
    }

    public void SaveGameToFile()
    {
        int keysCollected = Player.GetComponent<PickUpKey>().KeysCollected;
        GameObject breadcrumbsParent = Player.GetComponent<Breadcrumbs>().BreadcrumbsParent;
        SaveGameSystem.SaveGameToFile(new SaveGame(DungeonParameters, Player,
            ItemsParent, breadcrumbsParent, keysCollected, GetTimeElapsed()));
    }

    public float GetTimeElapsed()
    {
        return timeElapsed + Time.time - timeAtDungeonLoad;
    }
}

/// <summary>
/// GameState "Won" prevents the player from moving or dropping breadcrumbs.
/// It also makes the cursor visible.
///
/// GameState "Inactive" means we are not in a dungeon.
/// </summary>
public enum GameState { Active, Won, MainMenu }