using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ChessLogic.Enum;
using ChessLogic.Moves;

namespace ChessLogic
{
    public class King : Piece
    {
        public override PieceType Type => PieceType.King;
        public override Player Color { get; }

        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.North,
            Direction.NorthEast,
            Direction.East,
            Direction.SouthEast,
            Direction.South,
            Direction.SouthWest,
            Direction.West,
            Direction.NorthWest
        };

        public King(Player color)
        {
            Color = color;
        }

        private bool CanCastleKingSide(Position fromPos, Board board)
        {
            if (HasMoved)
            {
                return false;
            }
            Position rookPos = new Position(fromPos.Row, 7);
            Position[] betweenPositions = new Position[] { new(fromPos.Row, 5), new(fromPos.Row, 6) };

            return IsUnmovedRook(rookPos, board) && AllEmpty(betweenPositions, board); 
        }

        private bool CanCastleQueenSide(Position fromPos, Board board)
        {
            if (HasMoved)
            {
                return false;
            }
            Position rookPos = new Position(fromPos.Row, 0);
            Position[] betweenPositions = new Position[] { new(fromPos.Row, 1), new(fromPos.Row, 2), new(fromPos.Row, 3) };

            return IsUnmovedRook(rookPos, board) && AllEmpty(betweenPositions, board);
        }

        private static bool IsUnmovedRook(Position pos, Board board)
        {
            if (board.IsEmpty(pos))
            {
                return false;
            }

            Piece piece = board[pos];
            return !piece.HasMoved && PieceType.Rook == piece.Type;
        }

        private static bool AllEmpty(IEnumerable<Position> position, Board board)
        {
            return position.All(pos => board.IsEmpty(pos));
        }

        public override Piece Copy()
        {
            King copy = new King(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        private IEnumerable<Position> MovePositions(Position fromPos, Board board)
        {
            foreach (Direction dir in dirs)
            {
                Position toPos = fromPos + dir;

                if (!Board.IsInside(toPos))
                {
                    continue;
                }

                if (board.IsEmpty(toPos) || board[toPos].Color != Color)
                {
                    yield return toPos;
                }
            }
        }

        public override IEnumerable<Move> GetMoves(Position fromPos, Board board)
        {
            foreach (Position toPos in MovePositions(fromPos, board))
            {
                yield return new NormalMove(fromPos, toPos);
            }

            if (CanCastleKingSide(fromPos, board))
            {
                yield return new Castle(MoveType.CastleKS, fromPos);
            }

            if (CanCastleQueenSide(fromPos, board))
            {
                yield return new Castle(MoveType.CastleQS, fromPos);
            }
        }

        public override bool CanCaptureOpponentKing(Position from, Board baord)
        {
            return MovePositions(from, baord).Any(to =>
            {
                Piece piece = baord[to];
                return piece != null && piece.Type == PieceType.King;
            });
        }
    }
}
