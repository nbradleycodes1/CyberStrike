using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShuffleColors : MonoBehaviour
{
    // Update is called once per frame
    private bool allow = true;
    public float speed = 1.0f;
    public float maxColorValue = 255f;
    private Color32 curColor;

    void Start()
    {
        curColor = Color.white;
    }
    void Update()
    {
        if (allow)
            StartCoroutine(ColorAnim());
    }

    private IEnumerator ColorAnim()
    {
        allow = false;
        for (float t = 0f; t < 1f; t += Time.deltaTime * speed)
        {
            // float r = GetComponent<Image>().color.r * maxColorValue;
            // float g = GetComponent<Image>().color.g * maxColorValue;
            // float b = GetComponent<Image>().color.b * maxColorValue;


            float r = Mathf.Lerp(GetComponent<Image>().color.r, maxColorValue, t);
            float g = Mathf.Lerp(GetComponent<Image>().color.g, maxColorValue, t);
            float b = Mathf.Lerp(GetComponent<Image>().color.b, maxColorValue, t);

            Color32 newColor = new Color32((byte)r, (byte)g, (byte)b, 1);

            curColor = Color32.Lerp(curColor, newColor, t);
            GetComponent<Image>().color = curColor;
            yield return null;


            // float r = Mathf.Lerp(GetComponent<Image>().color.r, maxColorValue, t);
            // float g = Mathf.Lerp(GetComponent<Image>().color.g, maxColorValue, t);
            // float b = Mathf.Lerp(GetComponent<Image>().color.b, maxColorValue, t);
            // Color32 newColor = new Color32(r, g, b);

            // curColor = Color32.Lerp(curColor, newColor, t);
            // GetComponent<Image>().color = curColor;
            // yield return null;
        }
    }
}
