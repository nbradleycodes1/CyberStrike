using HiveCore;
using static HiveCore.Utils;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Player = HiveCore.Utils.Player;
using Move = System.ValueTuple<HiveCore.Piece, System.ValueTuple<int, int>>;

public class MoveHistory
{
    private Move[] _moves;
    private int _nextInsertionIndex;
    private int _maxManyMoves;
    private int _arraySize;
    public int RegisteredMoves;
    public MoveHistory(int max)
    {
        _arraySize = max * 2;
        _moves = new Move[_arraySize];
        _maxManyMoves = max;
        _nextInsertionIndex = 0;
        RegisteredMoves = 0;
    }

    public void Add(Move move)
    {
        // UnityEngine.Debug.Log($"Now adding move to the history: {move.Item1} {move.Item2}");
        _moves[_nextInsertionIndex] = (move.Item1, move.Item2);
        RegisteredMoves = (RegisteredMoves + 1) > _arraySize ? _arraySize : (RegisteredMoves + 1);
        _nextInsertionIndex = (_nextInsertionIndex + 1) % _arraySize;
    }

    public bool RepeatedForDraw()
    {
        if (RegisteredMoves >= _maxManyMoves)
        {
            for (int curMove = 0; curMove < RegisteredMoves; ++curMove)
            {
                int count = 0;
                for (int nextMove = curMove + 1; nextMove < RegisteredMoves; ++nextMove)
                {
                    if (_moves[curMove].Item1.Equals(_moves[nextMove].Item1) && _moves[curMove].Item2 == _moves[nextMove].Item2)
                    {
                        ++count;
                    }

                    // if curMove was found in the rest of the array the remaining times necessary for a draw
                    if (count == _maxManyMoves - 1)
                    {
                        // UnityEngine.Debug.Log("Detected Draw! Here are the moves that prove it");
                        // foreach (var m in _moves)
                        // {
                        //     UnityEngine.Debug.Log($"{m.Item1} {m.Item2}");
                        // }
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public Move Pop()
    {
        _nextInsertionIndex = --_nextInsertionIndex < 0 ? 0 : _nextInsertionIndex; 
        // not the move just added, but the previous one to the move just added
        RegisteredMoves -= 2;
        return _moves[RegisteredMoves];
    }

    public void RemoveLast()
    {
        --RegisteredMoves;
        _nextInsertionIndex = --_nextInsertionIndex < 0 ? 0 : _nextInsertionIndex; 
    }

    public void Clear()
    {
        RegisteredMoves = 0;
        _nextInsertionIndex = 0;
    }
}