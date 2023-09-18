using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using static HiveCore.Utils;
using static HiveCore.States;

#pragma warning disable IDE1006 // Private members naming style
#nullable enable

namespace HiveCore
{
    public partial class Board
    {
        private Dictionary<long, Dictionary<string, float>> _hashed_evals;
        public Dictionary<(int, int), Stack<Piece>> Hive;
        public Dictionary<Player, MoveHistory> History;
        public Dictionary<Player, MoveHistory> HistoryHeuristicPurposes;


        public Piece[] WhitePieces =
        {
            new Piece(wG1) { DefaultPoint = (-HAND_X[0], HAND_Y[2]), Point = (-HAND_X[0], HAND_Y[2])},
            new Piece(wG2) { DefaultPoint = (-HAND_X[1], HAND_Y[2]), Point = (-HAND_X[1], HAND_Y[2])},
            new Piece(wG3) { DefaultPoint = (-HAND_X[2], HAND_Y[2]), Point = (-HAND_X[2], HAND_Y[2])},

            new Piece(wQ1) { DefaultPoint = (-HAND_X[1], HAND_Y[5]), Point = (-HAND_X[1], HAND_Y[5])},

            new Piece(wB1) { DefaultPoint = (-HAND_X[0], HAND_Y[3]), Point = (-HAND_X[0], HAND_Y[3])},
            new Piece(wB2) { DefaultPoint = (-HAND_X[1], HAND_Y[3]), Point = (-HAND_X[1], HAND_Y[3])},

            new Piece(wA1) { DefaultPoint = (-HAND_X[0], HAND_Y[1]), Point = (-HAND_X[0], HAND_Y[1])},
            new Piece(wA2) { DefaultPoint = (-HAND_X[1], HAND_Y[1]), Point = (-HAND_X[1], HAND_Y[1])},
            new Piece(wA3) { DefaultPoint = (-HAND_X[2], HAND_Y[1]), Point = (-HAND_X[2], HAND_Y[1])},

            new Piece(wS1) { DefaultPoint = (-HAND_X[0], HAND_Y[4]), Point = (-HAND_X[0], HAND_Y[4])},
            new Piece(wS2) { DefaultPoint = (-HAND_X[1], HAND_Y[4]), Point = (-HAND_X[1], HAND_Y[4])},

        };
        public Piece[] BlackPieces =
        {
            new Piece(bG1) { DefaultPoint = (HAND_X[0], HAND_Y[1]), Point = (HAND_X[0], HAND_Y[1])},
            new Piece(bG2) { DefaultPoint = (HAND_X[1], HAND_Y[1]), Point = (HAND_X[1], HAND_Y[1])},
            new Piece(bG3) { DefaultPoint = (HAND_X[2], HAND_Y[1]), Point = (HAND_X[2], HAND_Y[1])},

            new Piece(bQ1) { DefaultPoint = (HAND_X[1], HAND_Y[4]), Point = (HAND_X[1], HAND_Y[4])},

            new Piece(bB1) { DefaultPoint = (HAND_X[0], HAND_Y[2]), Point = (HAND_X[0], HAND_Y[2])},
            new Piece(bB2) { DefaultPoint = (HAND_X[1], HAND_Y[2]), Point = (HAND_X[1], HAND_Y[2])},

            new Piece(bA1) { DefaultPoint = (HAND_X[0], HAND_Y[0]), Point = (HAND_X[0], HAND_Y[0])},
            new Piece(bA2) { DefaultPoint = (HAND_X[1], HAND_Y[0]), Point = (HAND_X[1], HAND_Y[0])},
            new Piece(bA3) { DefaultPoint = (HAND_X[2], HAND_Y[0]), Point = (HAND_X[2], HAND_Y[0])},

            new Piece(bS1) { DefaultPoint = (HAND_X[0], HAND_Y[3]), Point = (HAND_X[0], HAND_Y[3])},
            new Piece(bS2) { DefaultPoint = (HAND_X[1], HAND_Y[3]), Point = (HAND_X[1], HAND_Y[3])},
        };

        public Board()
        {
            _hashed_evals = new Dictionary<long, Dictionary<string, float>>(9000);
            Hive = new Dictionary<(int, int), Stack<Piece>>(22);
            History = new Dictionary<Player, MoveHistory>
            {
                { Player.Black, new MoveHistory(3) },
                { Player.White, new MoveHistory(3) }
            };

            HistoryHeuristicPurposes = new Dictionary<Player, MoveHistory>
            {
                { Player.Black, new MoveHistory(3) },
                { Player.White, new MoveHistory(3) }
            };
        }

