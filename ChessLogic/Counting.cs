using ChessLogic.Enum;

namespace ChessLogic
{
    public class Counting
    {
        private readonly Dictionary<PieceType, int> whiteCount = new();
        private readonly Dictionary<PieceType, int> blackCount = new();

        public int TotalCount { get; private set; }

        public Counting()
        {
            foreach (PieceType type in System.Enum.GetValues<PieceType>())
            {
                whiteCount[type] = 0;
                blackCount[type] = 0;

            }
        }

        public void IncrementCount(PieceType type, Player playerColor)
        {
            if (playerColor == Player.White)
            {
                whiteCount[type]++;
            }
            else if (playerColor == Player.Black)
            {
                blackCount[type]++;
            }
            TotalCount++;
        }

        public int White(PieceType type)
        {
            return whiteCount[type];
        }

        public int Black(PieceType type)
        {
            return blackCount[type];
        }

    }
}
