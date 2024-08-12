using System.Text;

namespace ChessLogic
{
    public class StateString
    {
        private readonly StringBuilder sb = new StringBuilder();

        public StateString(Player currentPlayer, Board board) 
        {
            //Add piece placement data
            AddPiecePlacement(board);
            sb.Append(' ');
            //ADD CURRENT PLAYER
            AddCurrentPlayer(currentPlayer);
            sb.Append(' ');
            //ADD CASTLING RIGHTS
            AddCastlingRights(board);
            sb.Append(' ');
            //ADD EN PASSANT DATA
            AddEnPassant(board,currentPlayer);
        }

        public override string ToString()
        {
            return sb.ToString();
        }

        private static char PieceChar(Piece piece) // UPPERCASE WHITE PIECES, LOWERCASE BLACK PIECES
        {
            char c = piece.Type switch
            {
                PieceType.Pawn => 'p',
                PieceType.Bishop => 'b',
                PieceType.Knight => 'n',
                PieceType.Rook => 'r',
                PieceType.Queen => 'q',
                PieceType.King => 'k',
                _ => ' '
            };

            if (piece.Color == Player.White) return char.ToUpper(c);
            return c;

        }

        private void AddRowData(Board board, int row)
        {
            int empty = 0; // EMPTY SQUARES
            for (int c = 0; c < 8; c++)
            {
                if (board[row,c] == null)
                {
                    empty++;
                    continue;
                }

                if(empty > 0)
                {
                    sb.Append(empty);
                    empty = 0;
                }
                sb.Append(PieceChar(board[row,c]));
            }

            if (empty > 0) sb.Append(empty);
        }

        private void AddPiecePlacement(Board board) 
        {
            for(int r= 0 ; r < 8; r++)
            {
                if (r != 0) sb.Append('/');
                AddRowData(board, r);
            }
        }

        private void AddCurrentPlayer(Player currentPlayer)
        {
            if (currentPlayer == Player.White) sb.Append('w');
            else if (currentPlayer == Player.Black) sb.Append('b');
        }

        private void AddCastlingRights(Board board)
        {
            bool castleWKS = board.CastleRightKS(Player.White);
            bool castleWQS = board.CastleRightQS(Player.White);
            bool castleBKS = board.CastleRightKS(Player.Black);
            bool castleBQS = board.CastleRightQS(Player.Black);

            if(!(castleWKS || castleWQS || castleBKS || castleBQS))
            {
                sb.Append('-');
                return;
            }

            if (castleWKS) sb.Append('K');
            if (castleWQS) sb.Append('Q');
            if (castleBKS) sb.Append('k');
            if (castleBQS) sb.Append('q');

        }

        private void AddEnPassant(Board board, Player currentPlayer)
        {
            if (!board.CanCaptureEnPassant(currentPlayer))
            {
                sb.Append("-");
                return;
            }
            Position pos = board.GetPawnSkipPosition(currentPlayer.Opponent());
            char file = (char)('a' + pos.Column);
            int rank = 9 - pos.Row;
            sb.Append(file);
            sb.Append(rank);
        }
    }
}
