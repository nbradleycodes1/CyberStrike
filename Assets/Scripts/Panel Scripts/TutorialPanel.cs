using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tutorialPanel;

    
        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            tutorialPanel.gameObject.SetActive(true);
        }

        //Detect when Cursor leaves the GameObject
        public void OnPointerExit(PointerEventData pointerEventData)
        {
            tutorialPanel.gameObject.SetActive(false);
        }
    }

