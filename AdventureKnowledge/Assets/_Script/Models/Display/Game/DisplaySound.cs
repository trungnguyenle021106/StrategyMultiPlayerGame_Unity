using UnityEngine;

public class DisplaySound : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip healdefActionSound;
    [SerializeField] AudioClip buffSound;
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip waterSound;
    [SerializeField] AudioClip windSound;
    [SerializeField] AudioClip thunderSound;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] AudioClip acceptSound;
    [SerializeField] AudioClip cancelSound;

    private void Start()
    {
        audioSource.volume = PlayerPrefs.GetFloat("soundGameVolume");
    }

    public void PlaySound(string nameSound)
    {
        switch (nameSound)
        {
            case "healdefActionSound":
                audioSource.clip = healdefActionSound;
                break;
            case "buffSound":
                audioSource.clip = buffSound;
                break;
            case "fireSound":
                audioSource.clip = fireSound;
                break;
            case "waterSound":
                audioSource.clip = waterSound;
                break;
            case "windSound":
                audioSource.clip = windSound;
                break;
            case "thunderSound":
                audioSource.clip = thunderSound;
                break;
            case "explosionSound":
                audioSource.clip = explosionSound;
                break;
            case "acceptSound":
                audioSource.clip = acceptSound;
                break;
            case "cancelSound":
                audioSource.clip = cancelSound;
                break;
            default:
                Debug.LogWarning("Sound name not recognized: " + nameSound);
                return;
        }
        audioSource.Play();
    }
}
