using UnityEngine;
using UnityEngine.EventSystems;

public class VolumeSlider : SliderText, IPointerUpHandler
{
    public string volumeType;

    void OnEnable()
    {
        if (volumeType == "BGM")
            slider.value = Jukebox.Instance.BGMVolume;
        else if (volumeType == "SFX")
            slider.value = Jukebox.Instance.SFXVolume;
    }

    public override void UpdateText()
    {
        base.UpdateText();
        if (volumeType == "BGM")
            Jukebox.Instance.AudioSourceBGM.volume = Jukebox.Instance.BGMVolume = slider.value;
        else if (volumeType == "SFX")
            Jukebox.Instance.AudioSourceSFX.volume = Jukebox.Instance.SFXVolume = slider.value;

        PlayerPrefs.SetFloat(volumeType + " volume", slider.value);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (volumeType == "SFX")
        {
            Jukebox.Instance.AudioSourceSFX.Stop();
            Jukebox.Instance.PlaySFX("Pick up key");
        }
    }
}
