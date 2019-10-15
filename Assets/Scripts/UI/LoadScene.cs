using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void Load(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void LoadStandardDungeon()
    {
        GM.Instance.Rows = 30;
        GM.Instance.Cols = 40;
        GM.Instance.TotalKeys = 4;
        Load("Dungeon");
    }

    public void LoadExpandedDungeon()
    {
        GM.Instance.Rows = 60;
        GM.Instance.Cols = 80;
        GM.Instance.TotalKeys = 16;
        Load("Dungeon");
    }
}
