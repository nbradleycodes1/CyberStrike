using HiveCore;
using static HiveCore.Utils;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Player = HiveCore.Utils.Player;
using System.Diagnostics;
using System.Threading.Tasks;

public class AIvsAI : GameManager
{
    public static event Action<bool, bool> AIvsAIGameOver;
    // public static event Action<string, float, float, int> AIMoveGUIPieceEvent;
    public static int playManyGames = 100;
    private bool AIPlayer1FirstMove = true;
    private bool AIPlayer2FirstMove = true;
    Stopwatch GameTimer = new Stopwatch();
    private Dictionary<string, int> weights1;
    private Dictionary<string, int> weights2;
    public bool canAISearch = true;

    protected void _MakeValidMoveAIvsAI((Piece piece, (int x, int y) to) move)
    {
        Board.MovePiece(move.piece, move.to, true);
        int stackCountAtPosOnHive = Board.Hive.ContainsKey(move.to) ? Board.Hive[move.to].Count : -10;

        // Update GUI
        GameObject activeGUIPieceObj = GameObject.FindWithTag(move.piece.Tag.ToString());
        activeGUIPieceObj.GetComponent<RectTransform>().SetParent(GameObject.Find("Canvas/Hive").transform, true);
        activeGUIPieceObj.GetComponent<RectTransform>().anchoredPosition = stackCountAtPosOnHive > 1 ? new Vector2(move.to.x + (stackCountAtPosOnHive * 2), move.to.y + (stackCountAtPosOnHive * 2)) : new Vector2(move.to.x, move.to.y);
    }

    void _DetectAIvsAIGameOver()
    {
        bool blackQueenSurrounded = Board.BlackPieces[Q1].IsSurrounded;
        bool whiteQueenSurrounded = Board.WhitePieces[Q1].IsSurrounded;
        if (Board.IsDraw())
        {
            AIvsAIGameOver(false, true);
            isGameOver = true;
        }
        else if (blackQueenSurrounded)
        {
            AIvsAIGameOver(true, false);
            isGameOver = true;
        }
        else if (whiteQueenSurrounded)
        {
            AIvsAIGameOver(false, false);
            isGameOver = true;
        }
        else if (GameTimer.ElapsedMilliseconds >= 300000)
        {
            // If a game goes more than 5 minutes, call it a draw and move on.
            GameTimer.Stop();
            GameTimer.Reset();
            AIvsAIGameOver(false,true);
            isGameOver = true;
        }
    }

    public void updateCSVFile()
    {
        int player1WinOrLoss;
        int player2WinOrLoss;

        string heuristicValuesToCSV;

        // If it is a draw, Write 0
        if (Board.IsDraw())
        {
            heuristicValuesToCSV = "";
            weights1.ToList().ForEach(weight => heuristicValuesToCSV += $"{weight.Value}, ");
            player1WinOrLoss = 0;
            heuristicValuesToCSV += player1WinOrLoss + "\n";
            WhiteWriteToCSVFile(heuristicValuesToCSV);

            heuristicValuesToCSV = "";
            weights2.ToList().ForEach(weight => heuristicValuesToCSV += $"{weight.Value}, ");
            player2WinOrLoss = 0;
            heuristicValuesToCSV += player2WinOrLoss + "\n";
            BlackWriteToCSVFile(heuristicValuesToCSV);
        }

        else
        {
            GameTimer.Start();
            // Player 2 (White) data to CSV file (weights1)
            heuristicValuesToCSV = "";
            weights1.ToList().ForEach(weight => heuristicValuesToCSV += $"{weight.Value}, ");
            player2WinOrLoss = Board.WhitePieces[Q1].IsSurrounded ? -1 : 1;
            heuristicValuesToCSV += player2WinOrLoss + "\n";
            WhiteWriteToCSVFile(heuristicValuesToCSV);

            // Player 1 (Black) data to CSV file (weights2)
            heuristicValuesToCSV = "";
            weights2.ToList().ForEach(weight => heuristicValuesToCSV += $"{weight.Value}, ");
            player1WinOrLoss = Board.WhitePieces[Q1].IsSurrounded ? 1 : -1;
            heuristicValuesToCSV += player1WinOrLoss + "\n";
            BlackWriteToCSVFile(heuristicValuesToCSV);
        }
    }

