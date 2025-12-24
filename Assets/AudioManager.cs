using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Audio Sources")]
    [SerializeField] private AudioSource MusicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("Audio Clips")]
    public AudioClip click;
    public AudioClip EattingAudio;
    public AudioClip Happy;
    public AudioClip Buying;

    public AudioClip background;    

    public AudioClip OutfitChangedAudio;    
     public static AudioManager Instance;

    private void Start()
    {
        MusicSource.clip = background;
        MusicSource.Play();
    }

    public void playSFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
    
    public void StopBG()
    {
        MusicSource.Stop();
        
    }
    public void playBG()
    {
        MusicSource.Play();
        
    }

}
