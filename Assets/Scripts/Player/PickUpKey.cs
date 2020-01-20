using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpKey : MonoBehaviour
{
    public int KeysCollected { get; private set; }
    private int totalKeys;
    private Text keyText;
    private GameObject winScreen;

    public void InitializeKeyCounter(int keysCollected = 0)
    {
        keyText = GM.Instance.Canvas.transform.Find("KeyCount").gameObject.GetComponent<Text>();
        winScreen = GM.Instance.Canvas.transform.Find("WinScreen").gameObject;
        winScreen.SetActive(false);

        KeysCollected = keysCollected;
        totalKeys = GM.Instance.DungeonParameters.TotalKeys;
        RefreshKeyUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Key")
        {
            Destroy(other.gameObject);
            ++KeysCollected;
            RefreshKeyUI();
            if (KeysCollected >= totalKeys)
            {
                ShowWinScreen();
            }
        }
    }

    private void RefreshKeyUI()
    {
        keyText.text = KeysCollected + "/" + totalKeys;
    }

    private void ShowWinScreen()
    {
        winScreen.SetActive(true);
        GM.Instance.GameState = GameState.Won;
    }
}
