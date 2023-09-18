using HiveCore;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using static HiveCore.Utils;
using static HiveCore.States;
using Move = System.ValueTuple<HiveCore.Piece, System.ValueTuple<int, int>>;
using PlayedPiece = System.ValueTuple<System.ValueTuple<int, int>, System.Collections.Generic.Stack<HiveCore.Piece>>;
using Unity.Collections;
using Unity.Jobs;

#pragma warning disable IDE1006 // Private members naming style
#nullable enable

// public struct GenerateMoves : IJob, Board
// {
//     public NativeHashSet<Move> Moves;
//     public NativeMultiHashMap<PlayedPiece> PlayedPieces;

//     public void Execute()
//     {

//     }
// }

namespace HiveCore
{
    public partial class Board
    {
        // For benchmarking only
        private int moveCount = 0;

        private bool queen = true;
        private int curMove = 0;
        private int manyCalculations = 0;
        private double totalTimeSoFar = 0;
        public int manyTerminalStatesFound = 0;
        public int timesPruned = 0;
        private int antPinWeight = 1;
        private int antOnBoardWeight = 1;
        private int queenSurroundedWeight = 1;
        private int queenPinWeight = 1;
        private int beetledQueenWeight = 1;
        private int playQueenWeight = 1;
        private int queenMobileWeight = 1;
        private int movesAvailWeight = 1;
        private int spawnsAvailWeight = 1;
        private int queenSpawnWeight = 1;
        private int queenDefenderWeight = 1;
        private int oppMovesOnQueenWeight = 1;
        private int piecesOnBoardWeight = 1;
        private int placingPositionsWeight = 1;
        private int beetleQueenDistanceWeight = 1;

        // **********************************


        /*************************************************************************
        **************************************************************************
                                            AI
        *************************************************************************
        *************************************************************************/
        private float _Evaluate(Player curPlayer, float alpha, float beta, Dictionary<string, int> weights, int moveCounter, bool easy, (Piece, (int, int))  curMove)
        {
            /**
            Cheapest & Valuable -> 
            Grasshopper - Cheap
            Queen - Cheap
            Beetle - Cheap
            Ant - Expensive
            Spider - Expensive
            **/

            /**
            This file lets you examine how each board state is being evaluated and hashed
            because you can also see the board state–i.e., piece's positions
            This one should be ok to be large, cause Unity does not compile it
            **/

            // WriteToTXTFile(Pieces, curHash, alpha, beta, eval);

            // Watchout with writing to the STATES table. If it goes above 9k entries (which does not take much), Unity can't handle it :/
            // UpdateStatesTable(curHash, alpha, beta, eval);

            // HashSet<(Piece, (int, int))> myMoves = GenerateMovesFor(curPlayer);
            // HashSet<(Piece, (int, int))> opponentMoves = GenerateMovesFor(curPlayer == Color.Black ? Color.White : Color.Black);
            // HashSet<(int, int)> placingPositions = GetPlacingSpotsFor(curPlayer);
            // HashSet<(int, int)> opponentPositions = GetPlacingSpotsFor(curPlayer == Color.Black ? Color.White : Color.Black);

            float eval = 0;
            _QueenSurrounded(curPlayer, weights["queenSurroundedWeight"], ref eval);
            _PiecesOnBoard(curPlayer, weights["piecesOnBoardWeight"], ref eval);
            _BeetleQueenDistance(curPlayer, weights["beetleQueenDistanceWeight"], ref eval);
            
            if (!easy)
            {
                if (BlackPieces[Q1].IsOnHive && WhitePieces[Q1].IsOnHive)
                {
                    _AntOnBoard(curPlayer, weights["antOnBoardWeight"], ref eval);
                }

                _QueenPinnedOrMobile(curPlayer, weights["queenPinWeight"], ref eval);
                _BeetleOnTopOfTheCircuitIsAGoodThing(curPlayer, weights["beetleOnTopOfTheCircuitIsAGoodThingWeight"], ref eval);
            
                _DefendersOnTheQueen(curPlayer, weights["queenDefenderWeight"], weights["queenDefenderWeight"], ref eval);
                if (curMove.Item1.Insect != Insect.QueenBee && moveCounter == 1)
                {
                _PlayQueen(curPlayer,weights["playQueenWeight"], ref eval);
                }
            }
            else
            {
               _DetectRepeat(curPlayer, ref eval);
            }
            
            // _AntPinned(curPlayer, weights["antPinWeight"], ref eval);
            // _PotentialSpawns(curPlayer, placingPositions, weights["spawnsAvailWeight"], ref eval);
            // _MovesAvailable(myMoves, placingPositions, weights["placingPositionsWeight"], ref eval);
            // _EnemyCanSpawnByYourQueen(curPlayer, opponentPositions, weights["queenSpawnWeight"] * (float)1.5, weights["queenSpawnWeight"], ref eval);
            // _OpponentHasNoMovesOnTheQueen(curPlayer, weights["oppMovesOnQueenWeight"], myMoves, opponentMoves, ref eval);

            //  _game_states[_GetCurrentHash()] = new Dictionary<string, float>{ {"alpha", alpha } , {"beta", beta }, {"eval", eval }};

            ++manyTerminalStatesFound;
            return eval;
        }

