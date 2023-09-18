using HiveCore;
using static HiveCore.Utils;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Move = System.ValueTuple<HiveCore.Piece, System.ValueTuple<int, int>>;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    // This event needs the parameters: pieceTag (string), toX (int), toY (int)
    public static event Action<string, float, float> DragGUIPieceEvent;
    public static event Action<string, int, int, int> DropValidGUIPieceEvent;
    public static event Action<string, int, int, int> DropInvalidGUIPieceEvent;
    public static event Action<(int, int), (int, int)> DisplayGUITrackersEvent;
    public static event Action<HashSet<(int, int)>> DisplayGUIMovesEvent;
    public static event Action<string> MakeGUIPieceActiveEvent;
    public static event Action<Board, bool, bool> SetupStartingGUIEvent;
    public static event Action <Board> RemoveGUIPiecesAndTrackersEvent;
    public static event Action<bool, bool> GameIsOverEvent;
    public static event Action<string> DisplayGUITextEvent;
    public static event Action<int, int> MoveHiveToEvent;
    public static event Action<string, bool> TwistPieceHandler;
    public static event Action<GameObject> FlickerPieceEvent;
    public static event Action RefreshGUIPiecesEvent;
    public static event Action<string, (int, int)> AnimatePieceToEvent;
    public static event Func<GameObject, (int, int), Task> AnimateGameObjectToEvent;
    public static event Action RestartHivePositionEvent;
    public static event Action UntwistPiecesEvent;
    public static event Action <bool, bool> ZoomHiveEvent;
    public Board Board { get; private set; }
    public bool isOnline = false;
    public static Player player1 = Player.White;
    public static Player player2 = Player.Black;
    private Move _prevBlackMove;
    private Move _prevWhiteMove;
    public static Player AIPlayer1 = Player.Black;
    public static Player AIPlayer2 = Player.White;
    public bool PlayingWithAI = false;
    public int ALPHA_BETA_DEPTH = 3;
    public static Player whoseTurn = Player.White;
    protected HashSet<(int x, int y)> _moves;
    // protected HashSet<(Piece piece, (int x, int y) to)> _moves;

    public bool MoveTrackers = true;
    public static bool isGameOver = false;
    public static bool IsAISearching = false;
    private GameObject _whitePanelNoSignal;
    private GameObject _blackPanelNoSignal;
    public bool startingGameAnimation = true;
    public bool runTurnAnimation = true;
    public bool ShowDroppingAnimationForAI = true;
    public float DroppingAnimationDuration = 0.7f;
    public static bool switchedSides = false;
    private bool _canUndoLastMove = true;
    public bool isAIvsAIGame = false;
    public int moveCount = 0;
    public bool isTutorial = false;
    public static bool allowPieceMove = true;
    public bool isEasyMode = false;

    // This script needs to be loaded after GUIManager
    void OnEnable()
    {
        player1 = Player.White;
        player2 = Player.Black;
        AIPlayer1 = Player.Black;
        whoseTurn = Player.White;
        IsAISearching = false;
        isGameOver = false;
        isTutorial = false;

        Debug.Log($"this is my switchedSides {switchedSides}");
        Debug.Log($"BEFORE AIPlayer1 {AIPlayer1} player1 {player1} player2 {player2}");


        Board = new Board();
        if (switchedSides)
        {
            AIPlayer1 = AIPlayer1 == Player.Black ? Player.White : Player.Black;
            player1 = player1 == Player.Black ? Player.White : Player.Black;
            player2 = player1 == Player.Black ? Player.White : Player.Black;
            switchedSides = false;
            GameObject.FindGameObjectWithTag("colorpickerscreen").SetActive(false);
            Debug.Log($"AFTER AIPlayer1 {AIPlayer1} player1 {player1} player2 {player2}");
        }

        Debug.Log($"isGameOver {isGameOver}");
        Debug.Log($"PlayingWithAI {PlayingWithAI}");
        Debug.Log($"whoseTurn {whoseTurn}");
        Debug.Log($"AIPlayer1 {AIPlayer1}");
        Debug.Log($"IsAISearching {IsAISearching}");
        Debug.Log($"will it work {!isGameOver && PlayingWithAI && whoseTurn == AIPlayer1 && !IsAISearching}");
        SetupStartingGUIEvent?.Invoke(Board, isOnline, startingGameAnimation);

        DragDrop.isOnline = isOnline;
        DragDrop.animateStartingPieces = startingGameAnimation;
        // Subscribe Handlers
        DragDrop.IsMyTurnEvent += IsMyTurnHandler;
        DragDrop.DragBeganEvent += DragBeganHandler;
        DragDrop.DraggingEvent += DraggingHandler;
        DragDrop.DragEndedEvent += DragEndedHandler;
        DragDrop.PointerEnterEvent += PointerEnterHandler;
        DragDrop.PointerExitEvent += PointerExitHandler;
        AIColorPicker.PlayerPickedEvent += PlayerPickedHandler;
        PauseMenuAI.ResetGameEvent += ResetGameHandler;
        // PauseMenu.ResetGameEvent += ResetGameHandler;
        // GameOver.ResetGameEvent += ResetGameHandler;
        // GameOverDraw.ResetGameEvent += ResetGameHandler;
        // SwitchSidesAILocalBoard.SwitchSides += SwitchSidesHandler;

        _whitePanelNoSignal = GameObject.FindGameObjectWithTag("whitenosignal");
        _blackPanelNoSignal = GameObject.FindGameObjectWithTag("blacknosignal");

        ShowWhoseTurnPanel();
    }

    void OnDisable()
    {
        // Subscribe Handlers
        DragDrop.IsMyTurnEvent -= IsMyTurnHandler;
        DragDrop.DragBeganEvent -= DragBeganHandler;
        DragDrop.DraggingEvent -= DraggingHandler;
        DragDrop.DragEndedEvent -= DragEndedHandler;
        DragDrop.PointerEnterEvent -= PointerEnterHandler;
        DragDrop.PointerExitEvent -= PointerExitHandler;
        AIColorPicker.PlayerPickedEvent -= PlayerPickedHandler;
        PauseMenuAI.ResetGameEvent -= ResetGameHandler;

        // PauseMenu.ResetGameEvent -= ResetGameHandler;
        // GameOver.ResetGameEvent -= ResetGameHandler;
        // GameOverDraw.ResetGameEvent -= ResetGameHandler;
        // SwitchSidesAILocalBoard.SwitchSides -= SwitchSidesHandler;
    }

    protected void ResetGameHandler()
    {
        // ResetGame();
        player1 = Player.White;
        AIPlayer1 = Player.Black;
        switchedSides = false;
        isGameOver = false;
        IsAISearching = false;
        whoseTurn = Player.White;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    protected void PlayerPickedHandler(int color)
    {
        if (color == 1)
        {
            player1 = Player.White;
            AIPlayer1 = Player.Black;
        }
        else
        {
            player1 = Player.Black;
            AIPlayer1 = Player.White;
        }
    }

    protected void SwitchSidesHandler()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        switchedSides = true;
    }

    protected void PointerEnterHandler(string pieceTag)
    {
        if (!IsAISearching)
        {
            Piece piece = Board.GetPieceReferenceByTagString(pieceTag);
            (int x, int y) posOnHive = piece.Point;
            int stackCountAtPosOnHive = Board.Hive.ContainsKey(posOnHive) ? Board.Hive[posOnHive].Count : -2;
            // Hovering over stack
            if (piece.Insect == Insect.Beetle && piece.IsOnHive && stackCountAtPosOnHive > 1)
            {
                DisplayPossibleMoves(pieceTag);
                RemoveHoveredStack();
                int yDelta = 70;
                int xDelta = -10;
                int skew = 10;
                foreach (Piece pieceInStack in Board.Hive[posOnHive].Reverse())
                {
                    GameObject pieceGUI = Instantiate(Resources.Load(pieceInStack.Player == Player.Black ? $"B{pieceInStack.Insect}" : $"W{pieceInStack.Insect}") as GameObject, new Vector2(pieceInStack.Point.x, pieceInStack.Point.y), Quaternion.identity);
                    AnimateGameObjectToEvent(pieceGUI, (pieceInStack.Point.x + xDelta, pieceInStack.Point.y + yDelta));
                    pieceGUI.GetComponent<RectTransform>().SetParent(GameObject.Find("Canvas/Hive").transform, false);
                    pieceGUI.GetComponent<RectTransform>().rotation = Quaternion.Euler(skew / 4, -skew / 4, skew / 2);
                    Destroy(pieceGUI.GetComponent<DragDrop>());
                    pieceGUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    pieceGUI.tag = "hoveredstack";
                    xDelta += 20;
                    yDelta += 10;
                    skew += 15;
                }
            }
            // Hovering over single piece
            else
            {
                if (PlayingWithAI)
                {
                    if (whoseTurn != AIPlayer1 && piece.Player != AIPlayer1)
                    {
                        DisplayPossibleMoves(pieceTag);
                        if (allowPieceMove)
                        {
                            TwistPieceHandler(pieceTag, true);
                        }
                    }
                }
                else
                {
                    if (DragDrop.isMyTurn)
                    {
                        DisplayPossibleMoves(pieceTag);
                        if (allowPieceMove)
                        {
                            TwistPieceHandler(pieceTag, true);
                        }
                    }
                }
            }
        }
    }

    protected void PointerExitHandler(string pieceTag)
    {
        if (!IsAISearching)
        {
            Piece piece = Board.GetPieceReferenceByTagString(pieceTag);
            (int x, int y) posOnHive = piece.Point;

            if (piece.IsOnHive && Board.Hive.ContainsKey(posOnHive) && Board.Hive[posOnHive].Count > 1)
            {
                RemoveHoveredStack();
            }
            else
            {
                if (PlayingWithAI)
                {
                    if (whoseTurn != AIPlayer1 && piece.Player != AIPlayer1)
                    if (allowPieceMove)
                    {
                        TwistPieceHandler(pieceTag, false);
                    }
                }
                else
                {
                    if (DragDrop.isMyTurn)
                    if (allowPieceMove)
                    {
                        TwistPieceHandler(pieceTag, false);
                    }
                }
            }
        }
        foreach (GameObject pm in GameObject.FindGameObjectsWithTag("placeholder"))
        {
            Destroy(pm);
        }
    }

    protected bool IsMyTurnHandler(string pieceName, int onlineId)
    {
        if (isOnline)
        {
            if (onlineId == 1) // white
            {
                return whoseTurn == Player.White && pieceName[0] == 'W';
            }

            if (onlineId == 2) // black
            {
                return whoseTurn == Player.Black && pieceName[0] == 'B';
            }
        }

        return PlayingWithAI
                ? whoseTurn != AIPlayer1 && AIPlayer1.ToString()[0] != pieceName[0]
                : (whoseTurn == Player.White && pieceName[0] == 'W') || (whoseTurn == Player.Black && pieceName[0] == 'B');
    }

    protected void DisplayPossibleMoves(string pieceTag)
    {
        // Hide warning msg if it's there
        // if (!isTutorial)
        // {
        //     GameObject warning = GameObject.FindGameObjectWithTag("warningmsg");
        //     if (warning && warning.active && warning.GetComponent<CanvasGroup>().alpha == 1f)
        //     {
        //         warning.GetComponent<FadeOut>().Run();
        //     }
        // }
        foreach (GameObject pm in GameObject.FindGameObjectsWithTag("placeholder"))
        {
            Destroy(pm);
        }

        Piece piece = Board.GetPieceReferenceByTagString(pieceTag);
        try
        {
            _moves = Board.GenerateMovesForPieceThrowsException(piece);
            // Tell GUIManager to display placeholders
            DisplayGUIMovesEvent(_moves);
            // Tell GUIManager to make this piece look and feel "active"
            allowPieceMove = true;
        }
        catch (ArgumentException ex)
        {
            allowPieceMove = false;
            // if (isOnline)
            // {
            //     if (DragDrop.isMyTurn)
            //     {
            //         DisplayGUITextEvent(ex.Message);
            //     }
            // }
            // else
            // {
            //     DisplayGUITextEvent(ex.Message);
            // }
        }
    }

    protected void ShowWarningMessageIfAny(string pieceTag)
    {
        Piece piece = Board.GetPieceReferenceByTagString(pieceTag);
        try
        {
            _moves = Board.GenerateMovesForPieceThrowsException(piece);
            // Tell GUIManager to display placeholders
            // DisplayGUIMovesEvent(_moves);
            // Tell GUIManager to make this piece look and feel "active"
            allowPieceMove = true;
        }
        catch (ArgumentException ex)
        {
            allowPieceMove = false;
            if (isOnline)
            {
                if (DragDrop.isMyTurn)
                {
                    DisplayGUITextEvent(ex.Message);
                }
            }
            else
            {
                DisplayGUITextEvent(ex.Message);
            }
        }
    }

    protected void DragBeganHandler(string pieceTag)
    {
        // UntwistPiecesEvent();
        // Piece piece = Board.GetPieceReferenceByTagString(pieceTag);
        // try
        // {
        //     _moves = Board.GenerateMovesForPieceThrowsException(piece);
        //     // Tell GUIManager to display placeholders
        //     DisplayGUIMovesEvent(_moves);

        //     // Tell GUIManager to make this piece look and feel "active"
        //     MakeGUIPieceActiveEvent(pieceTag);
        //     allowPieceMove = true;
        // }
        // catch (ArgumentException ex)
        // {
        //     allowPieceMove = false;
        //     if (isOnline)
        //     {
        //         if (DragDrop.isMyTurn)
        //         {
        //             DisplayGUITextEvent(ex.Message);
        //         }
        //     }
        //     else
        //     {
        //         DisplayGUITextEvent(ex.Message);
        //     }
        // }
        // RemoveHoveredStack();

        ShowWarningMessageIfAny(pieceTag);
        if (allowPieceMove)
        {
            UntwistPiecesEvent();
            MakeGUIPieceActiveEvent(pieceTag);
            RemoveHoveredStack();
            DisplayPossibleMoves(pieceTag);
        }



        // _moves = new HashSet<(Piece, (int, int))>(Board.GenerateMovesForPlayer(whoseTurn).Where(move => move.piece.Tag == Int32.Parse(pieceTag)));
        // // Tell GUIManager to display placeholders
        // DisplayGUIMovesEvent(new HashSet<(int, int)>(_moves.Select(m => m.to)));
        // // Tell GUIManager to make this piece look and feel "active"
        // MakeGUIPieceActiveEvent(pieceTag);
        // RemoveHoveredStack();
    }

    protected void DraggingHandler(string pieceTag, float deltaX, float deltaY, float canvasScaleFactor)
    {
        if (allowPieceMove)
        {
            // Tell GUI
            DragGUIPieceEvent(pieceTag, deltaX/canvasScaleFactor, deltaY/canvasScaleFactor);
        }
    }

    protected async void DragEndedHandler(string pieceTag, float toX, float toY)
    {
        if (allowPieceMove)
        {
            Piece piece = Board.GetPieceReferenceByTagString(pieceTag);
            (int x, int y) to = ((int)toX, (int)toY);
            // Find the first move that matches the moves calculated when DragBegan
            if (_moves.Any(move => move == to))
            {
                // Cache previous move
                if (whoseTurn == Player.Black) _prevBlackMove = (piece, piece.Point);
                else _prevWhiteMove = (piece, piece.Point);
                _MakeValidMove((piece, to));
                if (!isTutorial)
                {
                    _DetectGameOver();
                        _SwitchTurn();
                }
            }
            else
            {
                DropInvalidGUIPieceEvent(pieceTag, piece.Point.x, piece.Point.y, piece.IsOnHive ? Board.Hive[piece.Point].Count : -1);
            }
        }



        // Piece piece = Board.GetPieceReferenceByTagString(pieceTag);
        // (int x, int y) to = ((int)toX, (int)toY);
        // // Find the first move that matches the moves calculated when DragBegan
        // if (_moves.Any(move => move.piece.Equals(piece) && move.to == to))
        // {
        //     // Cache previous move
        //     if (whoseTurn == Player.Black) _prevBlackMove = (piece, piece.Point);
        //     else _prevWhiteMove = (piece, piece.Point);
        //     _MakeValidMove((piece, to));
        //     if (!isTutorial)
        //     {
        //         _DetectGameOver();
        //             _SwitchTurn();
        //     }
        // }
        // else
        // {
        //     DropInvalidGUIPieceEvent(pieceTag, piece.Point.x, piece.Point.y, piece.IsOnHive ? Board.Hive[piece.Point].Count : -1);
        // }
    }

    protected void RemoveHoveredStack()
    {
        foreach (GameObject pieceGUI in GameObject.FindGameObjectsWithTag("hoveredstack"))
            StartCoroutine(AnimatedDestroy(pieceGUI));
    }

    protected IEnumerator AnimatedDestroy(GameObject pieceGUI)
    {
        for (float i = 1f; i >= 0; i -= 0.05f)
        {
            if (pieceGUI != null)
                pieceGUI.GetComponent<CanvasGroup>().alpha = i;
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(pieceGUI);
    }

    protected void _DetectGameOver()
    {
        bool blackQueenSurrounded = Board.BlackPieces[Q1].IsSurrounded;
        bool whiteQueenSurrounded = Board.WhitePieces[Q1].IsSurrounded;
        if (Board.IsDraw())
        {
            GameIsOverEvent(false, true);
            isGameOver = true;
        }
        else if (blackQueenSurrounded)
        {
            GameIsOverEvent(true, false);
            isGameOver = true;
        }
        else if (whiteQueenSurrounded)
        {
            GameIsOverEvent(false, false);
            isGameOver = true;
        }
    }

    protected void _SwitchTurn()
    {
        // if opponent has moves
        Player opp = whoseTurn == Player.Black ? Player.White : Player.Black;
        if (Board.GenerateMovesForPlayer(opp).Count > 0)
        {
            // UnityEngine.Debug.Log($"switching turn to {opp}");
            // switch whose turn
            whoseTurn = opp;
            // DisplayGUITextEvent($"Whose turn: {whoseTurn}");
        }
        _canUndoLastMove = true;
        ShowWhoseTurnPanel();
        if (!isAIvsAIGame)
            RefreshGUIPiecesEvent();
    }

    protected void ShowWhoseTurnPanel()
    {
        if (runTurnAnimation)
        {
            if (whoseTurn == Player.Black)
            {
                _whitePanelNoSignal.SetActive(true);
                _blackPanelNoSignal.SetActive(false);
            }
            else
            {
                _whitePanelNoSignal.SetActive(false);
                _blackPanelNoSignal.SetActive(true);
            }
        }
    }


    void Update()
    {
        if (!isGameOver && PlayingWithAI && whoseTurn == AIPlayer1 && !IsAISearching)
        {
            int manyPiecesOnHand = GameObject.Find("Canvas/Hive").transform.childCount;
            int manyPiecesOnHive = GameObject.Find("Canvas/Hand").transform.childCount;
            if (manyPiecesOnHand + manyPiecesOnHive >= 22)
                AIMoveAsync();
        }

        if (!isGameOver && Board.Hive.Count > 0 && (!PlayingWithAI || !IsAISearching))
        {
            if (!isOnline || EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject != GameObject.FindGameObjectWithTag("textboxchat"))
            {
                if (DragDrop.isMyTurn)
                {
                    // Restart zoom and position
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        RestartHivePositionEvent();
                        ZoomHiveEvent(false, true);
                    }
                }
            }

            if ((!isOnline
                 || ((DragDrop.MyOnlineColor == 1 && whoseTurn == Player.White) || (DragDrop.MyOnlineColor == 2 && whoseTurn == Player.Black))))
            {
                if (GameObject.Find("Canvas/Pause Menu Canvas") == null || !GameObject.Find("Canvas/Pause Menu Canvas").active)
                {
                    // zoom in
                    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                    {
                        // EnableHiveZoom is attached to the ScrollView GameObject on the Online scene
                        // This is so that it won't zoom if you're scrolling through the chat
                        if (!isOnline || EnableHiveZoom.enabled)
                            ZoomHiveEvent(true, false);
                    }
                    // zoom out
                    if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                    {
                        if (!isOnline || EnableHiveZoom.enabled)
                            ZoomHiveEvent(false, false);
                    }    
                }
            }
            
            // Undo on local games
            if (!isOnline && _canUndoLastMove && Input.GetKeyDown(KeyCode.U))
            {
                if (_prevBlackMove.Item1 != null)
                {
                    _MakeValidMove(_prevBlackMove);
                    Board.History[Player.Black].RemoveLast();
                }

                if (_prevWhiteMove.Item1 != null)
                {
                    _MakeValidMove(_prevWhiteMove);
                    Board.History[Player.White].RemoveLast();
                }
                _canUndoLastMove = false;
            }
        }
    }

    private async void AIMoveAsync()
    {
        IsAISearching = true;
        const int MIN = 100000000;
        const int MAX = -100000000;
        Dictionary<string, int> heuristicWeights = new Dictionary<string, int>() {
                {"queenSurroundedWeight", 9},
                {"antOnBoardWeight", 5},
                {"piecesOnBoardWeight", 5},
                {"queenPinWeight", 10},
                {"beetledQueenWeight", 5},
                {"queenDefenderWeight", 10},
                {"beetleQueenDistanceWeight", 35},
                {"playQueenWeight", 1000},
                {"beetleOnTopOfTheCircuitIsAGoodThingWeight", 7}
        };

        // UnityEngine.Debug.Log("AI searching");
        (float eval, (Piece piece, (int x, int y) to)) = await Task.Run(() => Board.AlphaBeta(whoseTurn, AIPlayer1, MAX, MIN, ALPHA_BETA_DEPTH, heuristicWeights, moveCount, isEasyMode));
        // (Piece piece, (int x, int y) to) = await Task.Run(() => Board.MCTS(AIPlayer1));
       
        // Debug AI
        //UnityEngine.Debug.Log($"{eval}");

        // UnityEngine.Debug.Log("AI done searching");

        // Cache previous move so that we can also undo AI's move
        if (whoseTurn == Player.Black) _prevBlackMove = (piece, piece.Point);
        else _prevWhiteMove = (piece, piece.Point);

        ///////////////////////////////////////////////////////// AI Move
        // Make move in core
        Board.MovePiece(piece, to, true);
        (int x, int y) oldPoint = whoseTurn == Player.Black ? _prevBlackMove.Item2 : _prevWhiteMove.Item2;

        // Display trackers
        if (MoveTrackers) DisplayGUITrackersEvent(oldPoint, to);

        // Animate move
        if (Board.Hive.ContainsKey(oldPoint))
        {
            Piece op = Board.Hive[oldPoint].Peek();
            GameObject opgo = GameObject.FindGameObjectWithTag(op.Tag.ToString());
            opgo.GetComponent<Image>().raycastTarget = true;
        }
        GameObject pieceGUI = GameObject.FindGameObjectWithTag(piece.Tag.ToString());
        pieceGUI.GetComponent<RectTransform>().SetParent(GameObject.Find("Canvas/Hive").transform, false);
        await AnimateGameObjectToEvent(pieceGUI, to);

        if (ShowDroppingAnimationForAI)
        {
            GameObject pieceDropAnim = Instantiate(Resources.Load("Piece Drop Anim") as GameObject, new Vector2(to.x, to.y), Quaternion.identity);
            pieceDropAnim.transform.SetParent(GameObject.Find("Canvas/Hive").transform, false);
            StartCoroutine(DelayedDestroy(pieceDropAnim, DroppingAnimationDuration));
        }
        _DetectGameOver();
        _SwitchTurn();
        IsAISearching = false;
        moveCount++;
    }

    protected void _MakeValidMove((Piece piece, (int x, int y) to) move)
    {
        // UnityEngine.Debug.Log($"Now moving {move.piece} {move.to}");
        // Update Core
        Board.MovePiece(move.piece, move.to, true);
        DropValidGUIPieceEvent(move.piece.Tag.ToString(), move.to.x, move.to.y, move.piece.IsOnHive ? Board.Hive[move.to].Count : -1);
        // Animate back the pieces at the previous point
        (int x, int y) oldPoint = whoseTurn == Player.Black ? _prevBlackMove.Item2 : _prevWhiteMove.Item2;
        if (MoveTrackers) DisplayGUITrackersEvent(oldPoint, move.to);
    }

    public void ResetGame()
    {
        Board.ResetState();
        RemoveGUIPiecesAndTrackersEvent(Board);
        SetupStartingGUIEvent(Board, isOnline, startingGameAnimation);
        isGameOver = false;
        RestartHivePositionEvent();
        ZoomHiveEvent(false, true);
        whoseTurn = Player.White;
    }

    private IEnumerator DelayedDestroy(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(obj);
    }
}