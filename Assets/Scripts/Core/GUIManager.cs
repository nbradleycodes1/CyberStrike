using HiveCore;
using static HiveCore.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    protected GameObject _handGameObj;
    protected GameObject _hiveGameObj;
    protected GameObject _canvasGameObj;
    public GameObject Msg;
    protected GameObject _whiteWonScreen;
    protected GameObject _blackWonScreen;
    protected GameObject _drawScreen;
    protected Board _boardRef;
    public bool ShowDroppingAnimation = true;
    public bool showGameOverScreen = true;
    public float DroppingAnimationDuration = 0.7f;
    public bool isTutorial = false;

    // This script needs to be loaded before GameManager.cs

    void Update()
    {
        if (!isTutorial && Msg.active && Msg.GetComponent<CanvasGroup>().alpha == 1f && Input.anyKeyDown)
        {
            Msg.GetComponent<FadeOut>().Run();
        }
    }

    void OnEnable()
    {
        // Setup references
        _canvasGameObj = GameObject.Find("Canvas");
        _hiveGameObj = GameObject.Find("Canvas/Hive");
        _handGameObj = GameObject.Find("Canvas/Hand");

        _whiteWonScreen = GameObject.FindGameObjectWithTag("whitewonscreen");
        _blackWonScreen = GameObject.FindGameObjectWithTag("blackwonscreen");
        _drawScreen = GameObject.FindGameObjectWithTag("drawscreen");
        _whiteWonScreen.SetActive(false);
        _blackWonScreen.SetActive(false);
        _drawScreen.SetActive(false);

        // Subscribe to the events from GameManager
        GameManager.SetupStartingGUIEvent += SetupStartingGUIHandler;
        GameManager.DragGUIPieceEvent += DragGUIPieceHandler;
        GameManager.DisplayGUIMovesEvent += DisplayGUIMovesHandler;
        GameManager.MakeGUIPieceActiveEvent += MakeGUIPieceActiveHandler;
        GameManager.DropValidGUIPieceEvent += DropValidGUIPieceHandler;
        GameManager.DropInvalidGUIPieceEvent += DropInvalidGUIPieceHandler;
        GameManager.GameIsOverEvent += GameIsOverHandler;
        GameManager.DisplayGUITextEvent += DisplayGUITextHandler;
        GameManager.MoveHiveToEvent += MoveHiveToHandler;
        GameManager.RestartHivePositionEvent += RestartHivePositionHandler;
        GameManager.DisplayGUITrackersEvent += DisplayGUITrackersHandler;
        GameManager.RemoveGUIPiecesAndTrackersEvent += RemoveGUIPiecesAndTrackersHandler;
        GameManager.ZoomHiveEvent += ZoomHiveHandler;
        GameManager.TwistPieceHandler += TwistPieceHandler;
        GameManager.FlickerPieceEvent += FlickerPieceHandler;

        GameManager.RefreshGUIPiecesEvent += RefreshGUIPiecesHandler;
        GameManager.UntwistPiecesEvent += UntwistPiecesHandler;
        DragDrop.UntwistPiecesEvent += UntwistPiecesHandler;

        // hovering animation
        GameManager.AnimateGameObjectToEvent += AnimateGameObjectToHandler;
        TutorialGameManager.AnimateGameObjectToEvent += AnimateGameObjectToHandler;
        TutorialGameManager.RestartHivePositionEvent += RestartHivePositionHandler;
        TutorialGameManager.ZoomHiveEvent += ZoomHiveHandler;

        // For testing purposes
        AIvsAI.AIvsAIGameOver += GameIsOverHandler;
        // AIvsAI.DropValidGUIPieceEvent += DropValidGUIPieceHandler;
    }

    void OnDisable()
    {
        // Unsubscribe from the events from GameManager, not doing so
        // may introduce weird bugs when changing scenes
        GameManager.SetupStartingGUIEvent -= SetupStartingGUIHandler;
        GameManager.DragGUIPieceEvent -= DragGUIPieceHandler;
        GameManager.DisplayGUIMovesEvent -= DisplayGUIMovesHandler;
        GameManager.MakeGUIPieceActiveEvent -= MakeGUIPieceActiveHandler;
        GameManager.DropValidGUIPieceEvent -= DropValidGUIPieceHandler;
        GameManager.DropInvalidGUIPieceEvent -= DropInvalidGUIPieceHandler;
        GameManager.GameIsOverEvent -= GameIsOverHandler;
        GameManager.DisplayGUITextEvent -= DisplayGUITextHandler;
        GameManager.MoveHiveToEvent -= MoveHiveToHandler;
        GameManager.RestartHivePositionEvent -= RestartHivePositionHandler;
        GameManager.DisplayGUITrackersEvent -= DisplayGUITrackersHandler;
        GameManager.RemoveGUIPiecesAndTrackersEvent -= RemoveGUIPiecesAndTrackersHandler;
        GameManager.ZoomHiveEvent -= ZoomHiveHandler;
        GameManager.TwistPieceHandler -= TwistPieceHandler;
        GameManager.FlickerPieceEvent -= FlickerPieceHandler;

        GameManager.RefreshGUIPiecesEvent -= RefreshGUIPiecesHandler;
        GameManager.UntwistPiecesEvent -= UntwistPiecesHandler;
        DragDrop.UntwistPiecesEvent -= UntwistPiecesHandler;

        // hovering animation
        GameManager.AnimateGameObjectToEvent -= AnimateGameObjectToHandler;
        TutorialGameManager.AnimateGameObjectToEvent -= AnimateGameObjectToHandler;
        TutorialGameManager.RestartHivePositionEvent -= RestartHivePositionHandler;
        TutorialGameManager.ZoomHiveEvent -= ZoomHiveHandler;

        // For testing purposes
        AIvsAI.AIvsAIGameOver -= GameIsOverHandler;
        // AIvsAI.DropValidGUIPieceEvent -= DropValidGUIPieceHandler;
    }

