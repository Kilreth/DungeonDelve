using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitDungeon : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && GM.Instance.GameState == GameState.Active)
        {
            GM.Instance.SaveGameToFile();
            SceneManager.LoadScene("MainMenu");
        }
    }
}
