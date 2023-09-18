using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchForINGame : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject GlitchAnimation;

    void Start()
    {
        Invoke("Show", 0f);
    }

    private void Show()
    {
        GlitchAnimation.gameObject.SetActive(true);
        Invoke("Hide2", 1f);
    }

    private void Hide()
    {
        GlitchAnimation.gameObject.SetActive(false);
    }
}
