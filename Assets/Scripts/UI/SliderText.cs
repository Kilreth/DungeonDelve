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

    public void UpdateText()
    {
        if (slider.wholeNumbers)
        {
            text.text = slider.value.ToString();
        }
        else
        {
            text.text = String.Format("{0:P0}", slider.value).Replace(" %", "%");
        }
    }
}
