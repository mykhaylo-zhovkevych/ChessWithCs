using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    /// <summary>
    /// It is taken that thje top-right corner is (0,0)
    /// </summary>
    public class Position
    {
        public int Row { get; }
        public int Column { get; }

        public Position (int row, int column)
        {
            Row = row;
            Column = column;
        }
        // It return Player because Player has specifi color value
        public Player SquareColor()
        {
            // If is even
            if ((Row + Column) % 2 == 0)
            {
                return Player.White;
            }
            else
            {
                return Player.Black;
            }
        }


        // For creating internal method. Use `ctrl + .`
        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   Row == position.Row &&
                   Column == position.Column;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }

        public static bool operator ==(Position left, Position right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }

        // Returns one Position by adding one value of the Direction 
        public static Position operator +(Position pos, Direction dir)
        {
            return new Position(pos.Row + dir.RowDelta, pos.Column + dir.ColumnDelta);
        }
    }
}
