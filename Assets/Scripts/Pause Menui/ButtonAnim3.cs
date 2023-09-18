using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnim3 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject buttonHoverAnim3;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        buttonHoverAnim3.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        buttonHoverAnim3.gameObject.SetActive(false);
    }
}
