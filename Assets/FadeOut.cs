using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    private bool selfStart = false;
    private float seconds = 2f;
    void Start()
    {
        if (selfStart) Invoke("Run", seconds);
    }
    public void Run()
    {
        if (gameObject.active)
            StartCoroutine(AnimatedDestroy());
    }

    protected IEnumerator AnimatedDestroy()
    {
        for (float i = 1f; i >= 0; i -= 0.05f)
        {
            GetComponent<CanvasGroup>().alpha = i;
            yield return new WaitForSeconds(0.01f);
        }
        gameObject.SetActive(false);
    }
}