#region Handlers
    protected void SetupStartingGUIHandler(Board board, bool isOnline, bool animating)
    {
        /**
        TODO: setup buttons, animations, sound, other text?
        **/
        _boardRef = board;
        if (animating)
            StartCoroutine(SetupBoard(board, isOnline));
        else
            StaticSetupBoard(board, isOnline);
    }

    protected async Task AnimateGameObjectToHandler(GameObject gameObj, (int x, int y) to)
    {
        gameObj.GetComponent<RectTransform>().SetAsLastSibling();
        await BringTo(gameObj, to);
        if (gameObj.tag != "hoveredstack" && gameObj.name.Contains("Beetle") && _boardRef.Hive.ContainsKey(to) && _boardRef.Hive[to].Count > 1)
            FlickerPieceHandler(gameObj);
        gameObj.GetComponent<RectTransform>().localScale = gameObj.name.Contains("WBeetle") ? new Vector3(0.0375f, 0.0375f, 1f) : new Vector3(0.0675f, 0.0675f, 1f);
        // Debug.Log($"this guy {gameObject.name} now at {to}");
    }

    protected async Task BringTo(GameObject pieceGUI, (int x, int y) to)
    {
        RectTransform pieceRT = pieceGUI.GetComponent<RectTransform>();
        Vector2 destination = new Vector2(to.x, to.y);
        float t = 0f;
        while (Math.Floor(Vector2.Distance(pieceRT.anchoredPosition, destination)) > 1)
        {
            pieceRT.anchoredPosition = Vector2.Lerp(pieceRT.anchoredPosition, destination, Mathf.SmoothStep(0f, 1f, t));
            t += 0.01f;
            await Task.Delay(15);
        }
        pieceRT.anchoredPosition = destination;
    }

    protected void RefreshGUIPiecesHandler()
    {
        for (int p = 0; p < 11; ++p)
        {
            Piece coreWhitePiece = _boardRef.WhitePieces[p];
            Piece coreBlackPiece = _boardRef.BlackPieces[p];

            GameObject whitePieceGUI = GameObject.FindGameObjectWithTag(coreWhitePiece.Tag.ToString());
            GameObject blackPieceGUI = GameObject.FindGameObjectWithTag(coreBlackPiece.Tag.ToString());

            if (coreWhitePiece.IsOnHive)
            {
                whitePieceGUI.GetComponent<Image>().raycastTarget = coreWhitePiece.IsTop;
            }
            else
            {
                GameObject pieceAbove = GameObject.FindWithTag((coreWhitePiece.Tag + 1).ToString());
                whitePieceGUI.GetComponent<Image>().raycastTarget = pieceAbove == null || pieceAbove.gameObject.name != whitePieceGUI.gameObject.name || pieceAbove.gameObject.transform.parent.name == "Hive";
            }

            if (coreBlackPiece.IsOnHive)
            {
                blackPieceGUI.GetComponent<Image>().raycastTarget = coreBlackPiece.IsTop;
            }
            else
            {
                GameObject pieceAbove = GameObject.FindWithTag((coreBlackPiece.Tag + 1).ToString());
                blackPieceGUI.GetComponent<Image>().raycastTarget = pieceAbove == null || pieceAbove.gameObject.name != blackPieceGUI.gameObject.name || pieceAbove.gameObject.transform.parent.name == "Hive";
            }

            whitePieceGUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(coreWhitePiece.Point.x, coreWhitePiece.Point.y);
            blackPieceGUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(coreBlackPiece.Point.x, coreBlackPiece.Point.y);

            whitePieceGUI.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
            blackPieceGUI.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    protected void ZoomHiveHandler(bool zoomIn, bool restart)
    {
        if (restart)
        {
            _hiveGameObj.transform.localScale = new Vector3(1f, 1f, 1f);
            // _handGameObj.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            Vector3 delta;
            if (zoomIn) delta = new Vector3(0.1f, 0.1f, 0.1f);
            else delta = new Vector3(-0.1f, -0.1f, -0.1f);

            Vector3 maxScale = new Vector3(1.2f, 1.2f, 1.2f);
            Vector3 minScale = new Vector3(0.6f, 0.6f, 0.6f);
            Vector3 newScale = _hiveGameObj.transform.localScale + delta;
            _hiveGameObj.transform.localScale = Vector3.Min(Vector3.Max(newScale, minScale), maxScale);
            // _handGameObj.transform.localScale = Vector3.Min(Vector3.Max(newScale, minScale), maxScale);
        }
    }

    protected void DragGUIPieceHandler(string pieceTag, float toX, float toY)
    {
        GameObject movingPieceUI = GameObject.FindWithTag(pieceTag);
        // UnityEngine.Debug.Log($"Dragging by {toX} {toY}");
        movingPieceUI.GetComponent<RectTransform>().anchoredPosition += new Vector2(toX, toY);
    }

    protected void MakeGUIPieceActiveHandler(string pieceTag)
    {
        // Add stuff that makes it feel (sound) and look (graphics) active
        GameObject activeGUIPieceObj = GameObject.FindWithTag(pieceTag);
        CanvasGroup canvasGroup = activeGUIPieceObj.GetComponent<CanvasGroup>();
        RectTransform rectTransform = activeGUIPieceObj.GetComponent<RectTransform>();

        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        rectTransform.SetParent(rectTransform.root);
        rectTransform.SetAsLastSibling();
    }

    protected void DisplayGUIMovesHandler(HashSet<(int, int)> points)
    {
        foreach ((int x, int y) point in points)
        {
            GameObject placeholder = Instantiate(Resources.Load("possiblemove") as GameObject, new Vector2(point.x, point.y), Quaternion.identity) as GameObject;
            // needs to get instantiated inside Board (Game Object)
            placeholder.transform.SetParent(_hiveGameObj.transform, false);
        }
    }

    protected void DisplayGUITrackersHandler((int x, int y) prevMove, (int x, int y) curMove)
    {
        // Destroy previously displayed tracker placeholders
        foreach (var placeholder in GameObject.FindGameObjectsWithTag("tracker"))
        {
            Destroy(placeholder);
        }

        GameObject prevTracker = Instantiate(Resources.Load("prevmove") as GameObject, new Vector2(prevMove.x, prevMove.y), Quaternion.identity) as GameObject;
        // needs to get instantiated inside Board (Game Object)
        prevTracker.transform.SetParent(IsOnHand(prevMove) ? _handGameObj.transform : _hiveGameObj.transform, false);

        GameObject curTracker = Instantiate(Resources.Load("curmove") as GameObject, new Vector2(curMove.x, curMove.y), Quaternion.identity) as GameObject;
        // needs to get instantiated inside Board (Game Object)
        curTracker.transform.SetParent(IsOnHand(curMove) ? _handGameObj.transform : _hiveGameObj.transform, false);
    }

    protected void DropValidGUIPieceHandler(string pieceTag, int toX, int toY, int piecesAtToPoint)
    {
        // Add stuff that makes it feel (sound) and look (graphics) like a valid move

        // Set transparency and stuff back to normal
        GameObject activeGUIPieceObj = GameObject.FindWithTag(pieceTag);
        RectTransform rectTransform = activeGUIPieceObj.GetComponent<RectTransform>();

        // int offSetForStack = piecesAtToPoint * 2;
        // rectTransform.anchoredPosition = piecesAtToPoint > 1 ? new Vector2(toX + offSetForStack, toY + offSetForStack) : new Vector2(toX, toY);
        // In case the parent gameobject (Hand or Hive) have been modified to create a zooming effect
        rectTransform.localScale = activeGUIPieceObj.gameObject.name.Contains("WBeetle") ? new Vector3(0.0375f, 0.0375f, 1f) : new Vector3(0.0675f, 0.0675f, 1f);
        CanvasGroup canvasGroup = activeGUIPieceObj.GetComponent<CanvasGroup>();
        rectTransform.SetParent(piecesAtToPoint != -1 ? _hiveGameObj.transform : _handGameObj.transform, false);
        // rectTransform.SetParent(piecesAtToPoint != -1 ? _canvasGameObj.transform : _handGameObj.transform, false);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        activeGUIPieceObj.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        rectTransform.anchoredPosition = new Vector2(toX, toY);

        // Destroy Placeholders
        foreach (var placeholder in GameObject.FindGameObjectsWithTag("placeholder"))
        {
            Destroy(placeholder);
        }

        if (ShowDroppingAnimation)
        {
            GameObject pieceDropAnim = Instantiate(Resources.Load("Piece Drop Anim") as GameObject, new Vector2(toX, toY), Quaternion.identity);
            pieceDropAnim.transform.SetParent(_hiveGameObj.transform, false);
            StartCoroutine(DelayedDestroy(pieceDropAnim));
        }

        if (activeGUIPieceObj.name.Contains("Beetle") && piecesAtToPoint > 1)
            StartCoroutine(FlickerPiece(activeGUIPieceObj));
    }

    public void FlickerPieceHandler(GameObject piece)
    {
        StartCoroutine(FlickerPiece(piece));
    }

    private IEnumerator DelayedDestroy(GameObject obj)
    {
        yield return new WaitForSeconds(DroppingAnimationDuration);
        Destroy(obj);
    }

    private void DestroyHighlightAt(Vector2 pos)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("beetleonstack"))
        {
            if (obj.GetComponent<RectTransform>().anchoredPosition == pos)
                Destroy(obj);
        }
    }

    private IEnumerator FlickerPiece(GameObject piece)
    {
        Vector2 startingPos = piece.GetComponent<RectTransform>().anchoredPosition;
        if (!(GameObject.FindGameObjectWithTag("beetleonstack") && GameObject.FindGameObjectWithTag("beetleonstack").GetComponent<RectTransform>().anchoredPosition == startingPos))
        {
            GameObject highlight = Instantiate(Resources.Load("BeetleOnStack") as GameObject, startingPos, Quaternion.identity);
            highlight.transform.SetParent(_hiveGameObj.transform, false);
            highlight.GetComponent<RectTransform>().SetAsLastSibling();
        }
        bool ascending = true;
        while (startingPos.Equals(GameObject.FindGameObjectWithTag(piece.tag).GetComponent<RectTransform>().anchoredPosition))
        {
            if (ascending)
            {
                for (int i = 100; i <= 255; ++i)
                {
                    piece.GetComponent<Image>().color = new Color32((byte)i, (byte)i, (byte)i, 255);
                    yield return new WaitForSeconds(0.01f);
                }
                ascending = false;
            }
            else
            {
                for (int i = 255; i >= 100; --i)
                {
                    piece.GetComponent<Image>().color = new Color32((byte)i, (byte)i, (byte)i, 255);
                    yield return new WaitForSeconds(0.01f);
                }
                ascending = true;
            }
        }
        piece.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        DestroyHighlightAt(startingPos);
    }

    private async void TwistPieceHandler(string pieceTag, bool on)
    {
        await TwistPiece(GameObject.FindGameObjectWithTag(pieceTag), on);
    }

    private async Task TwistPiece(GameObject piece, bool on)
    {
        piece.GetComponent<RectTransform>().SetAsLastSibling();
        const int DELTA = 15;
        if (on)
        {
            for(int i = 0; i <= DELTA; ++i)
            {
                piece.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, i);
                await Task.Delay(1);
            }
            piece.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, DELTA);
        }
        else
        {
            for(int i = DELTA; i >= 0; --i)
            {
                piece.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, i);
                await Task.Delay(1);
            }
            piece.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void UntwistPiecesHandler()
    {
        for (int p = 0; p < 11; ++p)
        {
            Piece coreWhitePiece = _boardRef.WhitePieces[p];
            Piece coreBlackPiece = _boardRef.BlackPieces[p];

            GameObject whitePieceGUI = GameObject.FindGameObjectWithTag(coreWhitePiece.Tag.ToString());
            GameObject blackPieceGUI = GameObject.FindGameObjectWithTag(coreBlackPiece.Tag.ToString());

            whitePieceGUI.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
            blackPieceGUI.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    protected void DropInvalidGUIPieceHandler(string pieceTag, int toX, int toY, int piecesAtToPoint)
    {
        // Add stuff that makes it feel (sound) and look (graphics) like a valid move

        // Set transparency and stuff back to normal
        GameObject activeGUIPieceObj = GameObject.FindWithTag(pieceTag);
        RectTransform rectTransform = activeGUIPieceObj.GetComponent<RectTransform>();
        rectTransform.localScale = activeGUIPieceObj.gameObject.name.Contains("WBeetle") ? new Vector3(0.0375f, 0.0375f, 1f) : new Vector3(0.0675f, 0.0675f, 1f);
        CanvasGroup canvasGroup = activeGUIPieceObj.GetComponent<CanvasGroup>();
        rectTransform.SetParent(piecesAtToPoint != -1 ? _hiveGameObj.transform : _handGameObj.transform, false);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        activeGUIPieceObj.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        rectTransform.anchoredPosition = new Vector2(toX, toY);
        // AnimateGameObjectToHandler(activeGUIPieceObj, (toX, toY));

        // Destroy Placeholders
        foreach (var placeholder in GameObject.FindGameObjectsWithTag("placeholder"))
            Destroy(placeholder);

        if (activeGUIPieceObj.name.Contains("Beetle") && piecesAtToPoint > 1)
            StartCoroutine(FlickerPiece(activeGUIPieceObj));
    }

    protected void GameIsOverHandler(bool whiteWon, bool isDraw)
    {
        if (showGameOverScreen)
        {
            if (isDraw)
            {
                _drawScreen.SetActive(true);
            }
            else if (whiteWon)
            {
                _whiteWonScreen.SetActive(true);
            }
            else
            {
                _blackWonScreen.SetActive(true);
            }
        }
        else
        {
            if (isDraw)
            {
                DisplayGUITextHandler("Draw!");
            }
            else if (whiteWon)
            {
                DisplayGUITextHandler("White won!");
            }
            else
            {
                DisplayGUITextHandler("Black won!");
            }
        }
    }

    protected void DisplayGUITextHandler(string message)
    {
        Msg.GetComponent<CanvasGroup>().alpha = 1;
        Msg.SetActive(true);
        // Change the text by grabbing TextMeshProUGUI component 
        Msg.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
    }

    protected void MoveHiveToHandler(int x, int y)
    {
        RectTransform boardRT = _hiveGameObj.GetComponent<RectTransform>(); 
        boardRT.anchoredPosition += new Vector2(x, y);
    }
    protected void RestartHivePositionHandler()
    {
        RectTransform boardRT = _hiveGameObj.GetComponent<RectTransform>(); 
        boardRT.anchoredPosition = new Vector2(0, 0);
    }
#endregion

#region Helper Methods
    protected void RemoveGUIPiecesAndTrackersHandler(Board board)
    {
        // Destroy Pieces
        for (int p = 0; p < 11; ++p)
        {
            Destroy(GameObject.FindWithTag($"{board.WhitePieces[p].Tag}"));
            Destroy(GameObject.FindWithTag($"{board.BlackPieces[p].Tag}"));
        }

        // Destroy Trackers
        foreach (var placeholder in GameObject.FindGameObjectsWithTag("tracker"))
        {
            Destroy(placeholder);
        }
    }

    protected IEnumerator SetupBoard(Board board, bool isOnline)
    {
        int[] topPiece = {2, 3, 5, 8, 10};
        // Generate Pieces
        for (int p = 0; p < 11; ++p)
        {
            Piece whitePiece = board.WhitePieces[p];
            GameObject whitePieceGameObj = Instantiate(Resources.Load($"W{whitePiece.Insect}") as GameObject, new Vector2(whitePiece.Point.x, whitePiece.Point.y), Quaternion.identity);
            whitePieceGameObj.tag =  $"{whitePiece.Tag}";
            // The false is at the end is to keep scale, rotations, and translation values the same as the original
            // source: https://answers.unity.com/questions/1219299/everytime-i-instantiate-object-becomes-smaller.html
            // source: https://docs.unity3d.com/ScriptReference/Transform.SetParent.html
            whitePieceGameObj.transform.SetParent(_handGameObj.transform, false);
            if (isOnline) whitePieceGameObj.GetComponent<PhotonView>().ViewID = whitePiece.Tag;

            Piece blackPiece = board.BlackPieces[p];
            GameObject blackPieceGameObj = Instantiate(Resources.Load($"B{blackPiece.Insect}") as GameObject, new Vector2(blackPiece.Point.x, blackPiece.Point.y), Quaternion.identity);
            blackPieceGameObj.tag =  $"{blackPiece.Tag}";
            blackPieceGameObj.transform.SetParent(_handGameObj.transform, false);
            if (isOnline) blackPieceGameObj.GetComponent<PhotonView>().ViewID = blackPiece.Tag;

            // if it is a top piece
            if (Array.Exists(topPiece, index => index == p))
            {
                whitePieceGameObj.GetComponent<Image>().raycastTarget = true;
                blackPieceGameObj.GetComponent<Image>().raycastTarget = true;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    protected void StaticSetupBoard(Board board, bool isOnline)
    {
        int[] topPiece = {2, 3, 5, 8, 10};
        // Generate Pieces
        for (int p = 0; p < 11; ++p)
        {
            Piece whitePiece = board.WhitePieces[p];
            GameObject whitePieceGameObj = Instantiate(Resources.Load($"W{whitePiece.Insect}") as GameObject, new Vector2(whitePiece.Point.x, whitePiece.Point.y), Quaternion.identity) as GameObject;
            whitePieceGameObj.tag =  $"{whitePiece.Tag}";
            // The false is at the end is to keep scale, rotations, and translation values the same as the original
            // source: https://answers.unity.com/questions/1219299/everytime-i-instantiate-object-becomes-smaller.html
            // source: https://docs.unity3d.com/ScriptReference/Transform.SetParent.html
            whitePieceGameObj.transform.SetParent(_handGameObj.transform, false);
            if (isOnline) whitePieceGameObj.GetComponent<PhotonView>().ViewID = whitePiece.Tag;

            Piece blackPiece = board.BlackPieces[p];
            GameObject blackPieceGameObj = Instantiate(Resources.Load($"B{blackPiece.Insect}") as GameObject, new Vector2(blackPiece.Point.x, blackPiece.Point.y), Quaternion.identity) as GameObject;
            blackPieceGameObj.tag =  $"{blackPiece.Tag}";
            blackPieceGameObj.transform.SetParent(_handGameObj.transform, false);
            if (isOnline) blackPieceGameObj.GetComponent<PhotonView>().ViewID = blackPiece.Tag;

            // // if it is a top piece
            // if (Array.Exists(topPiece, index => index == p))
            // {
            //     whitePieceGameObj.GetComponent<Image>().raycastTarget = true;
            //     blackPieceGameObj.GetComponent<Image>().raycastTarget = true;
            // }
        }
    }
#endregion
}