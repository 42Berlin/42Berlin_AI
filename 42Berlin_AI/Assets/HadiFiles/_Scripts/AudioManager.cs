using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static public AudioManager instance;
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    public int testClipNumber;
    // Start is called before the first frame update

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // AudioClipPlayer(testClipNumber);
    }

    public void AudioClipPlayer(int clipNumber)
    {
        audioSource.clip = audioClips[clipNumber];
        audioSource.Play();
    }
}
