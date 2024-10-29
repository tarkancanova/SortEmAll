using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip backgroundMusic;

    void Start()
    {
        DontDestroyOnLoad(this);

        audioSource = GetComponent<AudioSource>();

        PlayMusic();
    }

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}