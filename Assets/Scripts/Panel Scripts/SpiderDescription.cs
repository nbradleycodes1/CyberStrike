using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpiderDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject spiderPanel, blackSprite, closeButton, piecesText, ToPage2;


    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        spiderPanel.gameObject.SetActive(true);
        blackSprite.gameObject.SetActive(true);

        ToPage2.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        piecesText.gameObject.SetActive(false);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        closeButton.gameObject.SetActive(true);
        piecesText.gameObject.SetActive(true);
        ToPage2.gameObject.SetActive(true);

        spiderPanel.gameObject.SetActive(false);
        blackSprite.gameObject.SetActive(false);
    }
}
