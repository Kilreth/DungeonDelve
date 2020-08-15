using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Jukebox : MonoBehaviour
{
    public static Jukebox Instance;

    public AudioSource AudioSourceBGM;
    public AudioSource AudioSourceSFX;

    public float BGMVolume;
    public float SFXVolume;

    [SerializeField]
    private AudioClip[] soundClips = null;
    [SerializeField]
    private string[] soundNames = null;
    private Dictionary<string, AudioClip> soundNameToClip;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
            InitializeJukebox();
        }
    }

    private void InitializeJukebox()
    {
        AudioSourceBGM.loop = true;
        soundNameToClip = new Dictionary<string, AudioClip>();
        for (int i = 0; i < soundNames.Length; ++i)
            soundNameToClip[soundNames[i]] = soundClips[i];

        AudioSourceBGM.volume = BGMVolume = PlayerPrefs.GetFloat("BGM volume", 1);
        AudioSourceSFX.volume = SFXVolume = PlayerPrefs.GetFloat("SFX volume", 1);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioSourceBGM.Stop();
        AudioSourceSFX.Stop();
        if (scene.name == "MainMenu")
        {
            AudioSourceBGM.clip = soundNameToClip["Title theme"];
            AudioSourceBGM.Play();
        }
        else if (scene.name == "Dungeon")
        {
            AudioSourceBGM.clip = soundNameToClip["Dungeon theme"];
            AudioSourceBGM.Play();
        }
    }

    public void PlaySFX(string name, float volumeScale=1)
    {
        AudioSourceSFX.PlayOneShot(soundNameToClip[name], volumeScale);
    }

    // If multiple sound names are given, play one at random.
    // Useful for adding variation.
    public void PlaySFX(string[] names, float volumeScale=1)
    {
        PlaySFX(names[UnityEngine.Random.Range(0, names.Length)], volumeScale);
    }

    public void DuckBGM()
    {
        StartCoroutine(DuckBGMImpl());
    }

    private IEnumerator DuckBGMImpl()
    {
        float startTime = Time.time;
        float time = 0;
        while (time < 6 && GM.Instance.GameState != GameState.MainMenu)
        {
            time = Time.time - startTime;
            if (time <= 0.5f)
                AudioSourceBGM.volume = BGMVolume * (1 - time * 2);
            else if (time <= 3)
                AudioSourceBGM.volume = 0;
            else
                AudioSourceBGM.volume = BGMVolume * (time - 3) / 3;
            yield return null;
        }
        AudioSourceBGM.volume = BGMVolume;
    }
}
