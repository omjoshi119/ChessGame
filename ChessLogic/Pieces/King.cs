namespace ChessLogic
{
    public class King : Piece
    {
        public override PieceType Type => PieceType.King; //This line defines a property `Type` in a class that overrides Piece class property to always return `PieceType.Bishop`.
        public override Player Color { get; }

        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.North,
            Direction.South,
            Direction.East,
            Direction.West,
            Direction.NorthEast,
            Direction.NorthWest,
            Direction.SouthEast,
            Direction.SouthWest,
        };
        public King(Player color)
        {
            Color = color;
        }

        private static bool IsRookUnmoved(Position pos, Board board) // TRUE IF ROOK IS UNMOVED AND ELIGIBLE FOR CASTLE
        {
            if(board.IsEmpty(pos)) return false;
            Piece piece = board[pos];
            return piece.Type == PieceType.Rook && !piece.HasMoved;
        }

        private static bool AllEmpty(IEnumerable<Position> positions, Board board) // TO SEE IF ALL SQUARES BETWEEN THE KING AND ROOK ARE EMPTY TO ALLOW CASTLING
        {
            return positions.All(pos => board.IsEmpty(pos));
        }

        private bool CanCastleKS(Position from, Board board) // from IS THE KING'S CURRENT POSN
        {
            if (HasMoved) return false;
            Position rookPos = new Position(from.Row, 7);
            Position[] betweenPositions = new Position[] { new(from.Row, 5), new(from.Row, 6) }; // SQUARES BETWEEN KING AND ROOK

            return IsRookUnmoved(rookPos,board) && AllEmpty(betweenPositions, board);

        }

        private bool CanCastleQS(Position from, Board board)
        {
            if (HasMoved) return false;
            Position rookPos = new Position(from.Row, 0);
            Position[] betweenPositions = new Position[] { new(from.Row, 1), new(from.Row, 2), new(from.Row, 3) };

            return IsRookUnmoved(rookPos,board) && AllEmpty(betweenPositions,board);
        }

        public override Piece Copy()
        {
            King copy = new King(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        private IEnumerable<Position> MovePositions(Position from, Board board)
        {
            foreach(Direction dir in dirs)
            {
                Position to = from + dir;
                if (!Board.IsInside(to)) continue;
                if (board.IsEmpty(to) || board[to].Color != Color) yield return to;
            }
        }

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            foreach (Position to in  MovePositions(from, board)) yield return new NormalMove(from,to);

            if(CanCastleKS(from,board)) yield return new Castle(MoveType.CastleKS,from);
            if(CanCastleQS(from,board)) yield return new Castle(MoveType.CastleQS,from);    

        }

        public override bool CanCaptureOpponentKing(Position from, Board board)
        {
            return MovePositions(from,board).Any(to =>
            {
                Piece piece = board[to];
                return piece != null && piece.Type == PieceType.King; 
            });
        }

    }
}
