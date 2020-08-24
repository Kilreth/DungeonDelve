using UnityEngine;
using UnityEngine.EventSystems;

public class OptionSlider : SliderText, IPointerUpHandler
{
    public string sliderName;

    void OnEnable()
    {
        if (sliderName == "BGM")
            slider.value = Jukebox.Instance.BGMVolume;
        else if (sliderName == "SFX")
            slider.value = Jukebox.Instance.SFXVolume;
        else if (sliderName == "Graphics")
            slider.value = PlayerPrefs.GetInt("Graphics", 1);
    }

    public override void UpdateText()
    {
        if (sliderName == "BGM")
        {
            base.UpdateText();
            Jukebox.Instance.AudioSourceBGM.volume = Jukebox.Instance.BGMVolume = slider.value;
            PlayerPrefs.SetFloat("BGM volume", slider.value);
        }
        else if (sliderName == "SFX")
        {
            base.UpdateText();
            Jukebox.Instance.AudioSourceSFX.volume = Jukebox.Instance.SFXVolume = slider.value;
            PlayerPrefs.SetFloat("SFX volume", slider.value);
        }
        else if (sliderName == "Graphics")
        {
            text.text = slider.value == 1 ? "High" : "Low";
            PlayerPrefs.SetInt("Graphics", (int)slider.value);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (sliderName == "SFX")
        {
            Jukebox.Instance.AudioSourceSFX.Stop();
            Jukebox.Instance.PlaySFX("Pick up key");
        }
    }
}
