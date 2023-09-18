using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Play_GlitchEffect : MonoBehaviour
{
    AudioSource aud;
    void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void play_sound()
    {
        aud.Play();
    }
}
