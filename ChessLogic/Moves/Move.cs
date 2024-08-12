using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public abstract class Move
    {
        public abstract MoveType Type { get; }
        public abstract Position FromPos { get; }
        public abstract Position ToPos { get; }
        public abstract bool Execute(Board board);  // RETURN TRUE ONLY IF A PIECE IS CAPTURED OR A PAWN IS MOVED

        public virtual bool IsLegal(Board board) // RETURNS TRUE IF EXECUTING THIS MOVE DOES NOT LEAVE THE CURRENT PLAYER'S KING IN CHECK
        {//WE MAKE A COPY OF THE CURRENT BOARD, EXECUTE THE MOVE OVER THERE AND SEE IF IT IS LEGAL OR NOT
            //METHOD IS VIRTUAL FOR ADDITINOAL CASTLING CONDNS TO BE ADDED LATER
            Player player = board[FromPos].Color;
            Board boardCopy = board.Copy();
            Execute(boardCopy);
            return !boardCopy.IsInCheck(player);
        }
    }
}
