using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpKey : MonoBehaviour
{
    private Text keyText;
    private GameObject winScreen;
    private int keysFound;
    private int totalKeys;

    void Start()
    {
        keyText = GM.Instance.Canvas.transform.Find("KeyCount").gameObject.GetComponent<Text>();
        winScreen = GM.Instance.Canvas.transform.Find("WinScreen").gameObject;
        winScreen.SetActive(false);

        keysFound = 0;
        totalKeys = GM.Instance.DungeonParameters.TotalKeys;
        RefreshKeyUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Key")
        {
            GameObject key = other.gameObject;
            while (key.transform.parent.gameObject.tag == "Key")
            {
                key = key.transform.parent.gameObject;
            }
            Destroy(key);

            ++keysFound;
            RefreshKeyUI();
            if (keysFound >= totalKeys)
            {
                ShowWinScreen();
            }
        }
    }

    private void RefreshKeyUI()
    {
        keyText.text = keysFound + "/" + totalKeys;
    }

    private void ShowWinScreen()
    {
        winScreen.SetActive(true);
        GM.Instance.GameState = GameState.Won;
    }
}
