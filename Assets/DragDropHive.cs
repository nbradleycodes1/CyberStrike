using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class DragDropHive : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Canvas canvas;
    private RectTransform _myRT;
    private bool canDrag = false;
    void Awake()
    {
        _myRT = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.Find("Canvas/Hive").GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out Vector2 localClickPos2);
        Vector3 localObjectPos2 = _myRT.InverseTransformPoint(localClickPos2);
        Vector3 objectPos2 = _myRT.TransformPoint(localObjectPos2);

        foreach (Transform child in GameObject.Find("Canvas/Hive").transform)
        {
            if (Vector2.Distance(objectPos2, child.gameObject.GetComponent<RectTransform>().anchoredPosition) <= 30f)
            {
                canDrag = true;
                return;
            }
        }
        canDrag = false;
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (canDrag && TutorialGameManager.dragScrollAllowed && eventData.button == PointerEventData.InputButton.Right)
            GetComponent<RectTransform>().anchoredPosition += new Vector2(eventData.delta.x / canvas.scaleFactor, eventData.delta.y / canvas.scaleFactor);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canDrag && TutorialGameManager.dragScrollAllowed && eventData.button == PointerEventData.InputButton.Right)
            GetComponent<RectTransform>().anchoredPosition = _myRT.anchoredPosition;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (canDrag && TutorialGameManager.dragScrollAllowed && eventData.button == PointerEventData.InputButton.Right)
            GetComponent<RectTransform>().anchoredPosition = _myRT.anchoredPosition;
    }
}