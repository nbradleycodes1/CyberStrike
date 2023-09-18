using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Color32 _myColor;
    private Color32 _myColorChild;
    void Awake()
    {
        _myColor = GetComponent<Image>().color;
        _myColorChild = transform.GetChild(0).GetComponent<Image>().color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DragDrop.isMyTurn)
        {
            GetComponent<Image>().color = new Color32((byte) 247, (byte) 127, (byte) 0, 255);
            transform.GetChild(0).GetComponent<Image>().color = new Color32((byte) 247, (byte) 127, (byte) 0, 50);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DragDrop.isMyTurn)
        {
            GetComponent<Image>().color = _myColor;
            transform.GetChild(0).GetComponent<Image>().color = _myColorChild;
        }
    }
}
