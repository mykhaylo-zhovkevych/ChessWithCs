using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLogic.Enum;

namespace ChessLogic
{
    // With this class will UI interact 
    public class GameState
    {
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }
        public Result Result { get; private set; } = null;
        private int noCaptureOrPawnMoveCount = 0;

        public GameState(Player player, Board board)
        {
            CurrentPlayer = player;
            Board = board;
        }

        // First function call
        public IEnumerable<Move> LegalMovesForPiece(Position pos)
        {
            if (Board.IsEmpty(pos) || Board[pos].Color != CurrentPlayer)
            {
                return Enumerable.Empty<Move>();
            }

            Piece piece = Board[pos];
            IEnumerable<Move> moveCandidates = piece.GetMoves(pos, Board);
            return moveCandidates.Where(move => move.IsLegal(Board));

        }

        public void MakeMove(Move move)
        {
            Board.SetPawnSkipPosition(CurrentPlayer, null);
            bool isCaptureOrPawnMove = move.Execute(Board);
            if (isCaptureOrPawnMove)
                noCaptureOrPawnMoveCount = 0;
            else
                noCaptureOrPawnMoveCount++;
            CurrentPlayer = CurrentPlayer.Opponent();
            CheckForGameOver();
        }

        public IEnumerable<Move> AllLegalMovesFor(Player player)
        {
            // Get all possible moves
            IEnumerable<Move> moveCandidates = Board.PiecePositionsFor(player).SelectMany(pos =>
            {
                Piece piece = Board[pos];
                return piece.GetMoves(pos, Board);
            });
            return moveCandidates.Where(move => move.IsLegal(Board));
        }            

        private void CheckForGameOver()
        {
            if (!AllLegalMovesFor(CurrentPlayer).Any())
            {
                if (Board.IsInCheck(CurrentPlayer))
                {
                    Result = Result.Win(CurrentPlayer.Opponent());
                }
                else
                {
                    Result = Result.Draw(EndReason.Stalemate);
                }
            }
            else if (Board.InsufficientMaterial())
            {
                Result = Result.Draw(EndReason.InsufficientMaterial);
            }
            else if (FiftyMoveRule())
            {
                Result = Result.Draw(EndReason.FiftyMoveRule);
            }
        }

        public bool IsGameOver()
        {
            return Result != null;
        }

        private bool FiftyMoveRule()
        {
            // because of each player
            int fullMoves = noCaptureOrPawnMoveCount / 2;
            return fullMoves >= 50;
        }
    }
}