        #region Heuristics

        private void _QueenSurrounded(Player curPlayer, int weight, ref float eval)
        {
            // ***************************************************************************************
            // Enemy Queen Surrounded -> ∞ (+)
            // Current Player's Queen surrounded -> -∞ (-)
            float surroundedWeight = 10000;

            // 4^n points for n pieces around queen
            int manyPiecesAroundOpponentsQueen = curPlayer == Player.Black ? WhitePieces[Q1].Neighbors.Count : BlackPieces[Q1].Neighbors.Count;
            int manyPiecesAroundMyQueen = curPlayer == Player.Black ? BlackPieces[Q1].Neighbors.Count : WhitePieces[Q1].Neighbors.Count;

            if (manyPiecesAroundOpponentsQueen == 6)
            {
                eval += surroundedWeight;
            }
            if (manyPiecesAroundMyQueen == 6)
            {
                eval -= surroundedWeight;
            }

            eval += (int)Math.Pow(weight, manyPiecesAroundOpponentsQueen);
            eval -= (int)Math.Pow(weight, manyPiecesAroundMyQueen);
            // ***************************************************************************************
        }

        private void _AntOnBoard(Player curPlayer, int weight, ref float eval)
        {
            // If curPlayer is white, check Blacks moves
            if (curPlayer == Player.White)
            {
                // Enemy ants pinned ? +20 if not, -20
                eval += BlackPieces[A1].IsOnHive ? 0 : -weight;
                eval += BlackPieces[A2].IsOnHive ? 0 : -weight;
                eval += BlackPieces[A3].IsOnHive ? 0 : -weight;

                eval += WhitePieces[A1].IsOnHive ? weight : 0;
                eval += WhitePieces[A2].IsOnHive ? weight : 0;
                eval += WhitePieces[A3].IsOnHive ? weight : 0;

            }

            // Otherwise, check white's moves
            else
            {
                eval += BlackPieces[A1].IsOnHive ? weight : 0;
                eval += BlackPieces[A2].IsOnHive ? weight : 0;
                eval += BlackPieces[A3].IsOnHive ? weight : 0;

                eval += WhitePieces[A1].IsOnHive ? 0 : -weight;
                eval += WhitePieces[A2].IsOnHive ? 0 : -weight;
                eval += WhitePieces[A3].IsOnHive ? 0 : -weight;
            }
        }
        
        // private void _BeetleQueenDistance(Player curPlayer, int weight, ref float eval)
        // {
        //     if (curPlayer == Player.White)
        //     {
        //         int x1 = Math.Abs(BlackPieces[Q1].Point.x - WhitePieces[B1].Point.x);
        //         int y1 = Math.Abs(BlackPieces[Q1].Point.y - WhitePieces[B1].Point.y);


        //         // On top
        //         if(x1 == 0 && y1 == 0)
        //             eval += weight * (float).8;

        //         // One level away
        //         else if(x1 <= 48 && y1 <= 56)
        //             eval += weight * (float).4;

        //         // Two levels away
        //         else if(x1 <= 96 && y1 <= 112)
        //         {
        //             if(x1 + y1 <= 152)
        //                 eval += weight * (float).3;
        //         }
                
        //         // Three levels away
        //         else if(x1 <= 144 && y1 <= 168)
        //         {
        //             if(x1 + y1 <= 228)
        //                 eval += weight * (float).2;
        //         }

        //         // Four levels away
        //         else if(x1 <= 192 && y1 <= 224)
        //         {
        //             if(x1 + y1 <= 304)
        //                 eval += weight * (float).1;
        //         }
        //     }
        //     if (curPlayer == Player.White)
        //     {
        //         int x1 = Math.Abs(BlackPieces[Q1].Point.x - WhitePieces[B1].Point.x);
        //         int y1 = Math.Abs(BlackPieces[Q1].Point.y - WhitePieces[B1].Point.y);

        //         // One level away
        //         else if(x1 <= 48 && y1 <= 56)
        //             eval += weight * (float).4;

        //         // Two levels away
        //         else if(x1 <= 96 && y1 <= 112)
        //         {
        //             if(x1 + y1 <= 152)
        //                 eval += weight * (float).3;
        //         }
                
