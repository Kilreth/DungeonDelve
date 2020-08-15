using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    [SerializeField]
    protected Slider slider;
    [SerializeField]
    private Text text;

    public void UpdateKeysText()
    {
        int scaled = CustomizeDungeon.KeysSliderToRealValue((int)slider.value, (int)slider.maxValue);
        text.text = scaled == Int32.MaxValue ? "<b>~</b>" : scaled.ToString();
    }

    public virtual void UpdateText()
    {
        if (slider.wholeNumbers)
            text.text = slider.value.ToString();
        else
            text.text = FormatPercent(slider.value);
    }

    public static string FormatPercent(double d)
    {
        return string.Format("{0:P0}", d).Replace(" %", "%");
    }
}
