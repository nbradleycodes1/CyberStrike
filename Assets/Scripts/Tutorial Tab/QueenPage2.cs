using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QueenPage2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject queenBlack, piece1, piece2, piece3, piece4, piece5, piece6;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        queenBlack.gameObject.SetActive(true);
        piece1.gameObject.SetActive(true);
        piece2.gameObject.SetActive(true);
        piece3.gameObject.SetActive(true);
        piece4.gameObject.SetActive(true);
        piece5.gameObject.SetActive(true);
        piece6.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        queenBlack.gameObject.SetActive(false);
        piece1.gameObject.SetActive(false);
        piece2.gameObject.SetActive(false);
        piece3.gameObject.SetActive(false);
        piece4.gameObject.SetActive(false);
        piece5.gameObject.SetActive(false);
        piece6.gameObject.SetActive(false);
    
    }
}
