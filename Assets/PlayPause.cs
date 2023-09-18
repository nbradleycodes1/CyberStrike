using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayPause : MonoBehaviour
{
    [Header("Play/Pause audio when active", order=1)]
    [SerializeField] GameObject obj1, obj2;

    void Update()
    {
        // If any of these objs are active
        if (
            (obj1 != null && obj1.active)
            || (obj2 != null && obj2.active)
            )
        {
            // pause the audio
            GetComponent<AudioSource>().Pause();
        }
        else
        {
            if (!GetComponent<AudioSource>().isPlaying)
            {
                // play the audio
                GetComponent<AudioSource>().Play();
            }
        }
    }
}
