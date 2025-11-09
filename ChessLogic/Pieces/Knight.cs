using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ChessLogic
{
    public class Knight : Piece
    {
        public override PieceType Type => PieceType.Knight;
        public override Player Color { get; }

        public Knight(Player color)
        {
            Color = color;
        }

        public override Piece Copy()
        {
            Knight copy = new Knight(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        private static IEnumerable<Position> PotentialToPositions(Position fromPos)
        {
            foreach (Direction vDir in new Direction[] {Direction.North, Direction.South })
            {
                foreach (Direction hDir in new Direction[] {Direction.West, Direction.East })
                {
                    yield return fromPos + 2 * vDir + hDir;
                    yield return fromPos + 2 * hDir + vDir;
                }
            }
        }

        // Returns Position Where the Knight are allowed
        private IEnumerable<Position> MovePositions(Position fromPos, Board board)
        {
            return PotentialToPositions(fromPos).Where(pos => Board.IsInside(pos) && (board.IsEmpty(pos) || board[pos].Color != Color));
        }

        public override IEnumerable<Move> GetMoves(Position fromPos, Board board)
        {
            return MovePositions(fromPos, board).Select(toPos => new NormalMove(fromPos, toPos));
        }

    }
}