        //         // Three levels away
        //         else if(x1 <= 144 && y1 <= 168)
        //         {
        //             if(x1 + y1 <= 228)
        //                 eval += weight * (float).2;
        //         }

        //         // Four levels away
        //         else if(x1 <= 192 && y1 <= 224)
        //         {
        //             if(x1 + y1 <= 304)
        //                 eval += weight * (float).1;
        //         }
        //     }
        // }

        private void _BeetleQueenDistance(Player curPlayer, int weight, ref float eval)
        {
            if (curPlayer == Player.White)
            {
                int x1 = Math.Abs(BlackPieces[Q1].Point.x - WhitePieces[B1].Point.x);
                int y1 = Math.Abs(BlackPieces[Q1].Point.y - WhitePieces[B1].Point.y);
                int x2 = Math.Abs(BlackPieces[Q1].Point.x - WhitePieces[B2].Point.x);
                int y2 = Math.Abs(BlackPieces[Q1].Point.y - WhitePieces[B2].Point.y);

                // On top
                if(x1 == 0 && y1 == 0)
                    eval += weight * (float).8;

                // One level away
                else if(x1 <= 48 && y1 <= 56)
                    eval += weight * (float).4;

                // Two levels away
                else if(x1 <= 96 && y1 <= 112)
                {
                    if(x1 + y1 <= 152)
                        eval += weight * (float).3;
                }
                
                // Three levels away
                else if(x1 <= 144 && y1 <= 168)
                {
                    if(x1 + y1 <= 228)
                        eval += weight * (float).2;
                }

                // Four levels away
                else if(x1 <= 192 && y1 <= 224)
                {
                    if(x1 + y1 <= 304)
                        eval += weight * (float).1;
                }

                // On top
                if(x2 == 0 && y2 == 0)
                    eval += weight * (float).8;

                // One level away
                else if(x2 <= 48 && y2 <= 56)
                    eval += weight * (float).4;

                // Two levels away
                else if(x2 <= 96 && y2 <= 112)
                {
                    if(x2 + y2 <= 152)
                        eval += weight * (float).3;
                }
                
                // Three levels away
                else if(x2 <= 144 && y2 <= 168)
                {
                    if(x2 + y2 <= 228)
                        eval += weight * (float).2;
                }

                // Four levels away
                else if(x2 <= 192 && y2 <= 224)
                {
                    if(x2 + y2 <= 304)
                        eval += weight * (float).1;
                }

                // opponent
                int ox1 = Math.Abs(WhitePieces[Q1].Point.x - BlackPieces[B1].Point.x);
                int oy1 = Math.Abs(WhitePieces[Q1].Point.y - BlackPieces[B1].Point.y);
                int ox2 = Math.Abs(WhitePieces[Q1].Point.x - BlackPieces[B2].Point.x);
                int oy2 = Math.Abs(WhitePieces[Q1].Point.y - BlackPieces[B2].Point.y);
                
                // On top
                if(ox1 == 0 && oy1 == 0)
                    eval -= weight;

                // One level away
                else if(ox1 <= 48 && oy1 <= 56)
                    eval -= weight * (float).8;

                // Two levels away
                else if(ox1 <= 96 && oy1 <= 112)
                {
                    if(ox1 + oy1 <= 152)
                        eval -= weight * (float).6;
                }
                
                // Three levels away
                else if(ox1 <= 144 && oy1 <= 168)
                {
                    if(ox1 + oy1 <= 228)
                        eval -= weight * (float).4;
                }

                // Four levels away
                else if(ox1 <= 192 && oy1 <= 224)
                {
                    if(ox1 + oy1 <= 304)
                        eval -= weight * (float).2;
                }

                // On top
                if(ox2 == 0 && oy2 == 0)
                    eval -= weight;

                // One level away
                else if(x2 <= 48 && oy2 <= 56)
                    eval -= weight * (float).8;

                // Two levels away
                else if(ox2 <= 96 && oy2 <= 112)
                {
                    if(ox2 + oy2 <= 152)
                        eval -= weight * (float).6;
                }
                
                // Three levels away
                else if(ox2 <= 144 && oy2 <= 168)
                {
                    if(ox2 + oy2 <= 228)
                        eval -= weight * (float).4;
                }

                // Four levels away
                else if(ox2 <= 192 && oy2 <= 224)
                {
                    if(ox2 + oy2 <= 304)
                        eval -= weight * (float).2;
                }
            }
            else
            {
                int x1 = Math.Abs(WhitePieces[Q1].Point.x - BlackPieces[B1].Point.x);
                int y1 = Math.Abs(WhitePieces[Q1].Point.y - BlackPieces[B1].Point.y);
                int x2 = Math.Abs(WhitePieces[Q1].Point.x - BlackPieces[B2].Point.x);
                int y2 = Math.Abs(WhitePieces[Q1].Point.y - BlackPieces[B2].Point.y);
                
                // On top
                if(x1 == 0 && y1 == 0)
                    eval += weight * (float).8;

                // One level away
                else if(x1 <= 48 && y1 <= 56)
                    eval += weight * (float).4;

                // Two levels away
                else if(x1 <= 96 && y1 <= 112)
                {
                    if(x1 + y1 <= 152)
                        eval += weight * (float).3;
                }
                
                // Three levels away
                else if(x1 <= 144 && y1 <= 168)
                {
                    if(x1 + y1 <= 228)
                        eval += weight * (float).2;
                }

                // Four levels away
                else if(x1 <= 192 && y1 <= 224)
                {
                    if(x1 + y1 <= 304)
                        eval += weight * (float).1;
                }

                // On top
                if(x2 == 0 && y2 == 0)
                    eval += weight * (float).8;

                // One level away
                else if(x2 <= 48 && y2 <= 56)
                    eval += weight * (float).4;

                // Two levels away
                else if(x2 <= 96 && y2 <= 112)
                {
                    if(x2 + y2 <= 152)
                        eval += weight * (float).3;
                }
                
                // Three levels away
                else if(x2 <= 144 && y2 <= 168)
                {
                    if(x2 + y2 <= 228)
                        eval += weight * (float).2;
                }

                // Four levels away
                else if(x2 <= 192 && y2 <= 224)
                {
                    if(x2 + y2 <= 304)
                        eval += weight * (float).1;
                }
                int ox1 = Math.Abs(BlackPieces[Q1].Point.x - WhitePieces[B1].Point.x);
                int oy1 = Math.Abs(BlackPieces[Q1].Point.y - WhitePieces[B1].Point.y);
                int ox2 = Math.Abs(BlackPieces[Q1].Point.x - WhitePieces[B2].Point.x);
                int oy2 = Math.Abs(BlackPieces[Q1].Point.y - WhitePieces[B2].Point.y);

                // On top
                if(ox1 == 0 && oy1 == 0)
                    eval -= weight;

                // One level away
                else if(ox1 <= 48 && oy1 <= 56)
                    eval -= weight * (float).8;

                // Two levels away
                else if(ox1 <= 96 && oy1 <= 112)
                {
                    if(ox1 + oy1 <= 152)
                        eval -= weight * (float).6;
                }
                
                // Three levels away
                else if(ox1 <= 144 && oy1 <= 168)
                {
                    if(ox1 + oy1 <= 228)
                        eval -= weight * (float).4;
                }

                // Four levels away
                else if(ox1 <= 192 && oy1 <= 224)
                {
                    if(ox1 + oy1 <= 304)
                        eval -= weight * (float).2;
                }

                // On top
                if(ox2 == 0 && oy2 == 0)
                    eval -= weight;

                // One level away
                else if(ox2 <= 48 && oy2 <= 56)
                    eval -= weight * (float).8;

                // Two levels away
                else if(ox2 <= 96 && oy2 <= 112)
                {
                    if(ox2 + oy2 <= 152)
                        eval -= weight * (float).6;
                }
                
                // Three levels away
                else if(ox2 <= 144 && oy2 <= 168)
                {
                    if(ox2 + oy2 <= 228)
                        eval -= weight * (float).4;
                }

                // Four levels away
                else if(ox2 <= 192 && oy2 <= 224)
                {
                    if(ox2 + oy2 <= 304)
                        eval -= weight * (float).2;
                }
            }
        } 

