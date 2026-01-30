using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SceneMusic : MonoBehaviour
{
    public AudioClip music;        
    [Range(0f, 1f)] public float volume = 0.6f;
    public bool loop = true;
    public bool playOnStart = true;

    private AudioSource a;

    void Awake()
    {
        a = GetComponent<AudioSource>();

        a.playOnAwake = false;       
        a.loop = loop;
        a.volume = volume;
        a.spatialBlend = 0f;        
    }

    void Start()
    {
        if (!playOnStart) return;
        if (music == null) return;

        a.clip = music;
        a.Play();
    }
}
