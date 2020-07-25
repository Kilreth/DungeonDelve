using UnityEngine;
using TMPro;

public class PickUpKey : MonoBehaviour
{
    public int KeysCollected { get; private set; }
    private int totalKeys;
    private TextMeshProUGUI keyText;
    private GameObject winScreen;

    public void InitializeKeyCounter(int keysCollected = 0)
    {
        keyText = GM.Instance.Canvas.transform.Find("KeySprite"
                        ).gameObject.transform.Find("KeyCount").GetComponent<TextMeshProUGUI>();
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