        private void _PiecesOnBoard(Player curPlayer, int weight, ref float eval)
        {
            eval += _GetManyPiecesPlayedBy(curPlayer) * weight;
        }

        private void _QueenPinnedOrMobile(Player curPlayer, int weight, ref float eval)
        {
            // ***************************************************************************************
            // Enemy Queen pinned (+) or mobile (-)
            // Current Player's Queen pinned (-) or mobile (+)

            // if curPlayer is White, check Black's moves
            if (curPlayer == Player.White)
            {
                HashSet<(int, int)> queenMoves = new HashSet<(int, int)>();

                // Check opponent Queen
                if (BlackPieces[Q1].IsOnHive)
                {
                    eval += _IsPinned(ref BlackPieces[Q1]) ? weight : -weight;
                }

                // Check your Queen
                if (WhitePieces[Q1].IsOnHive)
                {
                    eval += _IsPinned(ref WhitePieces[Q1]) ? -weight : weight;
                }

            }
            // Otherwise, check White's moves
            else
            {
                HashSet<(int, int)> queenMoves = new HashSet<(int, int)>();

                // Check opponent Queen
                if (WhitePieces[Q1].IsOnHive)
                {
                    eval += _IsPinned(ref WhitePieces[Q1]) ? weight : -weight;
                }

                // Check your Queen
                if (BlackPieces[Q1].IsOnHive)
                {
                    eval += _IsPinned(ref BlackPieces[Q1]) ? -weight : weight;
                }
            }

            // ***************************************************************************************
        }
        
