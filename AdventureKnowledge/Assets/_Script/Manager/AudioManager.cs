using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Slider bgVolumeSlider;
    [SerializeField] private Slider soundVolumeSlider;
    [SerializeField] private Slider soundGameVolumeSlider;
    [SerializeField] private AudioSource bgAudioSource;
    [SerializeField] private AudioSource soundAudioSource;
    [SerializeField] private AudioSource soundGameSource;
    [SerializeField] private AudioClip clickSound;


    private void Awake()
    {
        // Kiểm tra và đặt âm lượng cho soundAudioSource
        if (!PlayerPrefs.HasKey("soundVolume"))
        {
            PlayerPrefs.SetFloat("soundVolume", 1f);
            float savedSoundVolume = PlayerPrefs.GetFloat("soundVolume");
            soundAudioSource.volume = savedSoundVolume;
            soundVolumeSlider.value = PlayerPrefs.GetFloat("soundVolume");
        }
        else
        {
            soundVolumeSlider.value = PlayerPrefs.GetFloat("soundVolume");
            soundAudioSource.volume = PlayerPrefs.GetFloat("soundVolume");
        }


        // Kiểm tra và đặt âm lượng cho bgAudioSource nếu không null

        if (!PlayerPrefs.HasKey("backGroundVolume"))
        {
            PlayerPrefs.SetFloat("backGroundVolume", 1f);
            float savedBGVolume = PlayerPrefs.GetFloat("backGroundVolume");
            if (bgAudioSource != null)
            { bgAudioSource.volume = savedBGVolume; }
            bgVolumeSlider.value = savedBGVolume;
        }
        else
        {
            bgVolumeSlider.value = PlayerPrefs.GetFloat("backGroundVolume");
            if (bgAudioSource != null)
            { bgAudioSource.volume = PlayerPrefs.GetFloat("backGroundVolume"); }
        }



        if (!PlayerPrefs.HasKey("soundGameVolume"))
        {
            PlayerPrefs.SetFloat("soundGameVolume", 1f);
            if (soundGameSource != null) soundGameSource.volume = PlayerPrefs.GetFloat("soundGameVolume");
            soundGameVolumeSlider.value = PlayerPrefs.GetFloat("soundGameVolume");
        }
        else
        {
            if (soundGameSource != null) soundGameSource.volume = PlayerPrefs.GetFloat("soundGameVolume");
            soundGameVolumeSlider.value = PlayerPrefs.GetFloat("soundGameVolume");
        }
    }



    void Start()
    {

    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) { PlayClickSound(); }
    }

    void PlayClickSound() { soundAudioSource.PlayOneShot(clickSound); }

    public void SetSoundVolume()
    {
        PlayerPrefs.SetFloat("soundVolume", soundVolumeSlider.value);
        soundAudioSource.volume = soundVolumeSlider.value;
    }

    public void SetBGVolume()
    {
        PlayerPrefs.SetFloat("backGroundVolume", bgVolumeSlider.value);
        if (bgAudioSource != null) bgAudioSource.volume = bgVolumeSlider.value;
    }

    public void SetGameVolume()
    {
        PlayerPrefs.SetFloat("soundGameVolume", soundGameVolumeSlider.value);
        if (soundGameSource != null) soundGameSource.volume = PlayerPrefs.GetFloat("soundGameVolume");
    }
}
