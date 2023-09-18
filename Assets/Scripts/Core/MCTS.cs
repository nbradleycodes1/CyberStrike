using HiveCore;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using static HiveCore.Utils;
using static HiveCore.States;
using Move = System.ValueTuple<HiveCore.Piece, System.ValueTuple<int, int>>;

#pragma warning disable IDE1006 // Private members naming style
#nullable enable

namespace HiveCore
{
    public partial class Board
    {
        private readonly double _C = 100; // UCT constant for valueing expansion/exploitationg
        private const int _SIMULATION_DEPTH = 100;

        // KEEP TRACK OF NODE DEPTH ON THE TREE (not the simulation)

        Piece GetRandomPiece(Player player)
        {
            Random random = new Random();
            int index = random.Next(0, 11);
            return player == Player.Black ? BlackPieces[index] : WhitePieces[index];
        }

        // Based on the UCT from: https://github.com/cmelchior/hivemind/blob/4b76e92b8574b8d86c03e409a7c6ca62449f6852/src/main/java/dk/ilios/hivemind/ai/UCTMonteCarloTreeSearchAI.java
        public (Piece, (int, int)) MCTS(Player AIPlayer)
        {
            Node root = new Node(default, default!);
            root.Player = AIPlayer;
            Stopwatch time = new Stopwatch();
            time.Start();

            int manySimulations = 0;
            while (time.IsRunning && time.ElapsedMilliseconds <= 5999)
            {
                Node leaf = Selection(root);
                Node simulationStartNode = Expansion(leaf);
                float result = Simulation(AIPlayer);
                Backpropagation(simulationStartNode!, result);
                ++manySimulations;
            }

            time.Stop();
            UnityEngine.Debug.Log($"{manySimulations} simulatiosn in {time.Elapsed.TotalMilliseconds}ms");
            Node bestChild = GetBestChild(root); // Get the child node with the highest visit count
            return (bestChild.Piece, bestChild.To);
        }

        private Node Selection(Node node)
        {
            if (node.IsCompletelyVisited())
            {
                // UTC (Upper Condidence Bound for Trees)
                double bestScore = double.NegativeInfinity;
                Node bestNode = null;
                int terminalNodes = 0;
                foreach (Node child in node.Children.Values)
                {
                    if (child.IsTerminal)
                    {
                        ++terminalNodes;
                        continue; // Skip terminal nodes
                    }

                    double score = child.TotalResults + (_C * Math.Sqrt(Math.Log(node.VisitCount / child.VisitCount)));
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestNode = child;
                    }
                }

                // If no child can be selected, return parent node
                // TODO: ideally we should move up the tree and try another branch
                if (bestNode == null)
                {
                    return node; // Current node does not have any viable children
                }
                else
                {
                    return Selection(bestNode);
                }
            }
            else
            {
                // ASK NESTOR: What are we doing here? Why are we returning the node that does not have unexplored children?
                return node;
            }
        }

        private Node Expansion(Node leaf)
        {
            // Generate all moves from this node
            Piece piece = GetRandomPiece(leaf.Player);
            HashSet<(int, int)> moves = GenerateMovesForPiece(piece);

            // Keep looking until you find moves 
            while (moves.Count == 0)
            {
                piece = GetRandomPiece(leaf.Player);
                moves = GenerateMovesForPiece(piece);
            }
            leaf.MaxChildren = moves.Count;

            if (leaf.IsTerminal)
            {
                return leaf;
            }

            int movesCount = moves.Count;

            // 1) Start from a random position
            // 2) Check if move is already a a child, it yes, search right until move found (with loop back to start if needed)
            // 2a) As expand is only called on not fully visited nodes, this should always return a result
            // 3) Add move to tree as a node
            Random random = new Random();
            int moveIndex = random.Next(0, moves.Count);

            (int, int) to = moves.ElementAt(moveIndex);
            string nodeKey = $"{piece}{to}";

            while (leaf.Children.ContainsKey(nodeKey))
            {
                moveIndex = (moveIndex + 1) % moves.Count;
                to = moves.ElementAt(moveIndex);
                nodeKey = $"{piece}{to}";
                if (--movesCount < 0)
                {
                    UnityEngine.Debug.Log("Expand cannot find a new child node");
                    break;
                }
            }

            // Update tree with new node
            Node node = new Node((piece, moves.ElementAt(moveIndex)), leaf);
            leaf.Children[nodeKey] = node;
            return node;
        }

