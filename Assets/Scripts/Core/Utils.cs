using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable IDE1006 // Private members naming style
#nullable enable

namespace HiveCore
{
    public static class Utils
    {

        // Pieces represented by a binary number that maps to
        // their index in the pieces arrays–i.e., WhitePieces or BlackPieces
        public static readonly int G1 = 0b0000; // 0
        public static readonly int G2 = 0b0001; // 1
        public static readonly int G3 = 0b0010; // 2
        public static readonly int Q1 = 0b0011; // 3
        public static readonly int B1 = 0b0100; // 4
        public static readonly int B2 = 0b0101; // 5
        public static readonly int A1 = 0b0110; // 6
        public static readonly int A2 = 0b0111; // 7
        public static readonly int A3 = 0b1000; // 8
        public static readonly int S1 = 0b1001; // 9
        public static readonly int S2 = 0b1010; // 10

        // Color
        public static readonly int BLACK = 0b00100000; // 16
        public static readonly int WHITE = 0b00010000; // 32

        // Black Pieces
        public static readonly int bG1 = 0b010000;
        public static readonly int bG2 = 0b010001;
        public static readonly int bG3 = 0b010010;
        public static readonly int bQ1 = 0b010011;
        public static readonly int bB1 = 0b010100;
        public static readonly int bB2 = 0b010101;
        public static readonly int bA1 = 0b010110;
        public static readonly int bA2 = 0b010111;
        public static readonly int bA3 = 0b011000;
        public static readonly int bS1 = 0b011001;
        public static readonly int bS2 = 0b011010;

        // White Pieces
        public static readonly int wG1 = 0b100000;
        public static readonly int wG2 = 0b100001;
        public static readonly int wG3 = 0b100010;
        public static readonly int wQ1 = 0b100011;
        public static readonly int wB1 = 0b100100;
        public static readonly int wB2 = 0b100101;
        public static readonly int wA1 = 0b100110;
        public static readonly int wA2 = 0b100111;
        public static readonly int wA3 = 0b101000;
        public static readonly int wS1 = 0b101001;
        public static readonly int wS2 = 0b101010;



        // For parsing piece number 
        public static readonly int INSECT_PARSER = 0b00001111;
        // For parsing color
        public static readonly int COLOR_PARSER = 0b11110000;

        public static Player GetColorFromBin(int piece) {
            return (Player)(piece & COLOR_PARSER);
        }

        public static Insect GetInsectFromBin(int piece)
        {
            switch (piece & INSECT_PARSER)
            {
                case 0:
                case 1:
                case 2:
                    return Insect.Grasshopper;
                case 3:
                    return Insect.QueenBee;
                case 4:
                case 5:
                    return Insect.Beetle;
                case 6:
                case 7:
                case 8:
                    return Insect.Ant;
                default: // case 9 or 10
                    return Insect.Spider;
            };
        }

        public static readonly Dictionary<string, int> STRING_TO_BIN = new Dictionary<string, int>
        {
            {"bA1", bA1},
            {"bA2", bA2},
            {"bA3", bA3},
            {"bG1", bG1},
            {"bG2", bG2},
            {"bG3", bG3},
            {"bB1", bB1},
            {"bB2", bB2},
            {"bS1", bS1},
            {"bS2", bS2},
            {"bQ1", bQ1},
            {"wA1", wA1},
            {"wA2", wA2},
            {"wA3", wA3},
            {"wG1", wG1},
            {"wG2", wG2},
            {"wG3", wG3},
            {"wB1", wB1},
            {"wB2", wB2},
            {"wS1", wS1},
            {"wS2", wS2},
            {"wQ1", wQ1},
        };

        public static readonly int[] PIECE_NUMBER = {1, 2, 3, 1, 1, 2, 1, 2, 3, 1, 2};
        public static readonly int[] PIECE_GROUPING = {3, 1, 2, 3, 2};

        public static readonly int[] HAND_X = {335, 345, 355};
        public static readonly int[] HAND_Y = {180, 110, 40, -30, -100, -170};

        public static bool IsOnHand((int x, int y) point) => Array.Exists(HAND_X, handX => (handX == point.x) || (handX == -point.x)) && Array.Exists(HAND_Y, handY => (handY == point.y) || (handY == -point.y));

        public static readonly Dictionary<string, int> STRING_TO_INDEX = new Dictionary<string, int>
        {
            {"bA1", A1},
            {"bA2", A2},
            {"bA3", A3},
            {"bG1", G1},
            {"bG2", G2},
            {"bG3", G3},
            {"bB1", B1},
            {"bB2", B2},
            {"bS1", S1},
            {"bS2", S2},
            {"bQ1", Q1},
            {"wA1", A1},
            {"wA2", A2},
            {"wA3", A3},
            {"wG1", G1},
            {"wG2", G2},
            {"wG3", G3},
            {"wB1", B1},
            {"wB2", B2},
            {"wS1", S1},
            {"wS2", S2},
            {"wQ1", Q1},
        };

