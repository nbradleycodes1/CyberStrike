using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialPanelNew : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject TutPanel;


    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        TutPanel.gameObject.SetActive(true);

    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        TutPanel.gameObject.SetActive(false);
    }
}
