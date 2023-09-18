using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnim2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject buttonHoverAnim2;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        buttonHoverAnim2.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        buttonHoverAnim2.gameObject.SetActive(false);
    }
}
