namespace ChessLogic
{
    public abstract class Piece
    {
        public abstract PieceType Type { get; }
        public abstract Player Color { get; }
        public bool HasMoved { get; set; } = false; // coz some moves are only legal if a piece has not moved yet

        public abstract Piece Copy();

        public abstract IEnumerable<Move> GetMoves(Position from, Board board); // returns all possible moves for a piece given its position and the board

        protected IEnumerable<Position> MovePositionsInDir(Position from, Board board, Direction dir) // INLY FOR BISHOP, ROOK AND QUEEN WHICH CAN GO ALL THE WAY IN A GIVEN DIRECTION, WITHOUT JUMPING, we check for all squares in a given direction until we encounter a piece. If it is opponent's piece then it will also be a possible square. If it is our piece then we cannot capture it hence not a possible square.
        {
            for(Position pos = from + dir; Board.IsInside(pos); pos += dir)
            {
                if (board.IsEmpty(pos)) // no piece present
                {
                    yield return pos;
                    continue;
                }
                Piece piece = board[pos];
                if(piece.Color != Color) // opponent's piece
                {
                    yield return pos;
                }
                yield break; // our own piece
            }
        }

        protected IEnumerable<Position> MovePositionsInDirs(Position from, Board board, Direction[] dirs) // COLLECTS ALL REACHABLE POSITIONS FOR ALL DIRECTIONS
        {
            return dirs.SelectMany(dir => MovePositionsInDir(from, board, dir));
        }

        public virtual bool CanCaptureOpponentKing(Position from, Board board) // USED TO CHECK BEHIND THE SCENE TO DETECT "CHECK"
        {
            //GENERATE ALL MOVES A PIECE CAN MAKE
            return GetMoves(from, board).Any(move =>
            {
                Piece piece = board[move.ToPos];
                return piece != null && piece.Type == PieceType.King; // SEES IF THE PIECE AT A GIVEN "TO" POSITION IS A KING
            });
        }


    }
}
