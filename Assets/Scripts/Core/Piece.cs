using System.Linq;
using System.Collections.Generic;
using static HiveCore.Utils;

#pragma warning disable IDE1006 // Private members naming style
#nullable enable

namespace HiveCore
{
    public class Piece
    {
        public int Tag { get; set; }
        public int Index { get; set; }
        public Player Player { get; set; }
        public bool IsOnHive { get; set; }
        public bool IsPinned { get; set; }
        public Insect Insect { get; set; }
        public HashSet<(int, int)> Moves { get; set; }
        public (int x, int y) Point { get; set; }
        public (int x, int y) DefaultPoint { get; set; }
        public HashSet<(int, int)> Sides { get {
            return new HashSet<(int, int)>
                {
                    (Point.x + SIDE_OFFSETS_ARRAY[0].x, Point.y + SIDE_OFFSETS_ARRAY[0].y), // [0] North
                    (Point.x + SIDE_OFFSETS_ARRAY[1].x, Point.y + SIDE_OFFSETS_ARRAY[1].y), // [1] Northwest
                    (Point.x + SIDE_OFFSETS_ARRAY[2].x, Point.y + SIDE_OFFSETS_ARRAY[2].y), // [2] Southwest
                    (Point.x + SIDE_OFFSETS_ARRAY[3].x, Point.y + SIDE_OFFSETS_ARRAY[3].y), // [3] South
                    (Point.x + SIDE_OFFSETS_ARRAY[4].x, Point.y + SIDE_OFFSETS_ARRAY[4].y), // [4] Southeast
                    (Point.x + SIDE_OFFSETS_ARRAY[5].x, Point.y + SIDE_OFFSETS_ARRAY[5].y), // [5] Northeast
                };
        } }
        public HashSet<(int, int)> Neighbors { get; set; }
        public (int x, int y) GetSidePointByStringDir (string dir) => (SIDE_OFFSETS[dir].x + Point.x, SIDE_OFFSETS[dir].y + Point.y);
        public string GetSideStringByPoint ((int x, int y) sidePoint) => SIDE_OFFSETS.Keys.First(dir => (GetSidePointByStringDir(dir).x == sidePoint.x) && (GetSidePointByStringDir(dir).y == sidePoint.y));
        public HashSet<(int, int)> OpenSpotsAround { get { return new HashSet<(int, int)>(Sides.Except(Neighbors)); } }
        public override string ToString() { return $"{char.ToLower(Player.ToString()[0])}{Insect.ToString()[0]}{PIECE_NUMBER[Index]}"; }
        public bool IsSurrounded { get {return Neighbors.Count == MANY_SIDES; } }
        // When calling `_PlacePiece` and `_RemovePiece` from `Board`, `IsTop` property should get updated
        public bool IsTop { get; set; }
        public Piece(int piece)
        {
            Player = GetColorFromBin(piece);
            Insect = GetInsectFromBin(piece);
            // passing MANY_SIDES to the constructor should "ensure capacity"
            // but Unity complains :/
            Neighbors = new HashSet<(int, int)>();
            Tag = piece;
            Index = piece & INSECT_PARSER;
            SetToDefault();
        }

        public void SetToDefault()
        {
            Neighbors.Clear();
            Point = DefaultPoint;
            IsOnHive = false;
            IsTop = false;
            IsPinned = false;
        }
        public Piece Clone()
        {
            return new Piece(this.Tag) {
                Player = this.Player,
                Insect = this.Insect,
                Point = this.Point,
                IsOnHive = this.IsOnHive,
                Neighbors = new HashSet<(int, int)>(this.Neighbors),
                Tag = this.Tag,
                Index = this.Index,
                IsPinned = this.IsPinned,
            };
        }

        public override bool Equals(object? obj)
        {
            return
            // if they are the same type
            obj is Piece p
            // and both have the same binary representation
            && Tag == p.Tag;
        }
    }
}