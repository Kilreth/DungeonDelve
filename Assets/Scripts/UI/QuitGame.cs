using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }
}
