using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EasyPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject easyPanel;


    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        easyPanel.gameObject.SetActive(true);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        easyPanel.gameObject.SetActive(false);
    }
}
