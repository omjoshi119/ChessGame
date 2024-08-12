namespace ChessLogic
{
    public class Position
    {
        public int Row {  get; }
        public int Column { get; }

        public Position(int row, int column) // top left square is (0,0)
        { 
            Row = row; 
            Column = column; 
        }

        public Player SquareColor()
        {
            if ((Row + Column) % 2 == 0) return Player.White; // box with even coordinate sum will be white 
            return Player.Black; // else black
        }

        // equals and gethashcode and the overloaded operators are auto generated so that the position class can be used as a key in a dictionary
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

        public static Position operator +(Position pos, Direction dir)
        {
            return new Position(pos.Row + dir.RowDelta, pos.Column + dir.ColumnDelta);
        }

    }
}
