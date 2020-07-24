using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScaleTextOnRatio : MonoBehaviour
{
    public TextMeshProUGUI TextPro;
    public Text Text;
    public float BaseRatio;
    public float DefaultSize;
    public float Power = 1;

    // Update is called once per frame
    void Update()
    {
        int fontSize = (int)(DefaultSize * Math.Pow(BaseRatio / Screen.width * Screen.height, Power));

        if (TextPro != null)
            TextPro.fontSize = fontSize;
        if (Text != null)
            Text.fontSize = fontSize;
    }
}
