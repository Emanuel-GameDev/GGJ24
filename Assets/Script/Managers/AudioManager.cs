using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static public AudioManager instance;

    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource effectSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void PlaySound(AudioClip clip)
    {
        effectSource.PlayOneShot(clip);
    }

    public void CanPlaySound(float time)
    {

    }

    public void ChangeMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }


}
