using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class Castle : Move
    {
        public override MoveType Type { get; } // KING SIDE OR QUEEN SIDE
        public override Position FromPos { get; } // FROM AND TO POS ARE FOR THE KING, NOT THE ROOK
        public override Position ToPos { get; }

        private readonly Direction kingMoveDir;
        private readonly Position rookFromPos;
        private readonly Position rookToPos;

        public Castle(MoveType type, Position kingPos)
        {
            Type = type;
            FromPos = kingPos;

            if (type == MoveType.CastleKS)
            {
                kingMoveDir = Direction.East;
                ToPos = new Position(kingPos.Row, 6);
                rookFromPos = new Position(kingPos.Row, 7);
                rookToPos = new Position(kingPos.Row, 5);

            }
            else if (type == MoveType.CastleQS)
            {
                kingMoveDir = Direction.West;
                ToPos = new Position(kingPos.Row, 2);
                rookFromPos = new Position(kingPos.Row, 0);
                rookToPos = new Position(kingPos.Row, 3);
            }
        }

        public override bool Execute(Board board)
        {
            new NormalMove(FromPos, ToPos).Execute(board); // KING
            new NormalMove(rookFromPos, rookToPos).Execute(board);// ROOK

            return false; // SINCE THIS DOESNT CAPTURE OR DOESNT MOVE A PAWN -- USED OF COUNTING NUMBER OF MOVES FOR 50 MOVE RULE

        }

        public override bool IsLegal(Board board) // WE NEED TO OVERRIDE THIS COZ WE ALSO NEED TO MAKE SURE THAT KING IS NOT IN CHECK AFTER CASTLING, SINCE THE OG ISLEGAL CHECKED ONLY "BEFORE" THE MOVE SCENARIO
        {
            Player player = board[FromPos].Color;
            if (board.IsInCheck(player)) return false; // BEFORE THE MOVE

            Board copy = board.Copy();
            Position kingPosInCopy = FromPos;
            for(int i = 0; i < 2; i++)
            {
                new NormalMove(kingPosInCopy, kingPosInCopy + kingMoveDir).Execute(copy);
                kingPosInCopy += kingMoveDir;

                if (copy.IsInCheck(player)) return false;
            }
            return true;
        }

    }
}