        public void ResetState()
        {
            Hive.Clear();
            for (int p = 0; p < 11; ++p)
            {
                WhitePieces[p].SetToDefault();
                BlackPieces[p].SetToDefault();
            }
            History[Player.Black].Clear();
            History[Player.White].Clear();

            HistoryHeuristicPurposes[Player.Black].Clear();
            HistoryHeuristicPurposes[Player.Black].Clear();

            // _hashed_evals.Clear(); // should we tho?
        }

        public Piece GetPieceReferenceByTagString(string p)
        {
            int piece = Int32.Parse(p);
            Player color = GetColorFromBin(piece);

            if (color == Player.Black)
            {
                // the INSECT_PARSER is more of an INDEX_PARSER
                return BlackPieces[piece & INSECT_PARSER];
            }
            else
            {
                return WhitePieces[piece & INSECT_PARSER];
            }
        }

        public Piece GetPieceReferenceByTagINT(int piece)
        {
            Player color = GetColorFromBin(piece);
            if (color == Player.Black)
            {
                // the INSECT_PARSER is more of an INDEX_PARSER
                return BlackPieces[piece & INSECT_PARSER];
            }
            else
            {
                return WhitePieces[piece & INSECT_PARSER];
            }
        }


        public void MovePiece(Piece piece, (int, int) to, bool saveToHistory = false)
        {
            if (piece == null)
                return;

            if (piece.IsOnHive)
                _RemovePiece(ref piece);

            if (to != piece.DefaultPoint)
                _PlacePiece(ref piece, to);

            // this boolean is necessary for the times that `MovePiece` is called inside _IsValidMove
            if (saveToHistory)
            {
                History[piece.Player].Add((piece, to)); // may be a bottleneck for the AI?
                HistoryHeuristicPurposes[piece.Player].Add((piece,to));
            }
        }

