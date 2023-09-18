using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DIsappearSelfOnKeyUp : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        GameObject.FindGameObjectWithTag("volumeslider")?.GetComponent<HoverFade>().MakeFadeOut();
    }
}
