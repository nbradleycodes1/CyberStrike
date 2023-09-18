using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MultiPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject multiPanel, singleButton;


    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        multiPanel.gameObject.SetActive(true);
        singleButton.gameObject.SetActive(false);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        multiPanel.gameObject.SetActive(false);
        singleButton.gameObject.SetActive(true);
    }
}
