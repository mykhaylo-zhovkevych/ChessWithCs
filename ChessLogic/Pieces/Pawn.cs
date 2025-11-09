using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Namespaces and internal folders are independent
// This mean that Pawn is in the main folder and not in Pieces folder
namespace ChessLogic
{
    public class Pawn : Piece
    {

        public override PieceType Type => PieceType.Pawn;
        public override Player Color { get; }

        private readonly Direction forward;

        public Pawn(Player color)
        {
            Color = color;

            if (color == Player.White)
            {
                forward = Direction.North;
            }
            else if (color == Player.Black)
            {
                forward = Direction.South;
            }

        }

        public override Piece Copy()
        {
            Pawn copy = new Pawn(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        private static bool CanMoveTo(Position pos, Board board)
        {
            // 1. Dosent depend of the instance 2. Need concrete data(Instance) for method 
            return Board.IsInside(pos) && board.IsEmpty(pos);
        }

        private bool CanCaptureAt(Position pos, Board board)
        {
            if(!Board.IsInside(pos) || board.IsEmpty(pos))
            {
                return false;
            }
            return board[pos].Color != Color;

        }


        private IEnumerable<Move> ForwardMoves(Position fromPos, Board board)
        {
            Position oneMovePos = fromPos + forward;

            if (CanMoveTo(oneMovePos, board))
            {
                yield return new NormalMove(fromPos, oneMovePos);

                Position twoMovePos = oneMovePos + forward;

                if (!HasMoved && CanMoveTo(twoMovePos, board))
                {
                    yield return new NormalMove(fromPos, twoMovePos);
                }
            }
        }

        private IEnumerable<Move> DiagonalMoves(Position fromPos, Board board)
        {
            foreach (Direction dir in new Direction[] { Direction.West, Direction.East})
            {
                Position toPos = fromPos + forward + dir;

                if(CanCaptureAt(toPos, board))
                {
                    // 
                }
            }
        }

    }
}
