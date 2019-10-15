using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpKey : MonoBehaviour
{
    private Text KeyText;
    private GameObject WinScreen;
    private int keysFound = 0;

    void Start()
    {
        KeyText = GM.Instance.Canvas.transform.Find("KeyCount").gameObject.GetComponent<Text>();
        WinScreen = GM.Instance.Canvas.transform.Find("WinScreen").gameObject;
        WinScreen.SetActive(false);
        RefreshUI();
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
            RefreshUI();
            if (keysFound >= GM.Instance.TotalKeys)
            {
                ShowWinScreen();
            }
        }
    }

    private void RefreshUI()
    {
        KeyText.text = keysFound + "/" + GM.Instance.TotalKeys;
    }

    private void ShowWinScreen()
    {
        WinScreen.SetActive(true);
        GM.Instance.GameState = GameState.Won;
    }
}
