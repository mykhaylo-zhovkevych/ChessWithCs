using ChessLogic.Enum;

namespace ChessLogic.Moves
{
    // is he trying to recreate some pattern with this deligation of the methods? 
    public class PawnPromotion : Move
    {
        public override MoveType Type => MoveType.PawnPromotion;
        public override Position FromPos { get; }
        public override Position ToPos { get; }

        private readonly PieceType newType;

        public PawnPromotion(Position from, Position to, PieceType newType)
        {
            FromPos = from;
            ToPos = to;
            this.newType = newType;
        }

        private Piece CreatePromotionPiece(Player playerColor)
        {
            return newType switch
            {
                PieceType.Knight => new Knight(playerColor),
                PieceType.Bishop => new Bishop(playerColor),
                PieceType.Rook => new Rook(playerColor),
                _ => new Queen(playerColor)
            };
        }
        public override void Execute(Board board)
        {
            // Getting the pawn
            Piece pawn = board[FromPos];
            board[FromPos] = null; // Remove the pawn from its original position

            Piece promotionPiece = CreatePromotionPiece(pawn.Color);
            promotionPiece.HasMoved = true; // Mark the new piece as having moved
            board[ToPos] = promotionPiece;
        }
    }
}
