using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DestroyAfterAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        audioSource = audioSource? audioSource:  GetComponent<AudioSource>();

        // Automatically destroy after clip length
        Destroy(gameObject, audioSource.clip.length);
    }
}