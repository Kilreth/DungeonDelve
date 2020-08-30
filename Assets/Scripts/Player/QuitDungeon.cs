using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitDungeon : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (GM.Instance.GameState != GameState.Won)
                GM.Instance.SaveGameToFile();

            SceneManager.LoadScene("MainMenu");
        }
    }
}
