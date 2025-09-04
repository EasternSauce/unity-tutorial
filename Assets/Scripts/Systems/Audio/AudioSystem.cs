using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    [SerializeField] AudioSource mainAudioSource;

    public void Play(AudioClip audioClip)
    {
        mainAudioSource.clip = audioClip;
        mainAudioSource.Play();
    }
}