        private void _BeetleOnTopOfTheCircuitIsAGoodThing(Player curPlayer, int weight, ref float eval)
        {
            // Good points for the AI :)
            if ((curPlayer == Player.Black ? (BlackPieces[B1].IsTop && Hive[BlackPieces[B1].Point].Count > 0) : (WhitePieces[B1].IsTop && Hive[WhitePieces[B1].Point].Count > 0)))
            {
                eval += weight;
            }
            if ((curPlayer == Player.Black ? (BlackPieces[B2].IsTop && Hive[BlackPieces[B2].Point].Count > 0) : (WhitePieces[B2].IsTop && Hive[WhitePieces[B2].Point].Count > 0)))
            {
                eval += weight;
            }

            // Bad points for the other team :(
            if ((curPlayer == Player.Black ? (WhitePieces[B1].IsTop && Hive[WhitePieces[B1].Point].Count > 0) : (BlackPieces[B1].IsTop && Hive[BlackPieces[B1].Point].Count > 0)))
            {
                eval -= weight;
            }
            if ((curPlayer == Player.Black ? (WhitePieces[B2].IsTop && Hive[WhitePieces[B2].Point].Count > 0) : (BlackPieces[B2].IsTop && Hive[BlackPieces[B2].Point].Count > 0)))
            {
                eval -= weight;
            }
        }
        
        private void _BeetleOnTopOfQueen(Player curPlayer, int weight, ref float eval)
        {
            // ***************************************************************************************
            // Beetle on top of opponent's Queen? (+)
            // Opponent's Beetle on top of current player's Queen? (-)
            if (curPlayer == Player.Black)
            {
                if (WhitePieces[Q1].Point == BlackPieces[B1].Point || WhitePieces[Q1].Point == BlackPieces[B2].Point)
                {
                    eval += weight;
                }
                if (BlackPieces[Q1].Point == WhitePieces[B1].Point || BlackPieces[Q1].Point == WhitePieces[B2].Point)
                {
                    eval -= weight;
                }
            }
            else
            {
                if (BlackPieces[Q1].Point == WhitePieces[B1].Point || BlackPieces[Q1].Point == WhitePieces[B2].Point)
                {
                    eval += weight;
                }
                if (WhitePieces[Q1].Point == BlackPieces[B1].Point || WhitePieces[Q1].Point == BlackPieces[B2].Point)
                {
                    eval -= weight;
                }
            }

            // ***************************************************************************************
        }

        private void _DefendersOnTheQueen(Player curPlayer, int weight1, float weight2, ref float eval)
        {
            // ***************************************************************************************
            // Defenders on the Queen? (+)
            // Opponent Queen on Queen? (-) -> leads to more ties
            if (curPlayer == Player.Black)
            {
                foreach ((int, int) neighbor in BlackPieces[Q1].Neighbors)
                {
                    if (neighbor == BlackPieces[G1].Point || neighbor == BlackPieces[G2].Point
                    || neighbor == BlackPieces[B1].Point || neighbor == BlackPieces[B2].Point)
                    {
                        eval += weight1;
                    }
                    if (neighbor == WhitePieces[Q1].Point)
                    {
                        eval -= weight2;
                    }
                }
            }
            else
            {
                foreach ((int, int) neighbor in WhitePieces[Q1].Neighbors)
                {
                    if (neighbor == WhitePieces[G1].Point || neighbor == WhitePieces[G2].Point
                    || neighbor == WhitePieces[B1].Point || neighbor == WhitePieces[B2].Point)
                    {
                        eval += weight1;
                    }
                    if (neighbor == BlackPieces[Q1].Point)
                    {
                        eval -= weight2;
                    }
                }
            }

            // ***************************************************************************************
        }

        private void _PlayQueen(Player curPlayer, int weight, ref float eval)
        {
            if (curPlayer == Player.Black ? BlackPieces[Q1].IsOnHive : WhitePieces[Q1].IsOnHive)
            {
                eval += weight;
            }
            // if(curPlayer == Player.Black ? WhitePieces[Q1].IsOnHive: BlackPieces[Q1].IsOnHive)
            // {
            //     eval -= weight;
            // }
        }

        private void _DetectRepeat(Player curPlayer, ref float eval)    
        {
            if (IsRepeatingMoves(curPlayer))
            {
                eval = -1100000;
            }
        }

