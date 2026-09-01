using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLogic.Enum;

namespace ChessLogic
{
    public class EnPassantMove : Move
    {

        public override MoveType Type => MoveType.EnPassant;
        public override Position FromPos { get; }
        public override Position ToPos { get; }

        private readonly Position capturePos;
        public EnPassantMove(Position fromPos, Position toPos)
        {
            FromPos = fromPos;
            ToPos = toPos;
            capturePos = new Position(fromPos.Row, toPos.Column);
        }

        public override bool Execute(Board board)
        {
            new NormalMove(FromPos, ToPos).Execute(board);
            board[capturePos] = null;
            return true;
        }
    }
}
