using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Text text;

    public void UpdateKeysText()
    {
        int scaled = CustomizeDungeon.KeysSliderToRealValue((int)slider.value, (int)slider.maxValue);
        text.text = scaled == Int32.MaxValue ? "<b>~</b>" : scaled.ToString();
    }

    public void UpdateText()
    {
        if (slider.wholeNumbers)
            text.text = slider.value.ToString();
        else
            text.text = String.Format("{0:P0}", slider.value).Replace(" %", "%");
    }
}