        private void _AntPinned(Player curPlayer, float weight, ref float eval)
        {
            // ***************************************************************************************
            // Enemy ant pinned? (+)
            // Current player's ants pinned? (-)

            // If curPlayer is white, check Blacks moves
            if (curPlayer == Player.White)
            {
                // Enemy ants pinned ? +20 if not, -20
                eval += _IsPinned(ref BlackPieces[A1]) ? weight : -weight;
                eval += _IsPinned(ref BlackPieces[A1]) ? weight : -weight;
                eval += _IsPinned(ref BlackPieces[A1]) ? weight : -weight;

                // Current player's ants pinned ? -20 if not, +20
                eval += _IsPinned(ref WhitePieces[A1]) ? -weight : weight;
                eval += _IsPinned(ref WhitePieces[A2]) ? -weight : weight;
                eval += _IsPinned(ref WhitePieces[A3]) ? -weight : weight;
            }

            // Otherwise, check white's moves
            else
            {
                // Enemy ants pinned ? +20 if not, -20
                eval += _IsPinned(ref WhitePieces[A1]) ? weight : -weight;
                eval += _IsPinned(ref WhitePieces[A2]) ? weight : -weight;
                eval += _IsPinned(ref WhitePieces[A3]) ? weight : -weight;

                // Current player's ants pinned ? -20 if not, +20
                eval += _IsPinned(ref BlackPieces[A1]) ? -weight : weight;
                eval += _IsPinned(ref BlackPieces[A2]) ? -weight : weight;
                eval += _IsPinned(ref BlackPieces[A3]) ? -weight : weight;
            }

            // ***************************************************************************************
        }

        private void _PotentialSpawns(Player curPlayer, HashSet<(int, int)> placingPositions, float weight, ref float eval)
        {
            // ***************************************************************************************
            // Potential spawn points on enemy queen w/ pieces in reserve?

            // If there are potential spawn points
            if (placingPositions.Count() > 0)
            {
                // If color is Black, check the open spots around white queen
                if (curPlayer == Player.Black)
                {
                    // Loop through every open spot on the queen
                    foreach ((int, int) openSpot in WhitePieces[Q1].OpenSpotsAround)
                    {
                        // Loop through every potential placing spot
                        foreach ((int, int) placingSpot in placingPositions)
                        {
                            // If a placing spot is the same as an open spot, you can spawn on opponent's queen
                            if (openSpot == placingSpot)
                            {
                                eval += weight;
                            }
                        }
                    }
                }

                else
                {
                    // Loop through every open spot on the queen
                    foreach ((int, int) openSpot in BlackPieces[Q1].OpenSpotsAround)
                    {
                        // Loop through every potential placing spot
                        foreach ((int, int) placingSpot in placingPositions)
                        {
                            // If a placing spot is the same as an open spot, you can spawn on opponent's queen
                            if (openSpot == placingSpot)
                            {
                                eval += weight;
                            }
                        }
                    }
                }
            }
            // ***************************************************************************************
        }

        private void _MovesAvailable(HashSet<(Piece, (int, int))> myMoves, HashSet<(int, int)> placingPositions, float weight, ref float eval)
        {
            // ***************************************************************************************
            // More moves available (+)
            // No moves available (-)
            if (placingPositions.Count() > 0)
            {
                eval += placingPositions.Count();
            }

            if (myMoves.Count() > 0)
            {
                eval += myMoves.Count();
            }
            else
            {
                eval -= weight;
            }

            // ***************************************************************************************
        }

        private void _EnemyCanSpawnByYourQueen(Player curPlayer, HashSet<(int, int)> opponentPositions, float weightIfCan, float weightIfCannot, ref float eval)
        {
            // ***************************************************************************************
            // Enemy can spawn by your queen (-), if not (+)

            // If there are potential spawn points
            if (opponentPositions.Count() > 0)
            {
                // If current player's color is Black, check the open spots around black queen
                if (curPlayer == Player.Black)
                {
                    // Loop through every open spot on the queen
                    foreach ((int, int) openSpot in BlackPieces[Q1].OpenSpotsAround)
                    {
                        // Loop through every potential placing spot
                        foreach ((int, int) placingSpot in opponentPositions)
                        {
                            // If a placing spot is the same as an open spot, opponent can spawn on Queen
                            if (openSpot == placingSpot)
                            {
                                eval -= weightIfCan;
                            }
                        }
                    }
                }

                // Current player's color is white, check the open spots around white Queen
                else
                {
                    // Loop through every open spot on the queen
                    foreach ((int, int) openSpot in WhitePieces[Q1].OpenSpotsAround)
                    {
                        // Loop through every potential placing spot
                        foreach ((int, int) placingSpot in opponentPositions)
                        {
                            // If a placing spot is the same as an open spot, opponent can spawn on Queen
                            if (openSpot == placingSpot)
                            {
                                eval -= weightIfCan;
                            }
                        }
                    }
                }
            }
            // enemy cannot spawn by your Queen
            else
            {
                eval += weightIfCannot;
            }

            // ***************************************************************************************
        }

