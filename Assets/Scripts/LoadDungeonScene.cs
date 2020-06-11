using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadDungeonScene : MonoBehaviour
{
    [SerializeField]
    private DungeonParameters parameters = null;

    public void LoadNewDungeon()
    {
        GM.Instance.SaveGame = null;
        SceneManager.LoadScene("Dungeon");
    }

    public void LoadDungeonFromSave()
    {
        try
        {
            GM.Instance.SaveGame = SaveGameSystem.LoadGameFromFile();
            GM.Instance.DungeonParameters = GM.Instance.SaveGame.DungeonParameters;
            SceneManager.LoadScene("Dungeon");
        }
        catch (Exception) {}
    }
}
