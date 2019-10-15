using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour
{
    public static GM Instance;

    public float BlockScale = 2;
    public int Rows = 30;
    public int Cols = 40;
    public int TotalKeys = 10;
    public GameState GameState;
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
            DontDestroyOnLoad(this);
            InitializeGameManager();
        }
    }

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
        Canvas = FindObjectOfType<Canvas>();
        Random = new System.Random();
        instantiateDungeon = GetComponent<InstantiateDungeon>();
        instantiateDungeon.CreateDungeon();
        Player = instantiateDungeon.Player;
        GameState = GameState.Active;
    }
}

public enum GameState { Active, Won }