        private void _OpponentHasNoMovesOnTheQueen(Player curPlayer, float weight, HashSet<(Piece, (int, int))> myMoves, HashSet<(Piece, (int, int))> opponentMoves, ref float eval)
        {
            // ***************************************************************************************
            // Opponent has no available moves on the Queen
            // ***************************************************************************************
            foreach (var move in myMoves)
            {
                foreach ((int, int) openSpot in curPlayer == Player.White ? BlackPieces[Q1].OpenSpotsAround : WhitePieces[Q1].OpenSpotsAround)
                {
                    if (move.Item2 == openSpot)
                    {
                        eval += weight;
                    }
                }
            }

            foreach (var move in opponentMoves)
            {
                foreach ((int, int) openSpot in curPlayer == Player.White ? WhitePieces[Q1].OpenSpotsAround : BlackPieces[Q1].OpenSpotsAround)
                {
                    if (move.Item2 == openSpot)
                    {
                        eval -= weight;
                    }
                }
            }
        }

        private void _PiecesThatHaveAvailMovesAndGivePiecesAWeight(float weight, ref float eval)
        {
            // ***************************************************************************************
            // Pieces that have available moves and give the pieces a weight (+)
            // TODO: Implement
            // ***************************************************************************************
            eval += 0;
        }
        #endregion

        public (float eval, (Piece piece, (int, int) to) move) AlphaBeta(Player curPlayer, Player AI, float alpha, float beta, int depth, Dictionary<string, int> weights, int moveCounter, bool isEasy, (Piece, (int, int)) curMove = default )
        {
            // TODO: Detect draws inside here
            bool AIWin = AI == Player.Black ? WhitePieces[Q1].IsSurrounded : BlackPieces[Q1].IsSurrounded;
            bool PlayerWin = AI == Player.Black ? BlackPieces[Q1].IsSurrounded : WhitePieces[Q1].IsSurrounded;
            
            // if (History[Player.Black].RepeatedForDraw() || History[Player.White].RepeatedForDraw())
            // {
            //     return (-100000 - depth, curMove);
            // }

            if (AIWin && PlayerWin)
                return (0, curMove);

            else if (AIWin || PlayerWin)
                return AIWin ? (1000000 + depth, curMove) : (-1000000 + _Evaluate(AI, alpha, beta, weights, moveCounter, isEasy, curMove), curMove);
            else if (depth == 0)
                return (_Evaluate(AI, alpha, beta, weights, moveCounter, isEasy, curMove), curMove);
            
            if (curPlayer == AI)
            {
                float best = -10000000000;
                Piece movePiece = default;
                (int, int) movePoint = (0, 0);
                int moveCount = 0;

                foreach (Piece piece in curPlayer == Player.Black ? BlackPieces : WhitePieces)
                {
                    HashSet<(int, int)> moves = GenerateMovesForPiece(piece);

                    moveCount += moves.Count;

                    if (piece.Index == S2 && moveCount == 0)
                    {
                        // add the sentinel for "NULL" move
                        moves.Add((-1, -1));
                    }

                    foreach ((int, int) move in moves)
                    {
                        (int, int) oldPoint = piece.Point;
                        (float, (Piece , (int, int))) evaluation;

                        if (move.Item1 != -1 && move.Item2 != -1)
                        {
                            // make move
                            MovePiece(piece, move);
                            evaluation = AlphaBeta(curPlayer == Player.Black ? Player.White : Player.Black, AI, alpha, beta, depth - 1, weights, moveCounter, isEasy, (piece, move));

                        }
                        else
                            evaluation = AlphaBeta(curPlayer == Player.Black ? Player.White : Player.Black, AI, alpha, beta, depth, weights, moveCounter, isEasy, (piece, move));

                        if (move.Item1 != -1 && move.Item2 != -1)
                        {
                            // undo move
                            MovePiece(piece, oldPoint);
                        }

                        if (best < evaluation.Item1)
                        {
                            best = evaluation.Item1;
                            movePiece = piece;
                            movePoint = move;
                        }
                        if (alpha < best)
                            alpha = best;
                        if (beta <= alpha)
                            break;
                    }
                }
                return (best, (movePiece, movePoint));
            }
            else
            {
                float best = 10000000000;

                Piece movePiece = default;
                (int, int) movePoint = (0, 0);
                int moveCount = 0;

                foreach (Piece piece in curPlayer == Player.Black ? BlackPieces : WhitePieces)
                {
                    HashSet<(int, int)> moves = GenerateMovesForPiece(piece);

                    moveCount += moves.Count;

                    if (piece.Index == S2 && moveCount == 0)
                    {
                        moves.Add((-1, -1));
                    }

                    foreach ((int, int) move in moves)
                    {
                        (int, int) oldPoint = piece.Point;
                        (float, (Piece , (int, int))) evaluation;

                        if (move.Item1 != -1 && move.Item2 != -1)
                        {
                            // make move
                            MovePiece(piece, move);
                            evaluation = AlphaBeta(curPlayer == Player.Black ? Player.White : Player.Black, AI, alpha, beta, depth - 1, weights, moveCounter, isEasy, (piece, move));

                        }
                        else
                            evaluation = AlphaBeta(curPlayer == Player.Black ? Player.White : Player.Black, AI, alpha, beta, depth - 1, weights, moveCounter, isEasy, (piece, move));

                        if (move.Item1 != -1 && move.Item2 != -1)
                        {
                            // undo move
                            MovePiece(piece, oldPoint);
                        }

                        if (best > evaluation.Item1)
                        {
                            best = evaluation.Item1;
                            movePiece = piece;
                            movePoint = move;
                        }
                        if (beta > best)
                            beta = best;
                        if (beta <= alpha)
                            break;
                    }
                }
                return (best, (movePiece, movePoint));
            }
        }

