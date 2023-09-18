using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundtrack : MonoBehaviour
{
    public GameObject audio, on, off, audioClick, audioHover, Piece, pauseAudio, glitchAudio;

    public void OnClickedOn()
    {
        on.gameObject.SetActive(true);
        audioClick.gameObject.SetActive(true);
        audioHover.gameObject.SetActive(true);
        pauseAudio.gameObject.SetActive(true);
 
        glitchAudio.gameObject.SetActive(true);
        audio.gameObject.SetActive(true);
        off.gameObject.SetActive(false);
    }

    public void OnClickedOff()
    {
        off.gameObject.SetActive(true);
        audioClick.gameObject.SetActive(false);
        audioHover.gameObject.SetActive(false);
        pauseAudio.gameObject.SetActive(false);
        glitchAudio.gameObject.SetActive(false);
        audio.gameObject.SetActive(false);
        on.gameObject.SetActive(false);
    }
}
