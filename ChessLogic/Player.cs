using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLogic.Enum;

namespace ChessLogic
{

    // Extention Method
    public static class PlayerExtensions
    {
        public static Player Opponent(this Player player)
        {
            return player switch
            {
                Player.White => Player.Black,
                Player.Black => Player.White,
                _ => Player.None,
            };
        }
    }
}