        // Expensive because it goes through all the pieces, and it also returns a clone (snapshot) of that piece
        // It returns a hashset of moves, where each move is a tuple of type (Piece, (int, int))
        public HashSet<(Piece piece, (int, int) to)> GenerateMovesForPlayer(Player curPlayer)
        {
            HashSet<(Piece, (int, int))> moves = new HashSet<(Piece, (int, int))>();

            if (!IsGameOver())
            {
                Piece playersQueenPiece = curPlayer == Player.Black ? BlackPieces[Q1] : WhitePieces[Q1];
                int manyPiecesPlayedByCurPlayer = _GetManyPiecesPlayedBy(curPlayer);

                foreach (Piece piece in curPlayer == Player.Black ? BlackPieces : WhitePieces)
                {
                    // UnityEngine.Debug.Log($"given player {curPlayer} the piece {piece} onhive: {piece.IsOnHive} isTop: {piece.IsTop} queenOnHive: {playersQueenPiece.IsOnHive} ManyPlayed: {manyPiecesPlayedByCurPlayer}");
                    int prevManyMoves = moves.Count;
                    if (Hive.Count > 0)
                    {
                        if (piece.IsOnHive)
                        {
                            if (piece.IsTop && playersQueenPiece.IsOnHive)
                            {
                                foreach ((int, int) spot in GetMovingSpotsFor(piece))
                                {
                                    moves.Add((piece.Clone(), spot));
                                }
                            }
                        }
                        else
                        {
                            if (manyPiecesPlayedByCurPlayer == 0 && !piece.Equals(playersQueenPiece))
                            {
                                foreach ((int, int) spot in Hive[(0, 0)].Peek().Sides)
                                {
                                    moves.Add((piece.Clone(), spot));
                                }
                            }
                            else if (manyPiecesPlayedByCurPlayer == 3 && !playersQueenPiece.IsOnHive)
                            {
                                foreach ((int, int) spot in GetPlacingSpotsFor(curPlayer))
                                {
                                    // Must play queen on their 4th turn
                                    moves.Add((playersQueenPiece.Clone(), spot));
                                }
                            }
                            else
                            {
                                foreach ((int, int) spot in GetPlacingSpotsFor(curPlayer))
                                {
                                    moves.Add((piece.Clone(), spot));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!piece.Equals(playersQueenPiece))
                        {
                            moves.Add((piece.Clone(), (0, 0)));
                        }
                    }
                    piece.IsPinned = prevManyMoves == moves.Count;
                }
            }
            return moves;
        }

        public HashSet<(int, int)> GenerateMovesForPieceThrowsException(Piece piece)
        {
            // Stopwatch stopwatch = new Stopwatch();
            // stopwatch.Start();

            HashSet<(int, int)> moves = new HashSet<(int, int)>();
            Piece playersQueenPiece = piece.Player == Player.Black ? BlackPieces[Q1] : WhitePieces[Q1];
            int manyPiecesPlayedByCurPlayer = _GetManyPiecesPlayedBy(piece.Player);
            // UnityEngine.Debug.Log($"{piece} onHive: {piece.IsOnHive} top: {piece.IsTop} queenOnHive: {playersQueenPiece.IsOnHive} many: {manyPiecesPlayedByCurPlayer}");
            if (Hive.Count > 0)
            {
                if (piece.IsOnHive)
                {
                    if (piece.IsTop && playersQueenPiece.IsOnHive)
                    {
                        foreach ((int, int) spot in GetMovingSpotsFor(piece))
                        {
                            moves.Add(spot);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("You must play your hacker to move this piece.");
                    }
                }
                else
                {
                    if (manyPiecesPlayedByCurPlayer == 0)
                    {
                        if (!piece.Equals(playersQueenPiece))
                        {
                            foreach ((int, int) spot in Hive[(0, 0)].Peek().Sides)
                            {
                                moves.Add(spot);
                            }
                        }
                        else
                        {
                            throw new ArgumentException("You cannot play your hacker on your first turn.");
                        }
                    }
                    else if (manyPiecesPlayedByCurPlayer == 3 && !playersQueenPiece.IsOnHive)
                    {
                        if (piece.Equals(playersQueenPiece))
                        {
                            foreach ((int, int) spot in GetPlacingSpotsFor(piece.Player))
                            {
                                // Must play queen on their 4th turn
                                moves.Add(spot);
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Your hacker must be played by your 4th turn.");
                        }
                    }
                    else
                    {
                        foreach ((int, int) spot in GetPlacingSpotsFor(piece.Player))
                        {
                            moves.Add(spot);
                        }
                    }
                }
            }
            else
            {
                if (!piece.Equals(playersQueenPiece))
                {
                    moves.Add((0, 0));
                }
                else
                {
                    throw new ArgumentException("Your hacker cannot be played on your first turn.");
                }
            }

            // stopwatch.Stop();
            // UnityEngine.Debug.Log("Generating Piece Moves took: " + stopwatch.Elapsed.TotalMilliseconds + "ms");
            if (moves.Count > 0)
            {
                return moves;
            }
            else
            {
                if (piece.Neighbors.Count == 5)
                    throw new ArgumentException("Blocked by freedom of movement.");
                else
                    throw new ArgumentException("You cannot break the circuit!");
            }
        }

        public HashSet<(int, int)> GenerateMovesForPiece(Piece piece)
        {
            // Stopwatch stopwatch = new Stopwatch();
            // stopwatch.Start();

            HashSet<(int, int)> moves = new HashSet<(int, int)>();
            Piece playersQueenPiece = piece.Player == Player.Black ? BlackPieces[Q1] : WhitePieces[Q1];
            int manyPiecesPlayedByCurPlayer = _GetManyPiecesPlayedBy(piece.Player);
            // UnityEngine.Debug.Log($"{piece} onHive: {piece.IsOnHive} top: {piece.IsTop} queenOnHive: {playersQueenPiece.IsOnHive} many: {manyPiecesPlayedByCurPlayer}");
            if (Hive.Count > 0)
            {
                if (piece.IsOnHive)
                {
                    if (piece.IsTop && playersQueenPiece.IsOnHive)
                    {
                        foreach ((int, int) spot in GetMovingSpotsFor(piece))
                        {
                            moves.Add(spot);
                        }
                    }
                }
                else
                {
                    if (manyPiecesPlayedByCurPlayer == 0)
                    {
                        if (!piece.Equals(playersQueenPiece))
                        {
                            foreach ((int, int) spot in Hive[(0, 0)].Peek().Sides)
                            {
                                moves.Add(spot);
                            }
                        }
                    }
                    else if (manyPiecesPlayedByCurPlayer == 3 && !playersQueenPiece.IsOnHive)
                    {
                        if (piece.Equals(playersQueenPiece))
                        {
                            foreach ((int, int) spot in GetPlacingSpotsFor(piece.Player))
                            {
                                // Must play queen on their 4th turn
                                moves.Add(spot);
                            }
                        }
                    }
                    else
                    {
                        foreach ((int, int) spot in GetPlacingSpotsFor(piece.Player))
                        {
                            moves.Add(spot);
                        }
                    }
                }
            }
            else
            {
                if (!piece.Equals(playersQueenPiece))
                {
                    moves.Add((0, 0));
                }
            }

            // stopwatch.Stop();
            // UnityEngine.Debug.Log("Generating Piece Moves took: " + stopwatch.Elapsed.TotalMilliseconds + "ms");
            return moves;
        }

        public HashSet<(int, int)> GetPlacingSpotsFor(Player curPlayer)
        {
            // Stopwatch stopwatch = new Stopwatch();
            // stopwatch.Start();

            // Maybe keep track of the visited ones with a hashmap and also pass it to the hasopponent neighbor?
            HashSet<(int, int)> positions = new HashSet<(int, int)>();
            foreach (Piece piece in curPlayer == Player.Black ? BlackPieces : WhitePieces)
            {
                if (piece.IsOnHive)
                {
                    // iterate through this piece's available spots
                    foreach ((int, int) spot in piece.OpenSpotsAround)
                    {
                        //      Not been visited        It is not neighboring an opponent
                        if (!positions.Contains(spot) && !_HasOpponentNeighbor(spot, curPlayer))
                        {
                            positions.Add(spot);
                        }
                    }
                }
            }
            // stopwatch.Stop();
            // PrintRed($"Generating Available Placing Spots for {color} Player took: {stopwatch.Elapsed.TotalMilliseconds} ms");
            return positions;
        }

        public HashSet<(int, int)> GetMovingSpotsFor(Piece piece)
        {
            // Return this piece's valid moving spots
            return piece.Insect switch
            {
                Insect.Grasshopper => _GetGrasshopperMovingSpots(ref piece),
                Insect.QueenBee => _GetQueenMovingSpots(ref piece),
                Insect.Beetle => _GetBeetleMovingSpots(ref piece),
                Insect.Ant => _GetAntMovingSpots(ref piece),
                _ => _GetSpiderMovingSpots(ref piece), // it is a spider
            };
        }

        public bool IsGameOver() => BlackPieces[Q1].IsSurrounded || WhitePieces[Q1].IsSurrounded || IsDraw();
        public bool IsDraw() => (BlackPieces[Q1].IsSurrounded && WhitePieces[Q1].IsSurrounded) || (History[Player.Black].RepeatedForDraw() && History[Player.White].RepeatedForDraw());
        public bool IsRepeatingMoves(Player curPlayer) => (HistoryHeuristicPurposes[curPlayer].RepeatedForDraw());

#region Placing/Moving on the Board helper methods
        private void _PlacePiece(ref Piece piece, (int, int) to)
        {
            if (piece.Insect == Insect.Beetle && Hive.ContainsKey(to))
            {
                Hive[to].Peek().IsTop = false;
                Hive[to].Push(piece);
            }
            else
            {
                Hive[to] = new Stack<Piece>();
                Hive[to].Push(piece);
            }
            piece.Point = to;
            piece.IsTop = true;
            piece.IsOnHive = true;
            _UpdateNeighborsAt(to);
        }

        private void _RemovePiece(ref Piece piece)
        {
            (int, int) removingSpot = piece.Point;
            if (Hive.ContainsKey(removingSpot))
            {
                Hive[removingSpot].Pop();
                if (Hive[removingSpot].Count == 0)
                {
                    Hive.Remove(removingSpot);
                }
                else
                {
                    Hive[removingSpot].Peek().IsTop = true;
                }
                piece.SetToDefault();
                _UpdateNeighborsAt(removingSpot);
            }
        }

        private void _PopulateNeighborsFor(Piece piece)
        {
            piece.Neighbors.Clear();
            foreach ((int, int) sidePoint in piece.Sides)
            {
                // bool IsNeighbor = (point % side == (0, 0));
                // (int, int) neighborPoint = (point.x + side.Value.Item1, point.y + side.Value.Item2);
                bool neighborExists = Hive.ContainsKey(sidePoint);
                if (neighborExists)
                {
                    piece.Neighbors.Add(sidePoint);
                }
            }
        }
        private void _UpdateNeighborsAt((int x, int y) point)
        {
            // if this is a busy spot
            if (Hive.ContainsKey(point))
            {
                // for each piece at this spot
                foreach (Piece piece in Hive[point])
                {
                    // re-populate its neighbors
                    _PopulateNeighborsFor(piece);

                    // go around its new neighboring spots
                    foreach ((int, int) neighborSpot in piece.Neighbors)
                    {
                        // and let the pieces at this spot know 
                        foreach (Piece neighborPiece in Hive[neighborSpot])
                        {
                            // that `piece` is their new neighbor
                            if (!neighborPiece.Neighbors.Contains(piece.Point))
                            {
                                neighborPiece.Neighbors.Add(piece.Point);
                            }
                        }
                    }
                }
            }
            // this is an open spot
            else
            {
                // so for every neighboring point
                for (int i = 0; i < MANY_SIDES; ++i)
                {
                    (int, int) neighborPoint = (point.x + SIDE_OFFSETS_ARRAY[i].x, point.y + SIDE_OFFSETS_ARRAY[i].y);

                    // if there are pieces to update
                    if (Hive.ContainsKey(neighborPoint))
                    {
                        // for each piece at this spot
                        foreach (Piece piece in Hive[neighborPoint])
                        {
                            // let them know that `point` is now an open spot
                            piece.Neighbors.Remove(point);
                        }
                    }
                }
            }
        }

        private bool _IsPinned(ref Piece piece)
        {
            if (!piece.IsTop || piece.IsSurrounded)
            {
                return true;
            }
            else
            {
                // old expensive way
                // return GenerateMovesForPiece(piece).Count() == 0;

                // simulate breaking the hive
                (int, int) oldPoint = piece.Point;
                // temporarily remove it
                _RemovePiece(ref piece);
                bool isOneHive = IsAllConnected();
                // put it back
                _PlacePiece(ref piece, oldPoint);
                // if it breaks the hive, then it is pinned
                return !isOneHive;
            }
        }

        private int _GetManyPiecesPlayedBy(Player color)
        {
            int counter = 0;
            if (color == Player.Black)
            {
                for (int p = 0; p < 11; ++p)
                {
                    if (BlackPieces[p].IsOnHive)
                        ++counter;
                }
            }
            else
            {
                for (int p = 0; p < 11; ++p)
                {
                    if (WhitePieces[p].IsOnHive)
                        ++counter;
                }
            }
            return counter;
        }
        
#endregion

#region Each Moving Spot Getter for `Piece`
        private HashSet<(int, int)> _GetAntMovingSpots(ref Piece piece)
        {
            // Stopwatch stopwatch = new Stopwatch();
            // stopwatch.Start();
            HashSet<(int x, int y)> positions = new HashSet<(int x, int y)>();
            (int x, int y) oldAntPosition = piece.Point;
            //  positions.Add(oldAntPosition);
            _AntDFS(ref piece, ref positions, piece.Point,piece.Point);

            // Because the last point it found is where this piece is now positioned
            // Move it back to where it was
            MovePiece(piece, oldAntPosition);

            // itself should not be included
            positions.Remove(oldAntPosition);

            // stopwatch.Stop();
            // PrintRed("Generating Ant Moves took: " + stopwatch.Elapsed.TotalMilliseconds + "ms");
            return positions;
        }

        private void _AntDFS(ref Piece piece, ref HashSet<(int x, int y)> positions, (int x, int y) curSpot, (int x, int y) from)
        {
            for (int i = 0; i < MANY_SIDES; ++i)
            {
                (int x, int y) nextSpot = (curSpot.x + SIDE_OFFSETS_ARRAY[i].x, curSpot.y + SIDE_OFFSETS_ARRAY[i].y);
                // UnityEngine.Debug.Log($"checking ants {piece.Tag} from: {curSpot} to: {nextSpot} !contain: {!positions.Contains(nextSpot)} valid: {_IsValidMove(ref piece, curSpot, nextSpot)}");
                // If it has not been visited    AND Is a valid move
                if (!positions.Contains(nextSpot) && _IsValidMove(piece, curSpot, nextSpot))
                {
                    // UnityEngine.Debug.Log($"Ant's {nextSpot} passed!");
                    positions.Add(nextSpot);
                    // This move is important because it needs to update its neighbors
                    // so that it can later be appropriately validated by the _IsOneHive
                    MovePiece(piece, nextSpot);
                    _AntDFS(ref piece, ref positions, nextSpot, curSpot);
                }
            }
            MovePiece(piece, from);
        }

        private HashSet<(int, int)> _GetBeetleMovingSpots(ref Piece piece)
        {
            // Stopwatch stopwatch = new Stopwatch();
            // stopwatch.Start();
            HashSet<(int, int)> validMoves = new HashSet<(int, int)>();
            foreach ((int, int) side in piece.Sides)
            {
                // Because the beetle can only go around, and on top of other pieces
                // just check if it can move from its current point -> side 
                if (_IsValidMove(piece, piece.Point, side))
                {
                    validMoves.Add(side);
                }
            }
            // stopwatch.Stop();
            // PrintRed("Generating Beetle moves took: " + stopwatch.Elapsed.TotalMilliseconds + "ms");
            return validMoves;
        }

        private HashSet<(int, int)> _GetGrasshopperMovingSpots(ref Piece piece)
        {
            // Stopwatch stopwatch = new Stopwatch();
            // stopwatch.Start();
            HashSet<(int x, int y)> positions = new HashSet<(int x, int y)>();
            for (int s = 0; s < MANY_SIDES; ++s)
            {
                (int x, int y) nextSpot = (piece.Point.x + SIDE_OFFSETS_ARRAY[s].x, piece.Point.y + SIDE_OFFSETS_ARRAY[s].y);
                bool firstIsValid = false;

                // Keep hopping over pieces
                while (Hive.ContainsKey(nextSpot))
                {
                    // until you find a spot
                    nextSpot = (nextSpot.x + SIDE_OFFSETS_ARRAY[s].x, nextSpot.y + SIDE_OFFSETS_ARRAY[s].y);
                    firstIsValid = true;
                }

                if (firstIsValid && _IsOneHive(piece, nextSpot))
                {
                    positions.Add(nextSpot);
                }
            }
            // stopwatch.Stop();
            // PrintRed("Generating grasshoper moves took: " + stopwatch.Elapsed.TotalMilliseconds + "ms");
            return positions;
        }

        private HashSet<(int, int)> _GetSpiderMovingSpots(ref Piece piece)
        {
            HashSet<(int x, int y)> positions = new HashSet<(int x, int y)>();
            HashSet<(int x, int y)> visited = new HashSet<(int x, int y)>();
            (int, int) oldSpiderPosition = piece.Point;
            const int SPIDER_MAX_STEP_COUNT = 3;
            _SpiderDFS(ref piece, ref positions, ref visited, piece.Point, 0, SPIDER_MAX_STEP_COUNT);

            // Because the last point it found is where this piece is now positioned
            // Move it back to where it was
            MovePiece(piece, oldSpiderPosition);
            return positions;
        }

        private void _SpiderDFS(ref Piece piece, ref HashSet<(int x, int y)> positions, ref HashSet<(int x, int y)> visited, (int x, int y) curSpot, int curDepth, int maxDepth)
        {
            if (curDepth == maxDepth)
            {
                visited.Add(curSpot);
                positions.Add(curSpot);
                return;
            }
            visited.Add(curSpot);
            for (int i = 0; i < MANY_SIDES; ++i)
            {
                (int x, int y) nextSpot = (curSpot.x + SIDE_OFFSETS_ARRAY[i].x, curSpot.y + SIDE_OFFSETS_ARRAY[i].y);

                // TO BENCHMARK: Because DFS may run into itself at one point, maybe this should be validated

                // It has not been visited      AND Is a valid move
                if (!visited.Contains(nextSpot) && (piece.Point != nextSpot) && _IsValidMove(piece, curSpot, nextSpot))
                {
                    // This move is important because it needs to update its neighbors
                    // so that it can later be appropriately validated by the _IsOneHive
                    MovePiece(piece, nextSpot);
                    _SpiderDFS(ref piece, ref positions, ref visited, nextSpot, curDepth + 1, maxDepth);
                }
                MovePiece(piece, curSpot);
            }
        }

        private HashSet<(int, int)> _GetQueenMovingSpots(ref Piece piece)
        {
            // Stopwatch stopwatch = new Stopwatch();
            // stopwatch.Start();
            HashSet<(int, int)> spots = new HashSet<(int, int)>();
            foreach ((int, int) openSpot in piece.OpenSpotsAround)
            {
                // Since the queen can only go around its open spots,
                // only keep such valid open spots around it
                if (_IsValidMove(piece, piece.Point, openSpot))
                {
                    spots.Add(openSpot);
                }
            }
            // stopwatch.Stop();
            // PrintRed("Elapsed time: " + stopwatch.Elapsed.TotalMilliseconds + "ms");
            return spots;
        }
#endregion

#region Helper Methods For Finding Valid Moves
        private bool _HasOpponentNeighbor((int x, int y) point, Player color)
        {
            // foreach ((int, int) side in SIDE_OFFSETS.Values)
            for (int i = 0; i < MANY_SIDES; ++i)
            {
                (int, int) potentialOpponentNeighborPosition = (point.x + SIDE_OFFSETS_ARRAY[i].x, point.y + SIDE_OFFSETS_ARRAY[i].y);
                // if (Pieces.ContainsKey(potentialOpponentNeighborPosition) && Pieces[potentialOpponentNeighborPosition].TryPeek(out Piece topPiece) && topPiece.Color != color)
                // If piece is on the board                                     And Is not the same color as the piece that is about to be placed
                if (Hive.ContainsKey(potentialOpponentNeighborPosition) && Hive[potentialOpponentNeighborPosition].Peek().Player != color)
                {
                    // Has an opponent neighbor
                    return true;
                }
            }
            // Checked each side, and no opponent's pieces were found
            return false;
        }

        private bool _IsValidMove(Piece piece, (int x, int y) from, (int x, int y) to)
        {
            // // UnityEngine.Debug.Log($"_IsValidMove says: {piece.Insect == Insect.Beetle || !Hive.ContainsKey(to)} {_IsFreedomOfMovement(ref piece, from, to)} {_IsOneHive(ref piece, to)}");
            // UnityEngine.Debug.Log($"is {piece} a beetle? {piece.Insect == Insect.Beetle} | trying {piece} from {from} to: {to}. Someone there? {Hive.ContainsKey(to)}");
            // if (Hive.ContainsKey(to))
            // {
            //     UnityEngine.Debug.Log($"given {piece} from {from} to {to} someone is at {to} and is {Hive[to].Peek()}");
            // }
            //  Only beetle can crawl on top of point (to)
            // UnityEngine.Debug.Log($"First condition on {piece} from: {from} to: {to} piece.Insect == Insect.Beetle || !Hive.ContainsKey(to): {piece.Insect == Insect.Beetle || !Hive.ContainsKey(to)}");
            // UnityEngine.Debug.Log(_IsFreedomOfMovement(ref piece, from, to));
            // UnityEngine.Debug.Log(piece);
            // UnityEngine.Debug.Log(to);
            // UnityEngine.Debug.Log(_IsOneHive(ref piece, to));
            return (piece.Insect == Insect.Beetle || !Hive.ContainsKey(to)) && _IsFreedomOfMovement(piece, from, to) && _IsOneHive(piece, to);
        }

        private bool _IsFreedomOfMovement(Piece piece, (int x, int y) from, (int x, int y) to)
        {
            (int offsetX, int offsetY) offset = (to.x - from.x, to.y - from.y);

            int index = Array.IndexOf(SIDE_OFFSETS_ARRAY, offset); // direction we're going

            (int x, int y) leftOffset = index == 0 ? SIDE_OFFSETS_ARRAY[5] : SIDE_OFFSETS_ARRAY[index - 1];
            (int x, int y) rightOffset = index == 5 ? SIDE_OFFSETS_ARRAY[0] : SIDE_OFFSETS_ARRAY[index + 1];

            (int x, int y) peripheralLeftSpot = (from.x + leftOffset.x, from.y + leftOffset.y);
            (int x, int y) peripheralRightSpot = (from.x + rightOffset.x, from.y + rightOffset.y);

            bool peripheralLeftIsNotItself = peripheralLeftSpot.x != piece.Point.x || peripheralLeftSpot.y != piece.Point.y;
            bool peripheralRightIsNotItself = peripheralRightSpot.x != piece.Point.x || peripheralRightSpot.y != piece.Point.y;

            if(piece.Insect == Insect.Beetle) {
                int LeftStackCount = Hive.ContainsKey(peripheralLeftSpot) ? Hive[peripheralLeftSpot].Count : 0;
                int RightStackCount = Hive.ContainsKey(peripheralRightSpot) ? Hive[peripheralRightSpot].Count : 0;
                int FromStackCount = Hive.ContainsKey(from) ? Hive[from].Count : 0;
                int ToStackCount = Hive.ContainsKey(to) ? Hive[to].Count : 0;
                ++ToStackCount;

                return FromStackCount >= ToStackCount
                ? !(LeftStackCount >= FromStackCount && RightStackCount >= FromStackCount)
                : !(LeftStackCount >= ToStackCount && RightStackCount >= ToStackCount);
            }

             bool noneOfThePeripheralsIsItself = peripheralLeftIsNotItself && peripheralRightIsNotItself;
             bool onlyOneSpotIsOpen = Hive.ContainsKey(peripheralLeftSpot) ^ Hive.ContainsKey(peripheralRightSpot);
             //                                              Right is itself             Someone MUST be there on the left             OR    Left is itself                    Someone MUST be there on the right 
             bool checkThatTheOppositePeripheralExists = (!peripheralRightIsNotItself && Hive.ContainsKey(peripheralLeftSpot)) || (!peripheralLeftIsNotItself && Hive.ContainsKey(peripheralRightSpot));
            //  UnityEngine.Debug.Log($"_IsFreedomOfMovement {piece} from {from} to {to} noneOfThePeripheralsIsItself {noneOfThePeripheralsIsItself} onlyOneSpotIsOpen {onlyOneSpotIsOpen} checkThatTheOppositePeripheralExists {checkThatTheOppositePeripheralExists}");

            // if (noneOfThePeripheralsIsItself)
            // {
            //     UnityEngine.Debug.Log($"_IsFreedomOfMovement one{piece} {piece.Point} from {from} to {to} RETURNS {onlyOneSpotIsOpen}");
            // }
            // else
            // {
            //     UnityEngine.Debug.Log($"_IsFreedomOfMovement check {piece} from {from} to {to} RETURNS {checkThatTheOppositePeripheralExists}");
            // }
            return noneOfThePeripheralsIsItself
                    // If it is a beetle, check it can crawl on   OR get off of piece at to/from point   OR Treat it as a normal piece
                    // ? ((isBeetle && (board.Pieces.ContainsKey(to) || board.Pieces.ContainsKey(from))) || onlyOneSpotIsOpen)
                    ? onlyOneSpotIsOpen
                    : checkThatTheOppositePeripheralExists;
        }

        private bool _IsOneHive(Piece piece, (int x, int y) to)
        {
            // UnityEngine.Debug.Log($"ONEHIVE {piece.Tag} from {piece.Point} to {to}");
            (int, int) oldPoint = piece.Point;
            HashSet<(int x, int y)> oldNeighbors = new HashSet<(int x, int y)>(piece.Neighbors);

            _RemovePiece(ref piece);
            if (!IsAllConnected())
            {
                // UnityEngine.Debug.Log($"move {piece} TAG {piece.Tag} from {oldPoint} to {to} breaks the hive");
                // place it back
                _PlacePiece(ref piece, oldPoint);
                // this move breaks the hive
                return false;
            }
            else
            {
                // Temporarily place this piece to the `to` point
                _PlacePiece(ref piece, to);

                // If it is a grasshopper, just check if it is all connected  
                if ((piece.Insect == Insect.Grasshopper
                // if it is a beetle, then it better be on top of another piece
                || (piece.Insect == Insect.Beetle && (Hive.ContainsKey(oldPoint) || Hive[to].Count > 1))
                // for any other piece, make sure it was always connected, and that it is still connected (at least one common neighbor between both different states)
                || piece.Neighbors.Overlaps(oldNeighbors))
                && IsAllConnected())
                {
                    // UnityEngine.Debug.Log($"PASSED ONE HIVE move {piece} TAG: {piece.Tag} from: {oldPoint} to: {to} VALID");
                    // move it back
                    MovePiece(piece, oldPoint);
                    // this is a valid move
                    return true;
                }
                else
                {
                    // UnityEngine.Debug.Log($"move {piece} from {oldPoint} {to} invalid move because connected {IsAllConnected()} overlaps: {piece.Neighbors.Overlaps(oldNeighbors)}");
                    // string o = "";
                    // foreach ((int, int) n in piece.Neighbors)
                    // {
                    //     o += $" {n} ";
                    // }
                    // UnityEngine.Debug.Log($"cur neighbors {o}");

                    // o = "";
                    // foreach ((int, int) n in oldNeighbors)
                    // {
                    //     o += $" {n} ";
                    // }
                    // UnityEngine.Debug.Log($"OLD neighbors {o}");
                    // move it back
                    MovePiece(piece, oldPoint);
                    // this is an invalid move
                    return false;
                }
            }
        }

        public bool IsAllConnected()
        {
            var visited = new HashSet<(int, int)>();
            if (Hive.Count > 0)
            {
                (int, int) start = Hive.Keys.First();
                visited.Add(start);
                return _HiveDFS(ref visited, start);
            }
            else
            {
                return true;
            }
        }

        private bool _HiveDFS(ref HashSet<(int, int)> visited, (int, int) curPoint)
        {
            foreach ((int, int) neighbor in Hive[curPoint].Peek().Neighbors)
            {
                if (!visited.Contains(neighbor)) {
                    visited.Add(neighbor);
                    if (Hive.Count == visited.Count)
                    {
                        return true;
                    }
                    _HiveDFS(ref visited, neighbor);
                }
            }
            return Hive.Count == visited.Count;
        }
#endregion
    }
}