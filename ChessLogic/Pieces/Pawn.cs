namespace ChessLogic
{
    public class Pawn : Piece // we are inheriting from Piece
    {
        public override PieceType Type => PieceType.Pawn;
        public override Player Color { get; }

        private readonly Direction forward;

        public Pawn(Player color) // constructor to set the color of the pawn
        {
            Color = color;
            if (color == Player.White) forward = Direction.North;
            else if(color == Player.Black) forward = Direction.South;
        }

        public override Piece Copy()
        {
            Pawn copy = new Pawn(Color); // we make a copy of the pawn with the same color and the same hasmoved value
            copy.HasMoved = HasMoved;
            return copy;
        }

        private static bool CanMoveTo(Position pos, Board board) // RETURNS TRUE IF PAWN CAN MOVE TO THE FORWARD POSITION
        {
            return Board.IsInside(pos) && board.IsEmpty(pos); // PAWN CAN ADVANCE ONLY IF THERE IS NO PIECE IN FRONT OF IT
        }

        private bool CanCaptureAt(Position pos, Board board)
        {
            if(!Board.IsInside(pos) || board.IsEmpty(pos)) return false; // cant capture diagonally if there is no piece present there
            return board[pos].Color != Color; // can capture if there is an opponent's piece on a diagonal square
        }

        private static IEnumerable<Move> PromotionMoves(Position from,  Position to) // RETURNS THE 4 POSSIBLE PROMOTION MOVES
        {
            yield return new PawnPromotion(from, to, PieceType.Knight);
            yield return new PawnPromotion(from, to, PieceType.Bishop);
            yield return new PawnPromotion(from, to, PieceType.Rook);
            yield return new PawnPromotion(from, to, PieceType.Queen);

        }

        private IEnumerable<Move> ForwardMove(Position from, Board board)
        {
            Position OneMovePos = from + forward;
            if (CanMoveTo(OneMovePos, board))
            {

                if(OneMovePos.Row == 0 || OneMovePos.Row == 7)
                {
                    foreach(Move promMove in PromotionMoves(from, OneMovePos))
                    {
                        yield return promMove; 
                    }
                }
                else
                {
                    yield return new NormalMove(from, OneMovePos);
                    Position TwoMovesPos = OneMovePos + forward;
                    if (!HasMoved && CanMoveTo(TwoMovesPos, board)) // TWO MOVES IS ONLY ALLOWED WHEN IT HASNT MOVED BEFORE
                    {
                        yield return new DoublePawn(from, TwoMovesPos); // CHANGED NORMALMOVE TO DOUBLEPAWN TO IMPLEMENT ENPASSANT
                    }
                }
            }
        }

        private IEnumerable<Move> DiagonalMoves(Position from, Board board)
        {
            foreach(Direction dir in new Direction[] { Direction.West, Direction.East })
            {
                Position to = from + forward + dir;

                if(to == board.GetPawnSkipPosition(Color.Opponent())) yield return new EnPassant(from,to);

                else if (CanCaptureAt(to, board))
                {
                    if (to.Row == 0 || to.Row == 7)
                    {
                        foreach (Move promMove in PromotionMoves(from, to))
                        {
                            yield return promMove;
                        }
                    }
                    else
                    {
                        yield return new NormalMove(from, to);
                    }
                }
            }
        }

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            return ForwardMove(from, board).Concat(DiagonalMoves(from,board));
        }

        public override bool CanCaptureOpponentKing(Position from, Board board)
        {
            return DiagonalMoves(from,board).Any(move =>
            {
                Piece piece = board[move.ToPos];
                return piece != null && piece.Type == PieceType.King;
            });
        }
    }
}
