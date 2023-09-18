using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerVsAIPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject coopPanel, tutButton;


    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        coopPanel.gameObject.SetActive(true);

        tutButton.gameObject.SetActive(false);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        coopPanel.gameObject.SetActive(false);

        tutButton.gameObject.SetActive(true);
    }
}
