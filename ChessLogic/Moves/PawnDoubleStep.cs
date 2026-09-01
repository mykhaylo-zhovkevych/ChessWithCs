using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLogic.Enum;

namespace ChessLogic
{
    public class PawnDoubleStep : Move
    {
        public override MoveType Type => MoveType.DoublePawn;
        public override Position FromPos { get; }
        public override Position ToPos { get; }

        private readonly Position skippedPos;

        public PawnDoubleStep(Position from, Position to)
        {
            FromPos = from;
            ToPos = to;
            skippedPos = new Position((from.Row + to.Row) / 2, (from.Column + to.Column) / 2);
        }

        public override bool Execute(Board board)
        {
            Player player = board[FromPos].Color;
            board.SetPawnSkipPosition(player, skippedPos);
            new NormalMove(FromPos, ToPos).Execute(board);
            return true;
        }
    }
}