    void Update()
    {
        if (!isGameOver)
        {
            if (canAISearch)
            {
                if (whoseTurn == AIPlayer2)
                {
                    if (AIPlayer2FirstMove)
                    {
                        // WHITE PLAYER
                        weights1 = new Dictionary<string, int>();
                        weights1 = Board._RandomizeHeuristicWeights(Player.White);
                        string heuristicValues1Print = "White's Heuristic Values:\n\n";
                        weights1.ToList().ForEach(weight => heuristicValues1Print += $"Variable: {weight.Key} => {weight.Value}\n");
                        UnityEngine.Debug.Log($"{heuristicValues1Print}");
                    }
                    AI2MoveAsync();
                }
                else
                {
                    if (AIPlayer1FirstMove)
                    {
                        // BLACK PLAYER
                        weights2 = new Dictionary<string, int>();
                        weights2 = Board._RandomizeHeuristicWeights(Player.Black);
                        string heuristicValues2Print = "Black's Heuristic Values:\n\n";
                        weights2.ToList().ForEach(weight => heuristicValues2Print += $"Variable: {weight.Key} => {weight.Value}\n");
                        UnityEngine.Debug.Log($"{heuristicValues2Print}");
                    }
                    AI1MoveAsync();
                }
            }
        }
        else
        {
            UnityEngine.Debug.Log("game is over, updating csv and restarting game");
            updateCSVFile();
            --playManyGames;
            AIPlayer1FirstMove = true;
            AIPlayer2FirstMove = true;
            if (playManyGames >= 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    private async void AI2MoveAsync()
    {
        // Stopwatch stopwatch = new Stopwatch();
        // stopwatch.Start();
        canAISearch = false;
        // WHITE AI will go as deep as it can within 6 seconds
        const int MIN = 1000000000;
        const int MAX = -1000000000;

        Dictionary<string, int> testedHeuristicWeights = new Dictionary<string, int>()
        {
            {"queenSurroundedWeight", 8},
            {"antOnBoardWeight", 5},
            {"piecesOnBoardWeight", 3},
            {"queenPinWeight", 3},
            {"beetledQueenWeight", 5},
            {"queenDefenderWeight", 2},
            {"beetleQueenDistanceWeight", 10},
            {"playQueenWeight", 15}
        };

        (float eval, (Piece piece, (int x, int y) to)) = await Task.Run(() => Board.AlphaBeta(whoseTurn, AIPlayer2, MAX, MIN, ALPHA_BETA_DEPTH, weights1, 0, isEasyMode));
        // (float eval, (Piece piece, (int x, int y) to)) = await Task.Run(() => Board.AlphaBeta(whoseTurn, AIPlayer2, MAX, MIN, ALPHA_BETA_DEPTH, testedHeuristicWeights));

        // UnityEngine.Debug.Log($"White eval: {eval}");
        _MakeValidMoveAIvsAI((piece, to));
        _DetectAIvsAIGameOver();
        _SwitchTurn();

        // stopwatch.Stop();
        AIPlayer2FirstMove = false;
        canAISearch = true;
    }

    private async void AI1MoveAsync()
    {
        // Stopwatch stopwatch = new Stopwatch();
        // stopwatch.Start();
        canAISearch = false;
        // BLACK AI will only go a certain depth, not looking at time 
        const int MIN = 1000000000;
        const int MAX = -1000000000;

        Dictionary<string, int> testedHeuristicWeights = new Dictionary<string, int>()
        {
            {"queenSurroundedWeight", 8},
            {"antOnBoardWeight", 5},
            {"piecesOnBoardWeight", 3},
            {"queenPinWeight", 3},
            {"beetledQueenWeight", 5},
            {"queenDefenderWeight", 7},
            {"beetleQueenDistanceWeight", 30},
            {"playQueenWeight", 15}
        };

        // (float eval, (Piece piece, (int x, int y) to)) = await Task.Run(() => Board.AlphaBeta(whoseTurn, AIPlayer1, MAX, MIN, ALPHA_BETA_DEPTH, testedHeuristicWeights));
        (float eval, (Piece piece, (int x, int y) to)) = await Task.Run(() => Board.AlphaBeta(whoseTurn, AIPlayer1, MAX, MIN, ALPHA_BETA_DEPTH, weights2, 0, isEasyMode));

        // UnityEngine.Debug.Log($"Black eval: {eval}");
        _MakeValidMoveAIvsAI((piece, to));
        _DetectAIvsAIGameOver();
        _SwitchTurn();

        // stopwatch.Stop();
        AIPlayer1FirstMove = false;
        canAISearch = true;
    }
}