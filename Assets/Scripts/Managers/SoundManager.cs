using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Mixers")]
    public AudioMixer audioMixer;

    [Header("Mixer Parameters")]
    public string musicVolumeParameter = "MusicVolume";
    public string sfxVolumeParameter = "SFXVolume";

    [Header("Audio Clips")]
    public AudioClip[] musicTracks;
    public AudioClip[] soundEffects;

    [Header("UI Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private const string MusicVolumePref = "MusicVolume";
    private const string SFXVolumePref = "SFXVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;

        musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
        sfxSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
    }

    private void Start()
    {
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumePref, 0.5f);
        float sfxVolume = PlayerPrefs.GetFloat(SFXVolumePref, 0.5f);

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat(musicVolumeParameter, Mathf.Log10(volume) * 80);
        PlayerPrefs.SetFloat(MusicVolumePref, volume);
    }

    public void SetSFXVolume(float volume)
    {
        
        audioMixer.SetFloat(sfxVolumeParameter, Mathf.Log10(volume) * 80);
        PlayerPrefs.SetFloat(SFXVolumePref, volume);
    }

    public void PlayMusic(int trackIndex)
    {
        if (trackIndex >= 0 && trackIndex < musicTracks.Length)
        {
            if (musicSource.clip == musicTracks[trackIndex] && musicSource.isPlaying)
            {
                Debug.Log("The track is already playing.");
                return;
            }

            musicSource.clip = musicTracks[trackIndex];
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Invalid music track index.");
        }
    }


    public void PlaySFX(int sfxIndex)
    {
        if (sfxIndex >= 0 && sfxIndex < soundEffects.Length)
        {
            sfxSource.PlayOneShot(soundEffects[sfxIndex]);
        }
        else
        {
            Debug.LogWarning("Invalid sound effect index.");
        }
    }
}