        #region AI Method Helpers
        private long _GetCurrentHash()
        {
            long hash = 0;
            for (int p = 0; p < 11; ++p)
            {
                hash ^= (_GetHashFor(WhitePieces[p]) ^ _GetHashFor(BlackPieces[p]));
            }
            return hash;
        }

        private long _GetHashFor(Piece piece)
        {
            long hash = 0;
            if (piece.IsOnHive)
            {
                HashSet<(int, int)> neighbors = Hive[piece.Point].Peek().Neighbors;
                if (neighbors.Count > 0)
                {
                    foreach ((int, int) neighbor in neighbors)
                    {
                        foreach (Piece neighborPiece in Hive[neighbor])
                        {
                            (int, int) offset = (neighbor.Item1 - piece.Point.x, neighbor.Item2 - piece.Point.y);
                            int direction = Array.IndexOf(SIDE_OFFSETS_ARRAY, offset);
                            hash ^= ((neighborPiece.Tag) << piece.Tag) ^ direction;
                        }
                    }
                }
                else
                {
                    hash ^= piece.Tag ^ (piece.Tag << piece.Index);
                }
            }
            else
            {
                hash ^= piece.Tag ^ piece.Index;
            }
            return hash;
        }
        #endregion

        #region AI vs. AI

        //*************************************************************************

        // TODO: Implement a function that will change the heuristic weights with a random value
        public Dictionary<string, int> _RandomizeHeuristicWeights(Player player)
        {
            Random seed = new Random();
            int newSeed = seed.Next();
            Random rand1 = new Random(newSeed);
            newSeed = seed.Next();
            Random rand2 = new Random(newSeed);

            Random rand = player == Player.White ? rand1 : rand2;

            queenSurroundedWeight = rand.Next(100);
            antOnBoardWeight = rand.Next(100);
            piecesOnBoardWeight = rand.Next(100);
            queenPinWeight = rand.Next(100);
            beetledQueenWeight = rand.Next(100);
            queenDefenderWeight = rand.Next(100);
            beetleQueenDistanceWeight = rand.Next(100);
            playQueenWeight = rand.Next(100);

            //antPinWeight = rand.Next(100);
            // movesAvailWeight = rand.Next(100);
            // spawnsAvailWeight = rand.Next(100);
            // queenMobileWeight = rand.Next(100);
            // queenSpawnWeight = rand.Next(100);
            // oppMovesOnQueenWeight = rand.Next(100);
            // placingPositionsWeight = rand.Next(100);

            Dictionary<string, int> weights = new Dictionary<string, int>()
            {
                {"queenSurroundedWeight", queenSurroundedWeight},
                {"antOnBoardWeight", antOnBoardWeight},
                {"piecesOnBoardWeight", piecesOnBoardWeight},
                {"queenPinWeight", queenPinWeight},
                {"beetledQueenWeight", beetledQueenWeight},
                {"queenDefenderWeight", queenDefenderWeight},
                {"beetleQueenDistanceWeight", beetleQueenDistanceWeight},
                {"playQueenWeight", playQueenWeight}
            };

                /* 
                {"antPinWeight", antPinWeight},
                {"movesAvailWeight", movesAvailWeight},
                {"queenMobileWeight", queenMobileWeight},
                {"spawnsAvailWeight", spawnsAvailWeight},
                {"queenSpawnWeight", queenSpawnWeight},
                {"oppMovesOnQueenWeight",oppMovesOnQueenWeight},
                {"placingPositionsWeight", placingPositionsWeight},
                */

            return weights;
        }
        #endregion

    }
}