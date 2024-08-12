namespace ChessLogic
{
    public class Direction
    {
        public readonly static Direction North = new Direction(-1, 0);
        public readonly static Direction South = new Direction(1, 0);
        public readonly static Direction East = new Direction(0, 1);
        public readonly static Direction West = new Direction(0, -1);
        public readonly static Direction NorthWest = new Direction(-1, -1); // could've also used the overloaded plus operator like northwest = north + west; 
        public readonly static Direction SouthWest = new Direction(1, -1);
        public readonly static Direction NorthEast = new Direction(-1, 1);
        public readonly static Direction SouthEast = new Direction(1, 1);


        public int RowDelta { get; }// change in row position
        public int ColumnDelta { get; } // change in col position

        public Direction(int rowDelta, int columnDelta)
        {
            RowDelta = rowDelta;
            ColumnDelta = columnDelta;
        }

        //we need to override the plus operator to add two directions
        public static Direction operator +(Direction a, Direction b)
        {
            return new Direction(a.RowDelta + b.RowDelta, a.ColumnDelta + b.ColumnDelta);
        }

        //we also need to override the multiplication operator so that we can scale a direction

        public static Direction operator *(int scalar, Direction dir)
        {
            return new Direction(scalar * dir.RowDelta, scalar * dir.ColumnDelta);
        }
    }
}
