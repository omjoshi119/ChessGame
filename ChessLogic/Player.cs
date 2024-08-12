namespace ChessLogic
{
    public enum Player
    {
        None, // to set the winner to none (in case of draw)
        White, // white pieces
        Black // black pieces
    }

    public static class PlayerExtensions
    {
        public static Player Opponent(this Player player) // returns the opponent given the current player as a parameter
        {
            switch (player)
            {
                case Player.White:
                    return Player.Black;
                case Player.Black:
                    return Player.White;
                default: return Player.None;
            }
        }
    }
}
