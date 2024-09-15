using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public bool isMute = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {

    }

    void Update()
    {
        if (isMute)
            AudioListener.volume = 0;
        else
            AudioListener.volume = 1;
    }

    public void SetBGM(Vector3 pos, AudioClip bgm, bool isLoop)
    {
        if (isMute) return;
        GameObject sound = new GameObject("BGM");
        sound.transform.SetParent(transform);
        sound.transform.position = pos;
        AudioSource audioSource = sound.AddComponent<AudioSource>();

        audioSource.clip = bgm;
        audioSource.loop = isLoop;
        audioSource.minDistance = 20;
        audioSource.maxDistance = 50;
        audioSource.volume = 1f;
        audioSource.Play();
    }
    public void PlaySound(Vector3 pos, AudioClip sfx)
    {
        if (isMute) return;
        GameObject sound = new GameObject("SFX");
        sound.transform.SetParent(transform);
        sound.transform.position = pos;
        AudioSource audioSource = sound.AddComponent<AudioSource>();
        
        audioSource.clip = sfx;
        audioSource.minDistance = 20;
        audioSource.maxDistance = 50;
        audioSource.volume = 1f;
        audioSource.Play();
    }
}
