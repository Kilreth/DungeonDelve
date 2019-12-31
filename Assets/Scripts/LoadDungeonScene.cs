using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadDungeonScene : MonoBehaviour
{
    [SerializeField]
    private DungeonParameters parameters;

    public void LoadNewDungeon()
    {
        GM.Instance.SaveGame = null;
        GM.Instance.DungeonParameters = parameters;
        SceneManager.LoadScene("Dungeon");
    }

    public void LoadDungeonFromSave()
    {
        GM.Instance.SaveGame = SaveGameSystem.LoadGameFromFile();
        GM.Instance.DungeonParameters = GM.Instance.SaveGame.DungeonParameters;
        SceneManager.LoadScene("Dungeon");
    }
}
