using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SinglePanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject singlePanel, multiplayerButton;


    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        singlePanel.gameObject.SetActive(true);
        multiplayerButton.gameObject.SetActive(false);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        singlePanel.gameObject.SetActive(false);
        multiplayerButton.gameObject.SetActive(true);
    }
}