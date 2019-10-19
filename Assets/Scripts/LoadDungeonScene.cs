using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadDungeonScene : MonoBehaviour
{
    [SerializeField]
    private DungeonParameters parameters;

    public void Load()
    {
        GM.Instance.DungeonParameters = parameters;
        SceneManager.LoadScene("Dungeon");
    }
}
