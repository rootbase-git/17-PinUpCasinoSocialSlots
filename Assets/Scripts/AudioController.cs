using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioController : MonoBehaviour
{
    public AudioClip buttonPress;
    public AudioClip win;
    public AudioClip spinning;
    public AudioClip click;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(AudioClip clip, bool loop)
    {
        //_audioSource.clip = clip;
        if (loop)
        {
            _audioSource.loop = true;
            _audioSource.volume = .25f;
            _audioSource.pitch = Random.Range(2.1f, 2.3f);
            _audioSource.clip = clip;
            _audioSource.Play();
        }
        else
        {
            _audioSource.loop = false;
            _audioSource.volume = 1;
            _audioSource.pitch = 1;
            _audioSource.PlayOneShot(clip);
        }
    }

    public void StopAudio()
    {
        _audioSource.Stop();
    }

}
