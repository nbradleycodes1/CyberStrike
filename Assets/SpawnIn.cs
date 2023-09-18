using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnIn : MonoBehaviour
{
    public float delayBySeconds = 2f;
    private bool allowCalls = true;
    void Start()
    {
        if (allowCalls)
            Invoke("Run", delayBySeconds);
    }

    private void Run()
    {
        if (allowCalls && gameObject.active)
        {
            allowCalls = false;
            StartCoroutine(FadeIn());
        }
    }

    protected IEnumerator FadeIn()
    {
        for (float i = 0f; i < 1f; i += 0.05f)
        {
            GetComponent<CanvasGroup>().alpha = i;
            yield return new WaitForSeconds(0.01f);
        }
        GetComponent<CanvasGroup>().alpha = 1f;
    }
}
