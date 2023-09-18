using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HardPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hardPanel, easyButton;


    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        hardPanel.gameObject.SetActive(true);
        easyButton.gameObject.SetActive(false);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        hardPanel.gameObject.SetActive(false);
        easyButton.gameObject.SetActive(true);
    }
}
