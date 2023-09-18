using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine.SceneManagement;

public class DragDrop : MonoBehaviourPunCallbacks, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Canvas _canvas;
    public static Func<string, int, bool> IsMyTurnEvent;
    public static Action<string> DragBeganEvent;
    public static Action<string, float, float, float> DraggingEvent;
    public static Action<string, float, float> DragEndedEvent;
    public static Action<string> PointerEnterEvent;
    public static Action<string> PointerExitEvent;
    public static Action UntwistPiecesEvent;
    public static bool isOnline;
    public static bool isMyTurn;
    private Vector2 _lastEnteredPoint;
    private Vector2 _lastExitedPoint;
    private Vector2 _startPosition;
    public static int MyOnlineColor;
    private PhotonView _myPhotonView;
    private RectTransform _myRT;
    public static bool animateStartingPieces;
    private GameObject _myHive;
    private bool isMoveWithinHive;
    void Awake()
    {
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _myRT = GetComponent<RectTransform>();
        _myPhotonView = GetComponent<PhotonView>();
        ColorPicker.ColorWasPickedEvent += ColorWasPickedHandler;
        _myHive = GameObject.Find("Canvas/Hive");
    }

    private void ColorWasPickedHandler(int pickedColor)
    {
        MyOnlineColor = pickedColor;
        // Debug.Log($"Color was picked! {pickedColor} - {_color}");
    }

    // For animation on start
    void Start()
    {
        if (animateStartingPieces)
        {
            _startPosition = _myRT.anchoredPosition;
            _myRT.anchoredPosition += new Vector2(20f, 20f);
            StartCoroutine(FloatDown());
        }
    }

    IEnumerator FloatDown()
    {
        while(_myRT.anchoredPosition.y > _startPosition.y)
        {
            _myRT.anchoredPosition -= new Vector2(1f, 1f);
            yield return new WaitForSeconds (0.01f);
        }
    }

    // Keeping this just in case
    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     // RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.Find("Canvas").GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out Vector2 localClickPos);
    //     // Vector3 localObjectPos = _myRT.InverseTransformPoint(localClickPos);
    //     // Vector3 objectPos = _myRT.TransformPoint(localObjectPos);
    //     // float distance = Vector2.Distance(objectPos, GetComponent<RectTransform>().anchoredPosition);
    //     // _allowDragging = distance <= 15f;
    // }

    [PunRPC]
    private void _EmitIsMyTurnEvent()
    {
        // Ask GameManager
        if (IsMyTurnEvent(gameObject.name, MyOnlineColor) && !GameManager.IsAISearching)
        {
            isMyTurn = true;
        }
        else
        {
            isMyTurn = false;
            // Debug.Log("Not my turn!");
        }
    }


    // For animation
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Make sure this piece can move
        if (IsMyTurnEvent != null && !eventData.dragging)
        {
            if (isOnline)
            {
                // Call it on all clients
                _myPhotonView.RPC("_EmitIsMyTurnEvent", RpcTarget.All);
            }
            else
            {
                // Call it on local only
                _EmitIsMyTurnEvent();
            }
        }

        // Debug.Log($"Mouse: {Camera.main.ScreenToWorldPoint(Input.mousePosition)}");

        _lastEnteredPoint = eventData.position;
        /**
        eventData.delta != Vector2.zero is important so that this method does not get invoked again
        when the pointer somehow has not moved a single pixel. Otherwise, the GUI will get buggy
        **/

        // Debug.Log($"ENTER Object receing ondrag {eventData.pointerDrag}");
        if (!eventData.dragging && _lastEnteredPoint != _lastExitedPoint && eventData.delta != Vector2.zero)
        {
            PointerEnterEvent(gameObject.tag);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _lastExitedPoint = eventData.position;
        /**
        eventData.delta != Vector2.zero is important so that this method does not get invoked again
        when the pointer somehow has not moved a single pixel. Otherwise, the GUI will get buggy
        **/
        // Debug.Log($"EXIT enabled {_isHoverEnabled}");
        // Debug.Log($"EXIT Object receing ondrag {eventData.pointerDrag}");
        if (!eventData.dragging && _lastExitedPoint != _lastEnteredPoint && eventData.delta != Vector2.zero)
        {
            PointerExitEvent(gameObject.tag);
        }
    }

    [PunRPC]
    private void DragMeTo(float x, float y)
    {
        _myRT.SetParent(_myHive.transform, false);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
    }

    [PunRPC]
    private void _EmitDragBeganEvent()
    {
        isMoveWithinHive = _myRT.parent.name == "Hive";
        // Tell GameManager that this piece started moving
        DragBeganEvent(gameObject.tag);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isMyTurn && eventData.button == PointerEventData.InputButton.Left)
        {
            if (DragBeganEvent != null)
            {
                if (isOnline)
                {
                    // Call it on all clients
                    _myPhotonView.RPC("_EmitDragBeganEvent", RpcTarget.All);
                }
                else
                {
                    // Call it on local only
                    _EmitDragBeganEvent();
                }
            }
        }
    }

    [PunRPC]
    private void _EmitDraggingEvent(float deltaX, float deltaY, float canvasScaleFactor)
    {
        DraggingEvent(gameObject.tag, deltaX, deltaY, canvasScaleFactor);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isMyTurn)
            {
                if (DraggingEvent != null)
                {
                    // Tell GameManager that this piece is moving
                    if (isOnline)
                    {
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.Find("Canvas/Hive").GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out Vector2 localClickPos2);
                        Vector3 localObjectPos2 = _myRT.InverseTransformPoint(localClickPos2);
                        Vector3 objectPos2 = _myRT.TransformPoint(localObjectPos2);

                        if (isMoveWithinHive)
                        {
                            // what we had before
                            _myPhotonView.RPC("_EmitDraggingEvent", RpcTarget.All, eventData.delta.x, eventData.delta.y, _canvas.scaleFactor);
                        }
                        else
                        {
                            if (GameManager.allowPieceMove)
                            {
                                _myPhotonView.RPC("DragMeTo", RpcTarget.Others, objectPos2.x, objectPos2.y);
                            }
                            _EmitDraggingEvent(eventData.delta.x, eventData.delta.y, _canvas.scaleFactor);
                        }
                    }
                    else
                    {
                        // Call it on local only
                        _EmitDraggingEvent(eventData.delta.x, eventData.delta.y, _canvas.scaleFactor);
                    }
                }
            }
        }
        if (SceneManager.GetActiveScene().name != "TutorialBoard" )
        {
            if (isOnline)
            {
                if (MyOnlineColor == 1 && GameManager.whoseTurn == HiveCore.Utils.Player.White)
                {
                    if (eventData.button == PointerEventData.InputButton.Right && _myHive.transform.childCount > 0)
                    {
                        UntwistPiecesEvent();
                        _myHive.GetComponent<RectTransform>().anchoredPosition += new Vector2(eventData.delta.x / _canvas.scaleFactor, eventData.delta.y / _canvas.scaleFactor);
                    }
                }
                else if (MyOnlineColor == 2 && GameManager.whoseTurn == HiveCore.Utils.Player.Black)
                {
                    if (eventData.button == PointerEventData.InputButton.Right && _myHive.transform.childCount > 0)
                    {
                        UntwistPiecesEvent();
                        _myHive.GetComponent<RectTransform>().anchoredPosition += new Vector2(eventData.delta.x / _canvas.scaleFactor, eventData.delta.y / _canvas.scaleFactor);
                    }
                }
            }
            else
            {
                if (eventData.button == PointerEventData.InputButton.Right && _myHive.transform.childCount > 0)
                {
                    UntwistPiecesEvent();
                    _myHive.GetComponent<RectTransform>().anchoredPosition += new Vector2(eventData.delta.x / _canvas.scaleFactor, eventData.delta.y / _canvas.scaleFactor);
                }
            }
        }
    }


    [PunRPC]
    private void _EmitDragEndedEvent(float toX, float toY)
    {
        // Tell GameManager that this piece finished moving
        DragEndedEvent(gameObject.tag, toX, toY);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isMyTurn && eventData.button == PointerEventData.InputButton.Left)
        {
            if (DragEndedEvent != null)
            {
                RectTransform myRT = gameObject.GetComponent<RectTransform>();
                if (isOnline)
                {
                    // Call it on all clients
                    _myPhotonView.RPC("_EmitDragEndedEvent", RpcTarget.All, myRT.anchoredPosition.x, myRT.anchoredPosition.y);
                }
                else
                {
                    // Call it on local only
                    _EmitDragEndedEvent(myRT.anchoredPosition.x, myRT.anchoredPosition.y);
                }
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isMyTurn && eventData.button == PointerEventData.InputButton.Left)
        {
            // This method gets invoked when the piece is placed on top of another piece
            // Debug.Log("OnDrop");
            // throw new System.NotImplementedException();
        }
    }

}