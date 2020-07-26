using System.Linq;
using UnityEngine;
using TMPro;

public class PickUpKey : MonoBehaviour
{
    public int KeysCollected { get; private set; }
    private int totalKeys;
    private TextMeshProUGUI keyText;
    private GameObject winScreen;
    private TextMeshProUGUI timeElapsedText;

    public void InitializeKeyCounter(int keysCollected = 0)
    {
        keyText = GM.Instance.Canvas.transform.Find("KeySprite"
                        ).gameObject.transform.Find("KeyCount").GetComponent<TextMeshProUGUI>();
        winScreen = GM.Instance.Canvas.transform.Find("WinScreen").gameObject;
        winScreen.SetActive(false);
        timeElapsedText = winScreen.GetComponentsInChildren<TextMeshProUGUI>().First(
            x => x.name == "TimeElapsedText");

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
        timeElapsedText.text = string.Format("You cleared the dungeon in {0}!",
            TimeElapsedText.FormatTimeElapsed(GM.Instance.GetTimeElapsed()));
        GM.Instance.GameState = GameState.Won;
    }
}
