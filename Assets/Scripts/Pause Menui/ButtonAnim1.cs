using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnim1 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject buttonHoverAnim1;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        buttonHoverAnim1.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        buttonHoverAnim1.gameObject.SetActive(false);
    }
}
