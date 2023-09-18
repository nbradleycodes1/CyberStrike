using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AntDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject antPanel, blackSprite, closeButton, piecesText, ToPage2;


    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        antPanel.gameObject.SetActive(true);
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

        antPanel.gameObject.SetActive(false);
        blackSprite.gameObject.SetActive(false);
    }
}
