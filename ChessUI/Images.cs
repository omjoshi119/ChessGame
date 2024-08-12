using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChessLogic;


namespace ChessUI
{
    public static class Images
    {
        private static readonly Dictionary<PieceType, ImageSource> whiteSources = new() // key is piecetype and value is the image, so that we can lookup the image using the type of piece
        {
            {PieceType.Pawn, LoadImage("Assets/PawnW.png") },
            {PieceType.Bishop, LoadImage("Assets/BishopW.png") },
            {PieceType.Knight, LoadImage("Assets/KnightW.png") },
            {PieceType.Rook, LoadImage("Assets/RookW.png") },
            {PieceType.Queen, LoadImage("Assets/QueenW.png") },
            {PieceType.King, LoadImage("Assets/KingW.png") }
        };

        private static readonly Dictionary<PieceType, ImageSource> blackSources = new() // key is piecetype and value is the image, so that we can lookup the image using the type of piece
        {
            {PieceType.Pawn, LoadImage("Assets/PawnB.png") },
            {PieceType.Bishop, LoadImage("Assets/BishopB.png") },
            {PieceType.Knight, LoadImage("Assets/KnightB.png") },
            {PieceType.Rook, LoadImage("Assets/RookB.png") },
            {PieceType.Queen, LoadImage("Assets/QueenB.png") },
            {PieceType.King, LoadImage("Assets/KingB.png") }
        };

        private static ImageSource LoadImage(string filepath) // takes relative path of image as parameter
        {
            return new BitmapImage(new Uri(filepath, UriKind.Relative)); // returns a new bitmap image where we pass uri with given filepath and specify that is a relative path
        }

        public static ImageSource GetImage(Player color, PieceType type)
        {
            return color switch
            {
                Player.White => whiteSources[type],
                Player.Black => blackSources[type],
                _ => null
            };
        }

        public static ImageSource GetImage(Piece piece)
        {
            if(piece == null) return null;
            return GetImage(piece.Color, piece.Type);
        }
    }
} 
