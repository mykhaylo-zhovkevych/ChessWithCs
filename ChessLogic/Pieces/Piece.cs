using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public abstract class Piece
    {
        public abstract PieceType Type { get; }
        public abstract Player Color { get; }
        public bool HasMoved { get; set; } = false;

        public abstract Piece Copy();

        public abstract IEnumerable<Move> GetMoves(Position fromPos, Board board);

        protected IEnumerable<Position> MovePositionsInDir(Position fromPos, Board board, Direction dir)
        {
            // If the position inside of the board continue untill the end of the board
            for (Position pos = fromPos + dir; Board.IsInside(pos); pos += dir)
            {
                if(board.IsEmpty(pos))
                {
                    yield return pos;
                    continue;
                }
                
                Piece piece = board[pos];

                if (piece.Color != Color)
                {
                    yield return pos;
                }

                yield break;

            }
       
        }
        // Creates lazy sequence of positions in multiple directions
        protected IEnumerable<Position> MovePositionsInDirs(Position fromPos, Board board, Direction[] dirs)
        {
            return dirs.SelectMany(dir => MovePositionsInDir(fromPos, board, dir));
        }

    }
}
