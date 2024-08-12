namespace ChessLogic
{
    public class EnPassant : Move
    {
        public override MoveType Type => MoveType.EnPassant;
        public override Position FromPos { get; }
        public override Position ToPos { get; }

        private readonly Position capturePos;

        public EnPassant(Position from, Position to)
        {
            FromPos = from;
            ToPos = to;

            //THIS IS A DIAGONAL MOVE INTO THE SKIPPED POSITION WHICH IS WHEN THE OPPONENTS PAWN IS IN OUR CURR ROW AND THE "TO" COLUMN
            capturePos = new Position(from.Row, to.Column);
        }

        public override bool Execute(Board board)
        {
            new NormalMove(FromPos, ToPos).Execute(board);
            board[capturePos] = null;

            return true;
        }
    }
}
