using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchPanelInGame : MonoBehaviour
{
    public GameObject Glitch;
    void Start()
    {
        Invoke("Show", 0f);
    }

    private void Show(){
        Glitch.gameObject.SetActive(true);
        Invoke("Hide", 1f);
    }

     private void Hide(){
        Glitch.gameObject.SetActive(false);
        
    }
}
