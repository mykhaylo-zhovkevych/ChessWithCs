using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLogic.Enum;
using ChessLogic;

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

        // Promotion moves would be returned in the body
        private static IEnumerable<Move> PromotionMoves(Position from, Position to)
        {
            yield return new PawnPromotion(from, to, PieceType.Knight);
            yield return new PawnPromotion(from, to, PieceType.Bishop);
            yield return new PawnPromotion(from, to, PieceType.Rook);
            yield return new PawnPromotion(from, to, PieceType.Queen);
        }

        private IEnumerable<Move> ForwardMoves(Position fromPos, Board board)
        {
            Position oneMovePos = fromPos + forward;

            if (CanMoveTo(oneMovePos, board))
            {
                if (oneMovePos.Row == 0 || oneMovePos.Row == 7)
                {
                    foreach (Move promotionMoves in PromotionMoves(fromPos, oneMovePos))
                    {
                        yield return promotionMoves;
                    }
                }
                else
                {
                    yield return new NormalMove(fromPos, oneMovePos);
                }

                Position twoMovePos = oneMovePos + forward;

                if (!HasMoved && CanMoveTo(twoMovePos, board))
                {
                    yield return new PawnDoubleStep(fromPos, twoMovePos);
                }
            }
        }

        private IEnumerable<Move> DiagonalMoves(Position fromPos, Board board)
        {
            foreach (Direction dir in new Direction[] { Direction.West, Direction.East})
            {
                Position toPos = fromPos + forward + dir;

                if (toPos == board.GetPawnSkipPosition(Color.Opponent()))
                {
                    yield return new EnPassantMove(fromPos, toPos);
                }

                else if (CanCaptureAt(toPos, board))
                {
                   if (toPos.Row == 0 || toPos.Row == 7)
                   {
                       foreach (Move promotionMoves in PromotionMoves(fromPos, toPos))
                       {
                           yield return promotionMoves;
                       }
                   }
                   else
                   {
                       yield return new NormalMove(fromPos, toPos);
                   }
                }
            }
        }

        public override IEnumerable<Move> GetMoves(Position fromPos, Board board)
        {
            return ForwardMoves(fromPos, board).Concat(DiagonalMoves(fromPos, board));
        }
        public override bool CanCaptureOpponentKing(Position from, Board baord)
        {
            return DiagonalMoves(from, baord).Any(move =>
            {
                Piece piece = baord[move.ToPos];
                return piece != null && piece.Type == PieceType.King;
            });
        }
    }
}