        private float Simulation(Player whoseTurn, int depth = _SIMULATION_DEPTH)
        {
            if (IsGameOver() || depth == 0)
            {
                Dictionary<string, int> heuristicWeights = new Dictionary<string, int>() {
                        {"queenSurroundedWeight", 8},
                        {"antOnBoardWeight", 5},
                        {"piecesOnBoardWeight", 3},
                        {"queenPinWeight", 3},
                        {"beetledQueenWeight", 5},
                        {"queenDefenderWeight", 7},
                        {"beetleQueenDistanceWeight", 30},
                        {"playQueenWeight", 15}
                };
                return  5;//_Evaluate(whoseTurn, -1, -1, heuristicWeights, );
            }
            else
            {
                float result = 0;
                foreach (Piece piece in whoseTurn == Player.Black ? BlackPieces : WhitePieces)
                {
                    foreach ((int, int) move in GenerateMovesForPiece(piece))
                    {
                        (int, int) oldPoint = piece.Point;
                        MovePiece(piece, move);
                        result = Simulation(whoseTurn == Player.Black ? Player.White : Player.Black, depth - 1);
                        MovePiece(piece, oldPoint);
                        return result;
                    }
                }
                return result;
            }
        }

        private void Backpropagation(Node node, float result)
        {
            while (node?.Parent != null)
            {
                node.VisitCount++;
                node.TotalResults += result;
                node = node.Parent;
            }
            node.VisitCount++;
            node.TotalResults += result;
        }

        private Node GetBestChild(Node root)
        {
            Random random = new Random();
            Node bestNode = null;
            double maxValue = double.NegativeInfinity;

            foreach (Node child in root.Children.Values)
            {
                //                                                          random True / False
                if (child.Value > maxValue || (child.Value == maxValue && random.NextDouble() > 0.5))
                {
                    maxValue = child.Value;
                    bestNode = child;
                }
            }

            // return (bestNode != null && bestNode.Piece != null) ? bestNode : PASS
            return bestNode;
        }
    }

    // Based on the GameNode class from: https://github.com/cmelchior/hivemind/blob/4b76e92b8574b8d86c03e409a7c6ca62449f6852/src/main/java/dk/ilios/hivemind/ai/AbstractMonteCarloTreeSearchAI.java
    public class Node
    {
        public Node? Parent { get; set; }
        public int MaxChildren { get; set; }
        // public List<Node> Children { get; set; }
        public Dictionary<string, Node> Children { get; set; }
        public string Key { get; set; }
        public Piece Piece { get; set; }
        public Player Player { get; set; }
        public (int, int) To { get; set; }
        public int VisitCount { get; set; }
        public float TotalResults { get; set; }
        public bool IsTerminal { get; set; }
        public float Value { get { return TotalResults / VisitCount; } }

        public Node((Piece piece, (int, int) to) move, Node parent)
        {
            Children = new Dictionary<string, Node>();
            // Children = new List<Node>();
            MaxChildren = -1;
            VisitCount = 0;
            Parent = parent;
            Piece = move.piece;
            To = move.to;
        }

        public bool IsCompletelyVisited()
        {
            foreach (Node child in Children.Values)
            {
                if (child.VisitCount == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }

    //  TODO: RandomMoveSelector() or some way to choose a random move

    // Need an AI that makes random moves every time and one AI that recognizes a win in one move

    // TODO: implement a way to reset the game and start again (after MAX_DEPTH turns, consider it a draw)

// T -> all nodes in the expanded tree
// Expansion(NodeN) -> expands a node and add its children to the tree if it is not expanded yet
// Simulation(NodeN) -> simulates a game from the given node to the end and returns the result (-1 [Loss], 0 [Draw], 1 [Win])
// Backpropagation(NodeN, R) -> updates the value of a node N using the result R of the simulated game
// children(NodeN) -> the list of children of a node N

// PseudoCode:

    //  Require: root_node
    //      while (has_time)
    //      {
    //          current_node ← root_node
    //          while (current_node ∈ T)
    //          {
    //              last_node ← current_node
    //              current_node ← Selection(current_node)
    //          }
    //
    //          last_node ← Expansion(last_node)
    //          R ← Simulation(last_node)
    //          current_node ← last_node
    //
    //          while (current_node ∈ T)
    //          {
    //              Backpropagation(current_node, R)
    //              current_node ← current_node.parent
    //          }
    //      }
    //      return best_move = get_max(children(root_node))



    // UCT -> Upper Coinfidence Bounds Applied to Trees

    // k -> child
    // i -> node
    // I -> set of nodes reachable from the current node p
    // p -> the current node
    // valuei -> the value of the node i
    // visitCounti -> the visit count of i
    // visitCountp -> the visit count of p
    // C is a constant and needs to be tuned experimentally (lower values, the AI plays more defensively, higher values => offensively)
    
    // myMove = max(i∈I) where ( valuei + C * ( sqrt[ ln(p)/i ] ) )

    // Basically, for every node that you visited, run it through the formula, and grab the highest result and use that move

}