using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Jukebox : MonoBehaviour
{
    public static Jukebox Instance;

    [SerializeField]
    private AudioSource audioSourceBGM;
    [SerializeField]
    private AudioSource audioSourceSFX;

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
        audioSourceBGM.loop = true;
        soundNameToClip = new Dictionary<string, AudioClip>();
        for (int i = 0; i < soundNames.Length; ++i)
            soundNameToClip[soundNames[i]] = soundClips[i];
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
        audioSourceBGM.Stop();
        audioSourceSFX.Stop();
        if (scene.name == "MainMenu")
        {
            audioSourceBGM.clip = soundNameToClip["Title theme"];
            audioSourceBGM.Play();
        }
        else if (scene.name == "Dungeon")
        {
            audioSourceBGM.clip = soundNameToClip["Dungeon theme"];
            audioSourceBGM.Play();
        }
    }

    public void PlaySFX(string name, float volumeScale=1)
    {
        audioSourceSFX.PlayOneShot(soundNameToClip[name], volumeScale);
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
                audioSourceBGM.volume = 1 - time * 2;
            else if (time <= 3)
                audioSourceBGM.volume = 0;
            else
                audioSourceBGM.volume = (time - 3) / 3;
            yield return null;
        }
        audioSourceBGM.volume = 1;
    }
}
