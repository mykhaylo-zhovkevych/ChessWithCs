using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