        public enum Player { Black = 16, White = 32 };
        // public enum Color { Black, White }
        public enum Insect { Ant, Grasshopper, QueenBee, Beetle, Spider }

        public const int MANY_SIDES = 6;
        public static readonly Dictionary<string, (int x, int y)> SIDE_OFFSETS = new Dictionary<string, (int x, int y)>
        {
            { "NT", (0, 56) },    // [0] North
            { "NW", (-48, 28) },  // [1] Northwest
            { "SW", (-48, -28) }, // [2] Southwest
            { "ST", (0, -56) },   // [3] South
            { "SE", (48, -28) },  // [4] Southeast
            { "NE", (48, 28) },   // [5] Northeast
        };

        public static readonly (int x, int y)[] SIDE_OFFSETS_ARRAY =
        {
            (0, 56),    // [0] North
            (-48, 28),  // [1] Northwest
            (-48, -28), // [2] Southwest
            (0, -56),   // [3] South
            (48, -28),  // [4] Southeast
            (48, 28),   // [5] Northeast
        };

        public static void UpdateStatesTable(long hash, int alpha, int beta, int eval) {
            string fullClassPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Scripts/Core/States.cs");
            string result;
            using (StreamReader reader = new StreamReader(fullClassPath)) {
                // Read the entire contents of the file into a string
                string contents = reader.ReadToEnd();
                string dictString = $"{{\"alpha\", {alpha}}}, {{\"beta\", {beta}}}, {{\"eval\", {eval}}}";
                string entryString = $"{{ {hash}, new Dictionary<string, int> {{ {dictString} }} }},";
                int existingHashKeyIndex = contents.IndexOf(hash.ToString());
                if (existingHashKeyIndex != -1)
                {
                    // Find the start and end index of the line containing the existing entry
                    int startLineIndex = contents.LastIndexOf('\n', existingHashKeyIndex) + 1;
                    int endLineIndex = contents.IndexOf('\n', existingHashKeyIndex);

                    // Replace the existing line with the new entry
                    result = contents.Substring(0, startLineIndex) + entryString + contents.Substring(endLineIndex);
                }
                else
                {
                    result = contents.Insert(contents.IndexOf("/**add**/"), $"\t{entryString}\n");
                }
            }

            using (StreamWriter writer = new StreamWriter(fullClassPath, false)) {
                writer.Write(result);
            }
        }

        public static void WriteToTXTFile(Dictionary<(int, int), Stack<Piece>> Pieces, long curHash, int alpha, int beta, int eval, string fileName = "./ht.txt")
        {
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                string output = "\n*************************** Board State *************************************";
                foreach (var s in Pieces)
                {
                    foreach (var p in s.Value)
                    {
                        output += $"\nCurrent piece with tag: {p.Tag}|{p} is at {p.Point} and has neighbors:";
                        foreach (var n in p.Neighbors)
                        {
                            output += $"\n-> {n}";
                        }
                    }
                }
                output += $"\nTherefore [{curHash}] = alpha: {alpha}, beta: {beta}, eval: {eval}";
                output += "\n*****************************************************************************";
                writer.WriteLine(output);
            }
        }

        public static void WhiteWriteToCSVFile(string output, string fileName = "WhiteHeuristicWeightsTable")
        {
            string fullCSVPath = Path.Combine(Directory.GetCurrentDirectory(), $"Assets/Resources/{fileName}.csv");

            using (StreamWriter writer = new StreamWriter(fullCSVPath, true))
            {
                writer.Write(output);
            }
        }

        public static void BlackWriteToCSVFile(string output, string fileName = "BlackHeuristicWeightsTable")
        {
            string fullCSVPath = Path.Combine(Directory.GetCurrentDirectory(), $"Assets/Resources/{fileName}.csv");

            using (StreamWriter writer = new StreamWriter(fullCSVPath, true))
            {
                writer.Write(output);
            }
        }

        public static void PrintYellow(string warning)
        {
            Debug.Log($"<color=yellow>{warning}</color>");
        }

        public static void PrintRed (string message)
        {
            Debug.Log($"<color=red>{message}</color>");
        }

        public static void PrintGreen (string message)
        {
            Debug.Log($"<color=red>{message}</color>");
        }
    }

    // Maybe using this later
    public struct Move
    {
        public Piece Piece { get; set; }
        public (int x, int y) To { get; set; }
        public Move(Piece piece, (int, int) to)
        {
            Piece = piece;
            To = to;
        }
    }
}