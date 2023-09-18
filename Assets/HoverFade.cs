using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverFade : MonoBehaviour
{
    void Awake()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        gameObject.SetActive(false);
    }

    public void MakeFadeIn()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }
    public void MakeFadeOut()
    {
        StartCoroutine(FadeOut());
        gameObject.SetActive(false);
    }

    private IEnumerator FadeOut()
    {
        for (float i = 1f; i >= 0; i -= 0.05f)
        {
            GetComponent<CanvasGroup>().alpha = i;
            yield return new WaitForSeconds(0.01f);
        }
        GetComponent<CanvasGroup>().alpha = 0;
    }

    private IEnumerator FadeIn()
    {
        for (float i = 0; i < 1; i += 0.05f)
        {
            GetComponent<CanvasGroup>().alpha = i;
            yield return new WaitForSeconds(0.01f);
        }
        GetComponent<CanvasGroup>().alpha = 1;
    }
}
